﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Helpers;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using LlamaLibrary;
using LlamaLibrary.Helpers;
using LlamaLibrary.Helpers.NPC;
using LlamaLibrary.Logging;
using System.Windows.Controls;
using CharacterSync.Settings;
using LlamaLibrary.Extensions;

namespace CharacterSync;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class BasePlugin : TemplatePlugin, ICompiledPlugin
{
    internal static readonly string NameValue = "CharacterSync";

    private static Location lastLocation;

    internal static readonly LLogger Log = new(NameValue, Colors.Cyan);
    protected override Color LogColor { get; } = Colors.Cyan;

    private static readonly bool isSafeMode = false;

    private static readonly Regex SaveFolderRegex = new(
                                                 @"(?!ITEMODR\.DAT|ITEMFDR\.DAT|GEARSET\.DAT|UISAVE\.DAT|.*\.log)(?<dat>.*)",
                                                 RegexOptions.Compiled | RegexOptions.CultureInvariant);

    protected override Type SettingsForm { get; } = typeof(Form1);

    public IExportedApi Api { get; } = new ExportedApi();

    public override string Author => " DomesticWarlord";

    public override void OnEnabled()
    {
        var plugin = PluginManager.Plugins.FirstOrDefault(x => x.Plugin.Name == NameValue)?.Plugin;

        if (plugin != null)
        {
            RbButtonHelper.AddButton(plugin);
        }

        Log.Information($"{PluginName} Enabled");
    }

    public override void OnDisabled()
    {
        var plugin = PluginManager.Plugins.FirstOrDefault(x => x.Plugin.Name == NameValue)?.Plugin;

        if (plugin != null)
        {
            RbButtonHelper.RemoveButton(plugin);
        }

        Log.Information($"{PluginName} Disabled");
    }

    public override string PluginName => NameValue;

    public override Version Version => new(0, 0, 1);

    public override bool WantButton => true;
    public override string ButtonText => "Settings";

    public override void OnInitialize()
    {

        Log.Information($"Initializing {NameValue}");

    }

    public BasePlugin()
    {
    }

    protected override void OnBotStop(BotBase bot)
    {
        base.OnBotStop(bot);
    }

    private static void DoBackup()
    {
        var configFolder = Path.Combine(ff14bot.Helpers.Utils.AssemblyDirectory, "Plugins", "CharacterSync");

        if (!Directory.Exists(configFolder))
        {
            Directory.CreateDirectory(configFolder);
        }

        var backupFolder = new DirectoryInfo(Path.Combine(configFolder, "backups"));
        Directory.CreateDirectory(backupFolder.FullName);

        var folders = backupFolder.GetDirectories().OrderBy(x => long.Parse(x.Name)).ToArray();
        if (folders.Length > 2)
        {
            folders.FirstOrDefault()?.Delete(true);
        }

        var thisBackupFolder = Path.Combine(backupFolder.FullName, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
        Directory.CreateDirectory(thisBackupFolder);

        var xivFolder = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                       "My Games",
                                                       "FINAL FANTASY XIV - A Realm Reborn"));

        if (!xivFolder.Exists)
        {
            Log.Error("Could not find XIV folder.");
            return;
        }

        foreach (var directory in xivFolder.GetDirectories("FFXIV_CHR*"))
        {
            var thisBackupFile = Path.Combine(thisBackupFolder, directory.Name);
            Log.Information(thisBackupFile);
            Directory.CreateDirectory(thisBackupFile);

            foreach (var filePath in directory.GetFiles("*.DAT"))
            {
                File.Copy(filePath.FullName, filePath.FullName.Replace(directory.FullName, thisBackupFile), true);
            }
        }

        Log.Information("Backup OK!");
    }

    internal static async Task<bool> PluginTask()
    {

        //Log.Information("Backing up data");
        //DoBackup();

        var filepath = (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                                                 "My Games",
                                                                                 "FINAL FANTASY XIV - A Realm Reborn", $"FFXIV_CHR{LlamaLibrary.Extensions.LocalPlayerExtensions.PlayerId(Core.Player):X16}"));
        try
        {
            Log.Information("Trying to find directory");
            if (LlamaLibrary.Extensions.LocalPlayerExtensions.PlayerId(Core.Player) != 0)
            {
                var match = SaveFolderRegex.Match($"{filepath}");
                Log.Information($"Looking for match Value {match.Value}");
                Log.Information($"Looking for match Name {match.Name}");
                Log.Information($"Looking for match Groups {match.Groups}");
                Log.Information($"Looking for match Captures {match.Captures}");
                Log.Information($"Looking for match Length {match.Length}");
                if (match.Success)
                {
                    Log.Information("Match success");
                    var rootPath = match.Groups["path"].Value;
                    var datName = match.Groups["dat"].Value;

                    if (isSafeMode)
                    {
                        Log.Information($"SAFE MODE: {filepath}");
                    }
                    else if (PerformRewrite(datName))
                    {
                        filepath = $"{rootPath}FFXIV_CHR{LlamaLibrary.Extensions.LocalPlayerExtensions.PlayerId(Core.Player):X16}/{datName}";
                        Log.Information("REWRITE: " + filepath);
                    }
                }
                else
                {
                    Log.Error("No match found");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
            return false;
        }

        return true;
    }

    private static bool PerformRewrite(string datName)
    {
        switch (datName)
        {
            case "HOTBAR.DAT" when Settings.CharacterSync.Instance.SyncHotbars:
            case "MACRO.DAT" when Settings.CharacterSync.Instance.SyncMacro:
            case "KEYBIND.DAT" when Settings.CharacterSync.Instance.SyncKeybind:
            case "LOGFLTR.DAT" when Settings.CharacterSync.Instance.SyncLogfilter:
            case "COMMON.DAT" when Settings.CharacterSync.Instance.SyncCharSettings:
            case "CONTROL0.DAT" when Settings.CharacterSync.Instance.SyncKeyboardSettings:
            case "CONTROL1.DAT" when Settings.CharacterSync.Instance.SyncGamepadSettings:
            case "GS.DAT" when Settings.CharacterSync.Instance.SyncCardSets:
            case "ADDON.DAT":
                return true;
        }

        return false;
    }
}