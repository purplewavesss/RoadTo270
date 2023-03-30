﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Python.Runtime;
using RoadTo270.Codecs;
using RoadTo270.Models;
using RoadTo270.ViewModels;

namespace RoadTo270.Views;

public partial class MainMenuView : UserControl
{
    public MainMenuView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }


    private async void LoadScenario(object? sender, RoutedEventArgs e)
    {
        // Declare lists
        List<Party> parties = new List<Party>();

        string? filePath = await PromptForFile();

        if (filePath is null) return;
        
        PythonEngine.Initialize();

        using (Py.GIL())
        {
            dynamic json = Py.Import("json");
            PyDict jsonContents = json.loads(await File.ReadAllTextAsync(filePath));

            var partiesData = jsonContents["Parties"].As<PyDict>();

            foreach (var partyObj in partiesData.Values())
            {
                var party = partyObj.As<PyDict>();
                var colors = PyListDecoder.Decode(party["Color"].As<PyList>());

                Tuple<int, int, int> colorValues = new Tuple<int, int, int>(colors[0], colors[1], colors[2]);
                
                parties.Add(new Party(party["Name"].As<string>(), colorValues));
            }
        }
    }

    private async Task<string?> PromptForFile()
    {
        var context = DataContext as MainMenuViewModel;
        var window = context!.GameApp.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        OpenFileDialog fileDialog = new OpenFileDialog
        { Filters = new List<FileDialogFilter> 
            { new() { Name = "JSON files", Extensions = { "json", "JSON" }} }, 
          AllowMultiple = false
        };
        var result = await fileDialog.ShowAsync(window!.MainWindow);

        return result?[0];
    }
}