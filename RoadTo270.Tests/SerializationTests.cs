using System.Collections.Immutable;
using Python.Runtime;
using RoadTo270.Codecs;
using RoadTo270.Models;

namespace RoadTo270.Tests;

[TestFixture]
public class SerializationTests
{
    [SetUp]
    public void Setup()
    {
        Runtime.PythonDLL = "C:/Users/gavin/AppData/Local/Programs/Python/Python311/python311.dll";
    }

    [Test, Order(0)]
    public void SerializePartyTest()
    {
        ImmutableArray<Party> parties = new ImmutableArray<Party>();

        PythonEngine.Initialize();

        using (Py.GIL())
        {
            dynamic json = Py.Import("json");
            PyDict jsonContents = json.loads(File.ReadAllText("../../../../RoadTo270/test.json"));

            parties = Methods.CreatePartyList(jsonContents["Parties"].As<PyDict>());
        }
        
        PythonEngine.Shutdown();
        
        Assert.That(parties.Length is 1, Is.True);
    }
    
    [Test, Order(1)]
    public void SerializeIssuesTest()
    {
        ImmutableArray<Issue> issues = new ImmutableArray<Issue>();

        PythonEngine.Initialize();
        
        using (Py.GIL())
        {
            dynamic json = Py.Import("json");
            PyDict jsonContents = json.loads(File.ReadAllText("../../../../RoadTo270/test.json"));

            issues = Methods.CreateIssuesList(jsonContents["Issues"].As<PyDict>());
        }
        
        PythonEngine.Shutdown();
        
        Assert.That(issues.Length is 4, Is.True);
    }
    
    [Test, Order(2)]
    public void SerializeCandidatesTest()
    {
        ImmutableArray<Party> parties = new ImmutableArray<Party>();
        List<State> statesList = new List<State>();
        statesList.Add(new ("Utah", new [] {19, 30, 10, 70, 50}, 6, 32));
        statesList.Add(new ("NorthDakota", new [] {19, 30, 10, 70, 51}, 3, 34));
        var states = statesList.ToImmutableArray();
        ImmutableArray<Candidate> candidates = new ImmutableArray<Candidate>();

        PythonEngine.Initialize();
        
        using (Py.GIL())
        {
            dynamic json = Py.Import("json");
            PyDict jsonContents = json.loads(File.ReadAllText("../../../../RoadTo270/test.json"));
            parties = Methods.CreatePartyList(jsonContents["Parties"].As<PyDict>());
            candidates = Methods.CreateCandidatesList(jsonContents["Candidates"].As<PyDict>(), parties, states);
        }
        
        PythonEngine.Shutdown();
        
        Assert.That(candidates.Length is 2, Is.True);
    }
    
    [Test, Order(3)]
    public void SerializeTicketsTest()
    {
        ImmutableArray<Party> parties = new ImmutableArray<Party>();
        List<State> statesList = new List<State>();
        statesList.Add(new ("Utah", new [] {19, 30, 10, 70, 50}, 6, 32));
        statesList.Add(new ("NorthDakota", new [] {19, 30, 10, 70, 51}, 3, 34));
        var states = statesList.ToImmutableArray();
        ImmutableArray<Candidate> candidates = new ImmutableArray<Candidate>();
        ImmutableArray<Ticket> tickets = new ImmutableArray<Ticket>();

        PythonEngine.Initialize();
        
        using (Py.GIL())
        {
            dynamic json = Py.Import("json");
            PyDict jsonContents = json.loads(File.ReadAllText("../../../../RoadTo270/test.json"));
            parties = Methods.CreatePartyList(jsonContents["Parties"].As<PyDict>());
            candidates = Methods.CreateCandidatesList(jsonContents["Candidates"].As<PyDict>(), parties, states);
            tickets = Methods.CreateTicketsList(jsonContents["Tickets"].As<PyDict>(), parties, candidates);
        }
        
        PythonEngine.Shutdown();
        
        Assert.That(tickets.Length is 1, Is.True);
    }

    [Test, Order(4)]
    public void SerializeQuestionsTest()
    {
        ImmutableArray<Party> parties = new ImmutableArray<Party>();
        ImmutableArray<Issue> issues = new ImmutableArray<Issue>();
        List<State> statesList = new List<State>();
        statesList.Add(new ("Utah", new [] {19, 30, 10, 70, 50}, 6, 32));
        statesList.Add(new ("NorthDakota", new [] {19, 30, 10, 70, 51}, 3, 34));
        var states = statesList.ToImmutableArray();
        ImmutableArray<Candidate> candidates = new ImmutableArray<Candidate>();
        ImmutableArray<Ticket> tickets = new ImmutableArray<Ticket>();
        ImmutableDictionary<Ticket, List<Question>> candidateQuestions =
            new Dictionary<Ticket, List<Question>>().ToImmutableDictionary();

        PythonEngine.Initialize();
        
        using (Py.GIL())
        {
            dynamic json = Py.Import("json");
            PyDict jsonContents = json.loads(File.ReadAllText("../../../../RoadTo270/test.json"));
            parties = Methods.CreatePartyList(jsonContents["Parties"].As<PyDict>());
            issues = Methods.CreateIssuesList(jsonContents["Issues"].As<PyDict>());
            candidates = Methods.CreateCandidatesList(jsonContents["Candidates"].As<PyDict>(), parties, states);
            tickets = Methods.CreateTicketsList(jsonContents["Tickets"].As<PyDict>(), parties, candidates);
            candidateQuestions = Methods.CreateQuestionsList(jsonContents["Questions"].As<PyDict>(), 
                issues, states, candidates, tickets);
        }
        
        PythonEngine.Shutdown();
        
        Assert.That(candidateQuestions.Count is 1, Is.True);
    }
}

public static class Methods
{
    public static ImmutableArray<Party> CreatePartyList(PyDict data)
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
    
    public static ImmutableArray<Issue> CreateIssuesList(PyDict data)
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
    
    public static ImmutableArray<Candidate> CreateCandidatesList(PyDict data, ImmutableArray<Party> parties, 
        ImmutableArray<State> states)
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
    
    public static ImmutableArray<Ticket> CreateTicketsList(PyDict data, ImmutableArray<Party> parties, ImmutableArray<Candidate> candidates)
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
    
    public static ImmutableDictionary<Ticket, List<Question>> CreateQuestionsList(PyDict data, ImmutableArray<Issue> issues,
        ImmutableArray<State> states, ImmutableArray<Candidate> candidates, ImmutableArray<Ticket> tickets)
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

    private static ImmutableArray<Option> CreateOptionsList(PyDict data, ImmutableArray<Issue> issues, 
        ImmutableArray<Candidate> candidates, ImmutableArray<State> states)
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
}