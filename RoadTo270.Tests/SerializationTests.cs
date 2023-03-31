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
        statesList.Add(new ("Utah", new [] {19, 30, 10, 70, 50}, 32));
        statesList.Add(new ("NorthDakota", new [] {19, 30, 10, 70, 51}, 34));
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
}

public static class Methods
{
    public static ImmutableArray<Party> CreatePartyList(PyDict data)
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
    
    public static ImmutableArray<Issue> CreateIssuesList(PyDict data)
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
    
    public static ImmutableArray<Candidate> CreateCandidatesList(PyDict data, ImmutableArray<Party> parties, 
        ImmutableArray<State> states)
    {
        List<Candidate> candidates = new List<Candidate>();

        foreach (var candidateObj in data.Values())
        {
            var candidate = candidateObj.As<PyDict>();
            var affiliation = (from party in parties
                where party.Name == candidate["Affiliation"].As<string>()
                select party).ToList()[0];
            var homeState = (from state in states
                where state.Name == candidate["HomeState"].As<string>()
                select state).ToList()[0];
            var issueScores = PyObjectDecoder.DecodeToArray<int>(candidate["IssueScores"].As<PyList>());
            var stateModifiers = PyObjectDecoder.DecodeToArray<double>(candidate["StateModifiers"].As<PyList>()).ToImmutableArray();
            candidates.Add(new Candidate(candidate["Name"].As<string>(), candidate["Description"].As<string>(),
                candidate["ImagePath"].As<string>(), candidate["AdvisorImagePath"].As<string>(),
                affiliation, homeState, issueScores, stateModifiers, candidate["IsRunningMate"].As<bool>()));
        }

        return candidates.ToImmutableArray();
    }
}