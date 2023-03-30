using System.Collections.Generic;
using System.Collections.Immutable;

namespace RoadTo270.Models;

public class Game
{
    public readonly List<Party> Parties;
    public readonly ImmutableArray<Issue> Issues;
    public readonly List<State> States;
    public readonly List<Candidate> Candidates;
    public readonly Dictionary<string, List<Question>> CandidateQuestions;

    public Game(List<Party> parties, ImmutableArray<Issue> issues, List<State> states, List<Candidate> candidates, 
        Dictionary<string, List<Question>> candidateQuestions)
    {
        Parties = parties;
        Issues = issues;
        States = states;
        Candidates = candidates;
        CandidateQuestions = candidateQuestions;
    }
}