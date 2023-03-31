using System.Collections.Immutable;
using System.Linq;

namespace RoadTo270.Models;

public class Candidate: NamedObject
{
    public readonly string Description;
    public readonly string ImagePath;
    public readonly string AdvisorImagePath;
    public readonly Party Affiliation;
    public readonly State HomeState;
    public readonly int[] IssueScores;
    public readonly ImmutableArray<double> StateModifiers;
    public readonly bool IsRunningMate;
    public readonly double AggregateScore;

    public Candidate(string name, string description, string imagePath, string advisorImagePath, Party affiliation, State homeState, int[] issueScores, 
        ImmutableArray<double> stateModifiers, bool isRunningMate) : base(name)
    {
        Description = description;
        ImagePath = imagePath;
        AdvisorImagePath = advisorImagePath;
        Affiliation = affiliation;
        HomeState = homeState;
        IssueScores = issueScores;
        StateModifiers = stateModifiers;
        IsRunningMate = isRunningMate;
        AggregateScore = IssueScores.Average();
    }
}