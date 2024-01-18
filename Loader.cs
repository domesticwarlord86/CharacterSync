using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ICSharpCode.SharpZipLib.Zip;
using LlamaLibrary;
using LlamaLibrary.Helpers;
using LlamaLibrary.Logging;
using LlamaLibrary.Memory;
using Newtonsoft.Json.Serialization;

namespace CharacterSyncLoader;

public class __TARGET__Loader : BotPlugin
{
    private const string ProjectName = "__TARGET__";
    private const string CompiledAssemblyName = "__TARGET__.dll";
    private const string VersionFileName = "Version.txt";
    private const string VersionUrl = @$"https://sts.llamamagic.net/__TARGET__/{VersionFileName}";
    private const string DataUrl = @$"https://sts.llamamagic.net/__TARGET__/{ProjectName}.zip";
    private const bool Debug = true;

    private static readonly LLogger Log = new("__TARGET__", Colors.Lime);
    private static readonly string VersionPath = Path.Combine(LoaderFolderName, VersionFileName);
    private static readonly string ProjectAssembly = Path.Combine(LoaderFolderName, $@"{CompiledAssemblyName}");

    private static string _name;
    private static PulseFlags _pulseFlags;
    private static Action _onButtonPress;
    private static Action _enable;
    private static Action _disable;
    private static Action _initialize;
    private static Action _shutdown;
    private static bool _wantButton;

    private static bool _updated;
    private static readonly object Locker = new();
    private Action _clear;
    private Version _version = new(1, 0);

    public __TARGET__Loader()
    {
        if (!Debug)
        {
            Log.Information("Checking for updates...");
            lock (Locker)
            {
                if (_updated) return;

                _updated = true;
            }

            Task.Run(async () => { await Update(LoaderFolderName); }).Wait();
        }

        try
        {
            Unblock(ProjectAssembly);
        }
        catch (Exception e)
        {
            Log.Error("Failed to unblock");
            Log.Exception(e);
        }

        Load();
    }

    private static string LoaderFolderName => GeneralFunctions.SourceDirectory().FullName;

    public override string Author => "DomesticWarlord";
    public override Version Version => _version;
    public override string Name => _name;

    public override bool WantButton => _wantButton;

    public override void OnButtonPress()
    {
        _onButtonPress?.Invoke();
    }

    public override void OnInitialize()
    {
        _initialize?.Invoke();
    }

    public override void OnEnabled()
    {
        _enable?.Invoke();
    }

    public override void OnDisabled()
    {
        _disable?.Invoke();
    }

    public override void OnShutdown()
    {
        _shutdown?.Invoke();
    }

    private void Load()
    {
        Log.Information("Starting constructor");

        RedirectAssembly();

        Log.Information("Redirected assemblies");

        if (!File.Exists(ProjectAssembly))
        {
            Log.Error($"Can't find {ProjectAssembly}");
            return;
        }

        var assembly = LoadAssembly(ProjectAssembly);
        if (assembly == null) return;

        Log.Information($"{assembly.GetName().Name} v{assembly.GetName().Version} loaded");
        Type baseType;
        try
        {
            baseType = assembly.DefinedTypes.FirstOrDefault(i => typeof(ICompiledPlugin).IsAssignableFrom(i));
        }
        catch (ReflectionTypeLoadException ex)
        {
            var sb = new StringBuilder();
            foreach (var exSub in ex.LoaderExceptions)
            {
                sb.AppendLine(exSub.Message);
                var exFileNotFound = exSub as FileNotFoundException;
                if (exFileNotFound != null)
                    if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                    {
                        sb.AppendLine("Fusion Log:");
                        sb.AppendLine(exFileNotFound.FusionLog);
                    }

                sb.AppendLine();
            }

            var errorMessage = sb.ToString();
            Log.Error(errorMessage);
            return;
            //Display or log the error based on your application.
        }
        catch (Exception e)
        {
            Log.Error("Other Exception");
            Log.Exception(e);
            return;
        }

        if (baseType == null)
        {
            Log.Error("Base type is null");
            return;
        }

        //dispatcher.BeginInvoke(new Action(() =>
        //{
        ICompiledPlugin compiledAsyncBotbase;
        try
        {
            compiledAsyncBotbase = (ICompiledPlugin)Activator.CreateInstance(baseType);
        }
        catch (ReflectionTypeLoadException ex)
        {
            var sb = new StringBuilder();
            foreach (var exSub in ex.LoaderExceptions)
            {
                sb.AppendLine(exSub.Message);
                var exFileNotFound = exSub as FileNotFoundException;
                if (exFileNotFound != null)
                    if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                    {
                        sb.AppendLine("Fusion Log:");
                        sb.AppendLine(exFileNotFound.FusionLog);
                    }

                sb.AppendLine();
            }

            var errorMessage = sb.ToString();
            Log.Error(errorMessage);
            return;
            //Display or log the error based on your application.
        }
        catch (Exception e)
        {
            Log.Error("Other Exception2");
            Log.Exception(e);
            return;
        }

        _name = compiledAsyncBotbase.Name;
        _enable = compiledAsyncBotbase.OnEnabled;
        _disable = compiledAsyncBotbase.OnDisabled;
        _onButtonPress = compiledAsyncBotbase.OnButtonPress;
        _wantButton = compiledAsyncBotbase.WantButton;
        _initialize = compiledAsyncBotbase.OnInitialize;
        _shutdown = compiledAsyncBotbase.OnShutdown;
        _version = compiledAsyncBotbase.Version;
    }

