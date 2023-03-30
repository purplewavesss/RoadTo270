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
        List<Party> parties = new List<Party>();

        PythonEngine.Initialize();

        using (Py.GIL())
        {
            dynamic json = Py.Import("json");
            PyDict jsonContents = json.loads(File.ReadAllText("../../../../RoadTo270/test.json"));

            parties = Methods.CreatePartyList(jsonContents["Parties"].As<PyDict>());
        }
        
        PythonEngine.Shutdown();
        
        Assert.That(parties.Count is 1, Is.True);
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
}

public static class Methods
{
    public static List<Party> CreatePartyList(PyDict data)
    {
        List<Party> parties = new List<Party>();
        
        foreach (var partyObj in data.Values())
        {
            var party = partyObj.As<PyDict>();
            var colors = PyListDecoder.Decode<int>(party["Color"].As<PyList>());

            Tuple<int, int, int> colorValues = new Tuple<int, int, int>(colors[0], colors[1], colors[2]);
                
            parties.Add(new Party(party["Name"].As<string>(), colorValues));
        }

        return parties;
    }
    
    public static ImmutableArray<Issue> CreateIssuesList(PyDict data)
    {
        Issue[] issues = new Issue[data.Length()];

        foreach (var issueObj in data.Values())
        {
            var issue = issueObj.As<PyDict>();
            var positions = PyListDecoder.Decode<string>(issue["Positions"].As<PyList>()).ToImmutableArray();
            var constraints = PyListDecoder.Decode<int>(issue["Constraints"].As<PyList>()).ToImmutableArray();

            issues[issue["Index"].As<int>()] = new Issue(issue["Name"].As<String>(), positions, constraints);
        }

        return issues.ToImmutableArray();
    }
}