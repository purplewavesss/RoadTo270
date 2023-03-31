using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
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
        ImmutableArray<Party> parties = new ImmutableArray<Party>();
        ImmutableArray<Issue> issues = new ImmutableArray<Issue>();
        ImmutableArray<State> states = new ImmutableArray<State>();

        string? filePath = await PromptForFile();

        if (filePath is null) return;
        
        PythonEngine.Initialize();

        using (Py.GIL())
        {
            dynamic json = Py.Import("json");
            PyDict jsonContents = json.loads(await File.ReadAllTextAsync(filePath));

            parties = CreatePartyList(jsonContents["Parties"].As<PyDict>());
            issues = CreateIssuesList(jsonContents["Issues"].As<PyDict>());
            states = CreateStatesList(jsonContents["States"].As<PyDict>());
        }
        
        PythonEngine.Shutdown();
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

    private static ImmutableArray<Party> CreatePartyList(PyDict data)
    {
        List<Party> parties = new List<Party>();
        
        foreach (var partyObj in data.Values())
        {
            var party = partyObj.As<PyDict>();
            var colors = PyObjectDecoder.DecodeToList<int>(party["Color"].As<PyList>());

            Tuple<int, int, int> colorValues = new Tuple<int, int, int>(colors[0], colors[1], colors[2]);

            using (Py.GIL())
            {
                parties.Add(new Party(party["Name"].As<string>(), colorValues));
            }
        }

        return parties.ToImmutableArray();
    }
    
    private static ImmutableArray<Issue> CreateIssuesList(PyDict data)
    {
        Issue[] issues = new Issue[data.Length()];

        foreach (var issueObj in data.Values())
        {
            var issue = issueObj.As<PyDict>();
            var positions = PyObjectDecoder.DecodeToList<string>(issue["Positions"].As<PyList>()).ToImmutableArray();
            var constraints = PyObjectDecoder.DecodeToList<int>(issue["Constraints"].As<PyList>()).ToImmutableArray();

            issues[issue["Index"].As<int>()] = new Issue(issue["Name"].As<String>(), positions, constraints);
        }

        return issues.ToImmutableArray();
    }

    private ImmutableArray<State> CreateStatesList(PyDict data)
    {
        List<State> states = new List<State>();

        foreach (var stateObj in data.Values())
        {
            var state = stateObj.As<PyDict>();
            var name = state["Name"].As<string>();
            var context = GetMainWindow().DataContext as MainWindowViewModel;
            var statePath = context!.MainWindowMapView.Get<Avalonia.Controls.Shapes.Path>(name);
            var issuesScores = PyObjectDecoder.DecodeToArray<int>(state["IssueScores"].As<PyList>());
            
            states.Add(new State(name, issuesScores, statePath, state["Votes"].As<int>()));
        }

        return states.ToImmutableArray();
    }

    private MainWindow GetMainWindow()
    {
        var context = DataContext as MainMenuViewModel;
        var window = context!.GameApp.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return (window!.MainWindow as MainWindow)!;
    }
}