using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using ff14bot;
using ff14bot.Enums;
using LlamaLibrary.Logging;

namespace CharacterSync;

public partial class Form1 : Form
{
    public static bool loading;
    private static readonly LLogger Log = new("Settings", Colors.MediumSeaGreen);

    public Form1()
    {
        InitializeComponent();
        loading = true;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        propertyGrid1.SelectedObject = Settings.CharacterSync.Instance;
        propertyGrid1.Update();

        loading = false;
    }

    public static void BindField(Control control, string propertyName, object dataSource, string dataMember)
    {
        Binding bd;

        for (var index = control.DataBindings.Count - 1; index == 0; index--)
        {
            bd = control.DataBindings[index];
            if (bd.PropertyName == propertyName)
            {
                control.DataBindings.Remove(bd);
            }
        }

        try
        {
            control.DataBindings.Add(propertyName, dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }
        catch (Exception e)
        {
            Log.Information($"{control.Name} {propertyName}");
            Log.Exception(e);
        }
    }

    private async void copyButton_Click(object sender, EventArgs e)
    {
        Log.Information($"Copying character data from {Core.Player.Name}");
        Settings.CharacterSync.Instance.CharactedName = Core.Player.Name;

        await CharacterSync.BasePlugin.PluginTask();
    }

    private void propertyGrid1_Click(object sender, EventArgs e)
    {
        propertyGrid1.SelectedObject = Settings.CharacterSync.Instance;
    }
}