using System.Collections.Generic;

namespace RoadTo270.Models;

public class Option
{
    public readonly string Text;
    public readonly Dictionary<Candidate, double> CandidateEffects;
    public readonly Dictionary<State, double> StateEffects;
    public readonly Dictionary<Issue, double> IssueEffects;

    public Option(string text)
    {
        Text = text;
        CandidateEffects = new Dictionary<Candidate, double>();
        StateEffects = new Dictionary<State, double>();
        IssueEffects = new Dictionary<Issue, double>();
    }
}