using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using LlamaLibrary;
using LlamaLibrary.Helpers;
using LlamaLibrary.Logging;

namespace CharacterSync;
[Obfuscation(Exclude=true, ApplyToMembers=true)]
public class ExportedApi : IExportedApi
{
    private readonly LLogger Log = new("API", Colors.MediumPurple);

    public bool Init()
    {
        return true;
    }

    public async Task GoToBarracksTask()
    {
        Log.Information("Going to Barracks");

    }
}