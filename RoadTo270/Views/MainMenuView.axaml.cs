using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Python.Runtime;
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
    
    private void Credits(object? sender, RoutedEventArgs e)
    {
        var context = Functions.GetMainWindow(DataContext as MainMenuViewModel).DataContext as MainWindowViewModel;
        context!.Content = context.MainWindowCreditsViewModel;
    }
    
    private async void LoadScenario(object? sender, RoutedEventArgs e)
    {
        ImmutableArray<Party> parties;
        ImmutableArray<Issue> issues;
        ImmutableArray<State> states;
        ImmutableArray<Candidate> candidates;
        ImmutableArray<Ticket> tickets; 
        ImmutableDictionary<Ticket, List<Question>> candidateQuestions;
        int currentQuestion;
        
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
            candidates = CreateCandidatesList(jsonContents["Candidates"].As<PyDict>(), parties, states);
            tickets = CreateTicketsList(jsonContents["Tickets"].As<PyDict>(), parties, candidates);
            candidateQuestions = CreateQuestionsList(jsonContents["Questions"].As<PyDict>(), issues, states,
                candidates, tickets);
            currentQuestion = jsonContents["CurrentQuestion"].As<int>();
        }
        
        PythonEngine.Shutdown();

        var context = Functions.GetMainWindow(DataContext as MainMenuViewModel).DataContext as MainWindowViewModel;
        context!.Game = new Scenario(parties, issues, states, candidates, tickets, candidateQuestions, currentQuestion);
    }
}