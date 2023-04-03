using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Python.Runtime;
using RoadTo270.Codecs;
using RoadTo270.Models;
using RoadTo270.ViewModels;

namespace RoadTo270.Views;

public partial class MainMenuView : UserControl
{
    private async Task<string?> PromptForFile()
    {
        OpenFileDialog fileDialog = new OpenFileDialog
        { Filters = new List<FileDialogFilter> 
            { new() { Name = "JSON files", Extensions = { "json", "JSON" }} }, 
          AllowMultiple = false
        };
        var result = await fileDialog.ShowAsync(Functions.GetMainWindow(DataContext as MainMenuViewModel));

        return result?[0];
    }

    private static ImmutableArray<Party> CreatePartyList(PyDict data)
    {
        List<Party> parties = new List<Party>();

        try
        {
            foreach (var partyKey in data.Keys())
            {
                var party = data[partyKey].As<PyDict>();
                var colors = PyObjectDecoder.DecodeToList<int>(party["Color"].As<PyList>());

                Tuple<int, int, int> colorValues = new Tuple<int, int, int>(colors[0], colors[1], colors[2]);

                parties.Add(new Party(partyKey.As<string>(), colorValues));
            }

            return parties.ToImmutableArray();
        }
        catch (Exception)
        {
            throw new PartySerializationException();
        }
    }
    
    private static ImmutableArray<Issue> CreateIssuesList(PyDict data)
    {
        try
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
        catch (Exception)
        {
            throw new IssueSerializationException();
        }
    }

    private ImmutableArray<State> CreateStatesList(PyDict data)
    {
        try
        {
            List<State> states = new List<State>();

            foreach (var stateKey in data.Keys())
            {
                var state = data[stateKey].As<PyDict>();
                var context = Functions.GetMainWindow(DataContext as MainMenuViewModel).DataContext as MainWindowViewModel;
                var statePath = context!.MainWindowMapView.Get<Avalonia.Controls.Shapes.Path>(Functions.RemoveSpaces(stateKey.As<string>()));
                var issuesScores = PyObjectDecoder.DecodeToArray<int>(state["IssueScores"].As<PyList>());
            
                states.Add(new State(Functions.RemoveSpaces(stateKey.As<string>()), issuesScores, 
                    state["ElectoralVotes"].As<int>(), state["Votes"].As<int>(), statePath));
            }

            return states.ToImmutableArray();
        }
        catch (Exception)
        {
            throw new StateSerializationException();
        }
    }
    
    private static ImmutableArray<Candidate> CreateCandidatesList(PyDict data, ImmutableArray<Party> parties, 
        ImmutableArray<State> states)
    {
        try
        {
            List<Candidate> candidates = new List<Candidate>();

            foreach (var candidateKey in data.Keys())
            {
                var candidate = data[candidateKey].As<PyDict>();
                var affiliation = NamedObject.GetObject(candidate["Affiliation"].As<string>(), parties) as Party;
                var homeState = NamedObject.GetObject(Functions.RemoveSpaces(candidate["HomeState"].As<string>()), states) as State;
                var issueScores = PyObjectDecoder.DecodeToArray<int>(candidate["IssueScores"].As<PyList>());

                var stateModifiers = new Dictionary<State, double>();
                var stateModifiersKeys = new List<State>();
                var stateModifiersValues = new List<double>();
                
                foreach (var stateName in candidate["StateModifiers"].As<PyDict>().Keys())
                {
                    stateModifiersKeys.Add(NamedObject.GetObject(Functions.RemoveSpaces(stateName.As<string>()), states) as State);
                }
                
                foreach (var modifier in candidate["StateModifiers"].As<PyDict>().Values())
                {
                    stateModifiersValues.Add(modifier.As<double>());
                }

                for (int stateIndex = 0; stateIndex < stateModifiersKeys.Count; stateIndex++)
                {
                    stateModifiers.Add(stateModifiersKeys[stateIndex], stateModifiersValues[stateIndex]);
                }
                
                candidates.Add(new Candidate(candidateKey.As<string>(), candidate["Description"].As<string>(),
                    candidate["ImagePath"].As<string>(), candidate["AdvisorImagePath"].As<string>(),
                    affiliation, homeState, issueScores, stateModifiers.ToImmutableDictionary(), 
                    candidate["IsRunningMate"].As<bool>()));
            }

            return candidates.ToImmutableArray();
        }
        catch (Exception)
        {
            throw new CandidateSerializationException();
        }
    }

    private static ImmutableArray<Ticket> CreateTicketsList(PyDict data, ImmutableArray<Party> parties, ImmutableArray<Candidate> candidates)
    {
        try
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
        catch (Exception)
        {
            throw new TicketSerializationException();
        }
    }
    
    private static ImmutableDictionary<Ticket, List<Question>> CreateQuestionsList(PyDict data, ImmutableArray<Issue> issues,
        ImmutableArray<State> states, ImmutableArray<Candidate> candidates, ImmutableArray<Ticket> tickets)
    {
        try
        {
            var ticketQuestions = new Dictionary<Ticket, List<Question>>();
            var questions = new List<Question>();

            foreach (var questionKey in data.Keys())
            {
                var question = data[questionKey].As<PyDict>();
                var prerequisites = PyObjectDecoder.DecodeToDictionary<string, bool>(question["Prerequisites"].As<PyDict>());
                var askedTicketData = PyObjectDecoder.DecodeToArray<string>(question["AskedTickets"].As<PyList>());
                var askedTickets = askedTicketData.Select(askedTicket => NamedObject.GetObject(askedTicket, tickets) as Ticket).ToList();
                var options = CreateOptionsList(question["Options"].As<PyDict>(), issues, candidates, states);
            
                questions.Add(new Question(questionKey.As<string>(), prerequisites.ToImmutableDictionary(), 
                    askedTickets.ToImmutableArray(), options, question["Randomize"].As<bool>()));
            }

            foreach (var ticket in tickets)
            {
                ticketQuestions.Add(ticket, (from question in questions
                    where question.AskedTickets.Contains(ticket)
                    select question).ToList());
            }

            return ticketQuestions.ToImmutableDictionary();
        }

        catch (OptionSerializationException)
        {
            throw new OptionSerializationException();
        }

        catch (Exception)
        {
            throw new QuestionSerializationException();
        }
    }

    private static ImmutableArray<Option> CreateOptionsList(PyDict data, ImmutableArray<Issue> issues, 
        ImmutableArray<Candidate> candidates, ImmutableArray<State> states)
    {
        try
        {
            var options = new List<Option>();

            foreach (var optionsKey in data.Keys())
            {
                var option = data[optionsKey].As<PyDict>();
                var candidateEffectsData = option["CandidateEffects"].As<PyDict>();
                var stateEffectsData = option["StateEffects"].As<PyDict>();
                var issueEffectsData = PyObjectDecoder.DecodeToArray<double>(option["IssueEffects"].As<PyList>());
            
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

                var gameEffects = PyObjectDecoder.DecodeToDictionary<string, bool>(option["GameEffects"].As<PyDict>());
            
                options.Add(new Option(optionsKey.As<string>(), candidateEffects.ToImmutableDictionary(), 
                    stateEffects.ToImmutableDictionary(), issueEffects.ToImmutableDictionary(), 
                    gameEffects.ToImmutableDictionary(), option["Response"].As<string>()));
            }

            return options.ToImmutableArray();
        }
        catch (Exception)
        {
            throw new OptionSerializationException();
        }
    }
}