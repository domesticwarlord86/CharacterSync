using System.ComponentModel;
using System.IO;
using System.Reflection;
using ff14bot.Enums;
using LlamaLibrary.JsonObjects;

namespace CharacterSync.Settings;
[Obfuscation(Exclude=true, ApplyToMembers=true)]
public class CharacterSync : JsonSettings<CharacterSync>
{

    public CharacterSync() : base(Path.Combine(CharacterSettingsDirectory, "CharacterSync.json"))
    {
    }

    private static string _charactedName;
    [Description("Character settings are being copied from")]
    [Category("Character")]
    public string CharactedName
    {
        get => _charactedName;
        set
        {
            if (_charactedName != value)
            {
                _charactedName = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _syncHotbars;
    [Description("Should we sync Hotbars from main to sub characters?")]
    [Category("Settings")]
    [DefaultValue(true)]
    public bool SyncHotbars
    {
        get => _syncHotbars;
        set
        {
            if (value == _syncHotbars)
            {
                return;
            }

            _syncHotbars = value;
            OnPropertyChanged();
        }
    }

    private bool _syncMacro;
    [Description("Should we sync Macros from main to sub characters?")]
    [Category("Settings")]
    [DefaultValue(true)]
    public bool SyncMacro
    {
        get => _syncMacro;
        set
        {
            if (value == _syncMacro)
            {
                return;
            }

            _syncMacro = value;
            OnPropertyChanged();
        }
    }

    private bool _syncKeybin;
    [Description("Should we sync Keybinds from main to sub characters?")]
    [Category("Settings")]
    [DefaultValue(true)]
    public bool SyncKeybind
    {
        get => _syncKeybin;
        set
        {
            if (value == _syncKeybin)
            {
                return;
            }

            _syncKeybin = value;
            OnPropertyChanged();
        }
    }

    private bool _syncLogfilter;
    [Description("Should we sync Logfilters from main to sub characters?")]
    [Category("Settings")]
    [DefaultValue(true)]
    public bool SyncLogfilter
    {
        get => _syncLogfilter;
        set
        {
            if (value == _syncLogfilter)
            {
                return;
            }

            _syncLogfilter = value;
            OnPropertyChanged();
        }
    }

    private bool _syncCharSettings;
    [Description("Should we sync CharSettings from main to sub characters?")]
    [Category("Settings")]
    [DefaultValue(true)]
    public bool SyncCharSettings
    {
        get => _syncCharSettings;
        set
        {
            if (value == _syncCharSettings)
            {
                return;
            }

            _syncCharSettings = value;
            OnPropertyChanged();
        }
    }

    private bool _syncKeyboardSettings;
    [Description("Should we sync KeyboardSettings from main to sub characters?")]
    [Category("Settings")]
    [DefaultValue(true)]
    public bool SyncKeyboardSettings
    {
        get => _syncKeyboardSettings;
        set
        {
            if (value == _syncKeyboardSettings)
            {
                return;
            }

            _syncKeyboardSettings = value;
            OnPropertyChanged();
        }
    }

    private bool _syncGamepadSettings;
    [Description("Should we sync GamepadSettings from main to sub characters?")]
    [Category("Settings")]
    [DefaultValue(true)]
    public bool SyncGamepadSettings
    {
        get => _syncGamepadSettings;
        set
        {
            if (value == _syncGamepadSettings)
            {
                return;
            }

            _syncGamepadSettings = value;
            OnPropertyChanged();
        }
    }

    private bool _syncCardSets;
    [Description("Should we sync Card Sets from main to sub characters?")]
    [Category("Settings")]
    [DefaultValue(true)]
    public bool SyncCardSets
    {
        get => _syncCardSets;
        set
        {
            if (value == _syncCardSets)
            {
                return;
            }

            _syncCardSets = value;
            OnPropertyChanged();
        }
    }

}