    private static Assembly? LoadAssembly(string path)
    {
        if (!File.Exists(path)) return null;

        Assembly? assembly = null;
        try
        {
            assembly = Assembly.LoadFrom(path);
        }
        catch (Exception e)
        {
            Logging.WriteException(e);
        }

        return assembly;
    }

    private static void RedirectAssembly()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            var name = Assembly.GetEntryAssembly()?.GetName().Name;
            var assemblyName = new AssemblyName(args.Name);
            Log.Debug($"Resolving assembly: {assemblyName.Name} {assemblyName.CultureInfo.Name}");

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var i = loadedAssemblies.Length - 1; i >= 0; i--)
            {
                var assembly = loadedAssemblies[i];
                if (assembly.FullName == args.Name)
                {
                    Log.Information("Assembly already loaded");
                    return assembly;
                }
            }

            if (assemblyName.Name.Contains("resources") && assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false)
            {
                var resource = $"{assemblyName.Name}.dll";
                var sourcePath = GeneralFunctions.SourceDirectory().FullName;
                var targetFile = Path.Combine(sourcePath, $"{assemblyName.CultureInfo}", resource);

                Log.Debug("Checking for: " + targetFile);
                if (File.Exists(targetFile))
                {
                    Log.Debug("Redirecting assembly to: " + targetFile);
                    return Assembly.LoadFrom(targetFile);
                    //return Assembly.Load(System.IO.File.ReadAllBytes(targetFile));
                }
            }

            if (assemblyName.Name == name) return Assembly.GetEntryAssembly();

            switch (assemblyName.Name)
            {
                case "Newtonsoft":
                    return typeof(JsonContract).Assembly;
                case "GreyMagic":
                    return Core.Memory.GetType().Assembly;
                case "ff14bot":
                    return Core.Me.GetType().Assembly;
                case "LlamaLibrary":
                    return typeof(OffsetManager).Assembly;
                default:
                    return null!;
            }
        };

        AppDomain.CurrentDomain.AssemblyResolve += AssemblyProxy.OnCurrentDomainOnAssemblyResolve;
    }

    private static async Task Update(string loaderFolderName)
    {
        var local = GetLocalVersion();
        var data = await TryUpdate(local);
        if (data == null) return;

        try
        {
            Clean(loaderFolderName);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }

        try
        {
            Extract(data, loaderFolderName);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }

    private static string GetLocalVersion()
    {
        if (!File.Exists(VersionPath)) return null;

        try
        {
            var version = File.ReadAllText(VersionPath);
            return GetNumbers(version);
        }
        catch
        {
            return null;
        }
    }

    private static void Clean(string directory)
    {
        foreach (var file in new DirectoryInfo(directory).GetFiles()) file.Delete();

        foreach (var dir in new DirectoryInfo(directory).GetDirectories()) dir.Delete(true);
    }

    private static async Task<byte[]> TryUpdate(string localVersion)
    {
        try
        {
            using var client = new HttpClient();
            var stopwatch = Stopwatch.StartNew();
            var version = GetNumbers(await client.GetStringAsync(VersionUrl));
            Log.Information($"Local: {localVersion} | Latest: {version}");

            if (string.IsNullOrEmpty(version) || version.Equals(localVersion)) return null;

            Log.Information($"{ProjectName} Updating....");
            using var response = await client.GetAsync(DataUrl);
            if (!response.IsSuccessStatusCode)
            {
                Log.Information($"[Error] Could not download {ProjectName}: {response.StatusCode}");
                return null;
            }

            using var inputStream = await response.Content.ReadAsStreamAsync();
            using var memoryStream = new MemoryStream();
            await inputStream.CopyToAsync(memoryStream);

            stopwatch.Stop();
            Log.Information($"Download took {stopwatch.ElapsedMilliseconds} ms.");

            return memoryStream.ToArray();
        }
        catch (Exception e)
        {
            Log.Information($"[Error] {e}");
            return null;
        }
    }

    private static string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c) || c == '.').ToArray());
    }

    private static void Extract(byte[] files, string directory)
    {
        using (var stream = new MemoryStream(files))
        {
            var zip = new FastZip();
            zip.ExtractZip(stream, directory, FastZip.Overwrite.Always, null, null, null, false, true);
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteFile(string name);

    private static bool Unblock(string fileName)
    {
        return DeleteFile(fileName + ":Zone.Identifier");
    }
}