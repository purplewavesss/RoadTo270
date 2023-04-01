using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        var parties = new ImmutableArray<Party>();
        var issues = new ImmutableArray<Issue>();
        var states = new ImmutableArray<State>();
        var candidates = new ImmutableArray<Candidate>();
        var tickets = new ImmutableArray<Ticket>();
        var candidateQuestions = new Dictionary<Ticket, List<Question>>().ToImmutableDictionary();
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

        var context = GetMainWindow().DataContext as MainWindowViewModel;
        context!.Game = new Scenario(parties, issues, states, candidates, tickets, candidateQuestions, currentQuestion);
    }

    private async Task<string?> PromptForFile()
    {
        OpenFileDialog fileDialog = new OpenFileDialog
        { Filters = new List<FileDialogFilter> 
            { new() { Name = "JSON files", Extensions = { "json", "JSON" }} }, 
          AllowMultiple = false
        };
        var result = await fileDialog.ShowAsync(GetMainWindow());

        return result?[0];
    }

    private static ImmutableArray<Party> CreatePartyList(PyDict data)
    {
        List<Party> parties = new List<Party>();
        
        foreach (var partyKey in data.Keys())
        {
            var party = data[partyKey].As<PyDict>();
            var colors = PyObjectDecoder.DecodeToList<int>(party["Color"].As<PyList>());

            Tuple<int, int, int> colorValues = new Tuple<int, int, int>(colors[0], colors[1], colors[2]);

            parties.Add(new Party(partyKey.As<string>(), colorValues));
        }

        return parties.ToImmutableArray();
    }
    
    private static ImmutableArray<Issue> CreateIssuesList(PyDict data)
    {
        Issue[] issues = new Issue[data.Length()];

        foreach (var issueKey in data.Keys())
        {
            var issue = data[issueKey].As<PyDict>();
            var positions = PyObjectDecoder.DecodeToList<string>(issue["Positions"].As<PyList>()).ToImmutableArray();
            var constraints = PyObjectDecoder.DecodeToList<int>(issue["Constraints"].As<PyList>()).ToImmutableArray();

            issues[issue["Index"].As<int>()] = new Issue(issueKey.As<string>(), positions, constraints);
        }

        return issues.ToImmutableArray();
    }

    private ImmutableArray<State> CreateStatesList(PyDict data)
    {
        List<State> states = new List<State>();

        foreach (var stateKey in data.Keys())
        {
            var state = data[stateKey].As<PyDict>();
            var context = GetMainWindow().DataContext as MainWindowViewModel;
            var statePath = context!.MainWindowMapView.Get<Avalonia.Controls.Shapes.Path>(Functions.RemoveSpaces(stateKey.As<string>()));
            var issuesScores = PyObjectDecoder.DecodeToArray<int>(state["IssueScores"].As<PyList>());
            
            states.Add(new State(Functions.RemoveSpaces(stateKey.As<string>()), issuesScores, 
                state["ElectoralVotes"].As<int>(), state["Votes"].As<int>(), statePath));
        }

        return states.ToImmutableArray();
    }
    
    private static ImmutableArray<Candidate> CreateCandidatesList(PyDict data, ImmutableArray<Party> parties, 
        ImmutableArray<State> states)
    {
        List<Candidate> candidates = new List<Candidate>();

        foreach (var candidateKey in data.Keys())
        {
            var candidate = data[candidateKey].As<PyDict>();
            var affiliation = NamedObject.GetObject(candidate["Affiliation"].As<string>(), parties) as Party;
            var homeState = NamedObject.GetObject(Functions.RemoveSpaces(candidate["HomeState"].As<string>()), states) as State;
            var issueScores = PyObjectDecoder.DecodeToArray<int>(candidate["IssueScores"].As<PyList>());
            var stateModifiers = PyObjectDecoder.DecodeToArray<double>(candidate["StateModifiers"].As<PyList>()).ToImmutableArray();
            candidates.Add(new Candidate(candidateKey.As<string>(), candidate["Description"].As<string>(),
                candidate["ImagePath"].As<string>(), candidate["AdvisorImagePath"].As<string>(),
                affiliation, homeState, issueScores, stateModifiers, candidate["IsRunningMate"].As<bool>()));
        }

        return candidates.ToImmutableArray();
    }

    private static ImmutableArray<Ticket> CreateTicketsList(PyDict data, ImmutableArray<Party> parties, ImmutableArray<Candidate> candidates)
    {
        List<Ticket> tickets = new List<Ticket>();

        foreach (var ticketKey in data.Keys())
        {
            var ticket = data[ticketKey].As<PyDict>();
            var president = NamedObject.GetObject(ticket["President"].As<string>(), candidates) as Candidate;
            var vicePresident = NamedObject.GetObject(ticket["VicePresident"].As<string>(), candidates) as Candidate;
            var affiliation = NamedObject.GetObject(ticket["Affiliation"].As<string>(), parties) as Party;
            tickets.Add(new Ticket(ticketKey.As<string>(), president, vicePresident, affiliation));
        }

        return tickets.ToImmutableArray();
    }
    
    private static ImmutableDictionary<Ticket, List<Question>> CreateQuestionsList(PyDict data, ImmutableArray<Issue> issues,
        ImmutableArray<State> states, ImmutableArray<Candidate> candidates, ImmutableArray<Ticket> tickets)
    {
        var ticketQuestions = new Dictionary<Ticket, List<Question>>();
        var questions = new List<Question>();

        foreach (var questionKey in data.Keys())
        {
            var question = data[questionKey].As<PyDict>();
            var askedTicketData = PyObjectDecoder.DecodeToArray<string>(question["AskedTickets"].As<PyList>());
            var askedTickets = askedTicketData.Select(askedTicket => NamedObject.GetObject(askedTicket, tickets) as Ticket).ToList();
            var options = CreateOptionsList(question["Options"].As<PyDict>(), issues, candidates, states);
            
            questions.Add(new Question(questionKey.As<string>(), askedTickets.ToImmutableArray(), options, 
                question["Randomize"].As<bool>()));
        }

        foreach (var ticket in tickets)
        {
            ticketQuestions.Add(ticket, (from question in questions
                                 where question.AskedTickets.Contains(ticket)
                                 select question).ToList());
        }

        return ticketQuestions.ToImmutableDictionary();
    }

    private static ImmutableArray<Option> CreateOptionsList(PyDict data, ImmutableArray<Issue> issues, 
        ImmutableArray<Candidate> candidates, ImmutableArray<State> states)
    {
        var options = new List<Option>();

        foreach (var optionsKey in data.Keys())
        {
            var option = data[optionsKey].As<PyDict>();
            var candidateEffectsData = option["CandidateEffects"].As<PyDict>();
            var stateEffectsData = option["StateEffects"].As<PyDict>();
            var issueEffectsData = PyObjectDecoder.DecodeToArray<int>(option["IssueEffects"].As<PyList>());
            
            var candidateEffects = new Dictionary<Candidate, double>();
            foreach (var candidateName in candidateEffectsData.Keys())
                candidateEffects.Add(NamedObject.GetObject(candidateName.As<string>(), candidates) as Candidate, 
                    candidateEffectsData[candidateName].As<double>());
            
            var stateEffects = new Dictionary<State, double>();
            foreach (var stateName in stateEffectsData.Keys())
                stateEffects.Add(NamedObject.GetObject(Functions.RemoveSpaces(stateName.As<string>()), states) as State, 
                    stateEffectsData[stateName].As<double>());

            var issueEffects = new Dictionary<Issue, double>();
            for (int issueIndex = 0; issueIndex < issues.Length; issueIndex++)
                issueEffects.Add(issues[issueIndex], issueEffectsData[issueIndex]);
            
            options.Add(new Option(optionsKey.As<string>(), candidateEffects.ToImmutableDictionary(), 
                stateEffects.ToImmutableDictionary(), issueEffects.ToImmutableDictionary(), option["Response"].As<string>()));
        }

        return options.ToImmutableArray();
    }

    private MainWindow GetMainWindow()
    {
        var context = DataContext as MainMenuViewModel;
        var window = context!.GameApp.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return (window!.MainWindow as MainWindow)!;
    }
}