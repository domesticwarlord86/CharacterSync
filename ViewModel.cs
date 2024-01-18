using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary;
using LlamaLibrary.Helpers;

namespace AnimaWeapons;

public class ViewModel
{
    private static ViewModel _settings;


    static ViewModel()
    {

    }

    public static ViewModel Instance => _settings ??= new ViewModel();

}
