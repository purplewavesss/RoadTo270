using System.Collections.Immutable;

namespace RoadTo270.Models;

public class Option
{
    public readonly string Text;
    public readonly ImmutableDictionary<Candidate, double> CandidateEffects;
    public readonly ImmutableDictionary<State, double> StateEffects;
    public readonly ImmutableDictionary<Issue, double> IssueEffects;
    public readonly ImmutableDictionary<string, bool> GameEffects;
    public readonly string Response;

    public Option(string text, ImmutableDictionary<Candidate, double> candidateEffects, 
        ImmutableDictionary<State, double> stateEffects, ImmutableDictionary<Issue, double> issueEffects, 
        ImmutableDictionary<string, bool> gameEffects, string response)
    {
        Text = text;
        CandidateEffects = candidateEffects;
        StateEffects = stateEffects;
        IssueEffects = issueEffects;
        GameEffects = gameEffects;
        Response = response;
    }
}