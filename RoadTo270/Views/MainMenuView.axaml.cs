﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
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
            candidates = CreateCandidatesList(jsonContents["Candidates"].As<PyDict>(), parties, states, filePath);
            tickets = CreateTicketsList(jsonContents["Tickets"].As<PyDict>(), parties, candidates);
            candidateQuestions = CreateQuestionsList(jsonContents["Questions"].As<PyDict>(), issues, states,
                candidates, tickets);
            currentQuestion = jsonContents["CurrentQuestion"].As<int>();
        }
        
        PythonEngine.Shutdown();

        var context = Functions.GetMainWindow(DataContext as MainMenuViewModel).DataContext as MainWindowViewModel;
        context!.Game = new Scenario(parties, issues, states, candidates, tickets, candidateQuestions, currentQuestion);
        LoadCandidateMenu(context.Game);
    }

    private void LoadCandidateMenu(Scenario scenario)
    {
        var context = DataContext as MainMenuViewModel;
        var candidateMenu = new PresidentSelectionViewModel(context.GameApp, scenario, (DataContext as MainMenuViewModel)!);
        var windowContext = Functions.GetMainWindow(context).DataContext as MainWindowViewModel;
        windowContext!.Content = candidateMenu;
    }
    
    private async Task<string?> PromptForFile()
    {
        OpenFileDialog fileDialog = new OpenFileDialog
        { Filters = new List<FileDialogFilter> { new() { Name = "JSON files", Extensions = { "json", "JSON" }} }, 
            AllowMultiple = false
        };
        var result = await fileDialog.ShowAsync(Functions.GetMainWindow(DataContext as MainMenuViewModel));
    
        return result?[0];
    }
}