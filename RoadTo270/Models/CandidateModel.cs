using System.Collections.Immutable;
using System.Linq;

namespace RoadTo270.Models;

public class Candidate
{
    public readonly string Name;
    public readonly string Description;
    public readonly string ImagePath;
    public readonly string AdvisorImagePath;
    public readonly Party Affiliation;
    public readonly State HomeState;
    public readonly ImmutableArray<int> IssueScores;
    public readonly ImmutableArray<double> StateModifiers;
    public readonly bool IsRunningMate;
    public readonly double AggregateScore;

    public Candidate(string name, string description, string imagePath, Party affiliation, State homeState, ImmutableArray<int> issueScores, 
        ImmutableArray<double> stateModifiers, bool isRunningMate, string advisorImagePath)
    {
        Name = name;
        Description = description;
        ImagePath = imagePath;
        Affiliation = affiliation;
        HomeState = homeState;
        IssueScores = issueScores;
        StateModifiers = stateModifiers;
        IsRunningMate = isRunningMate;
        AdvisorImagePath = advisorImagePath;
        AggregateScore = IssueScores.Average();
    }
}