using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Shapes;

namespace RoadTo270.Models;

public class State: NamedObject
{
    public readonly double AggregateScore;
    public readonly int[] IssueScores;
    public readonly int ElectoralVotes;
    public readonly int Votes;
    public readonly Path? MapState;
    public readonly Dictionary<string, double> Support;

    public State(string name, int[] issueScores, int electoralVotes, int votes, Path? mapState = null) : base(name)
    {
        IssueScores = issueScores;
        MapState = mapState;
        ElectoralVotes = electoralVotes;
        Votes = votes;
        Support = new Dictionary<string, double>();
        AggregateScore = IssueScores.Average();
    }
}