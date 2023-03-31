using System.Collections.Generic;
using System.Collections.Immutable;

namespace RoadTo270.Models;

public class Game
{
    public readonly ImmutableArray<Party> Parties;
    public readonly ImmutableArray<Issue> Issues;
    public readonly ImmutableArray<State> States;
    public readonly ImmutableArray<Candidate> Candidates;
    public readonly ImmutableDictionary<string, List<Question>> CandidateQuestions;

    public Game(ImmutableArray<Party> parties, ImmutableArray<Issue> issues, ImmutableArray<State> states, 
        ImmutableArray<Candidate> candidates, ImmutableDictionary<string, List<Question>> candidateQuestions)
    {
        Parties = parties;
        Issues = issues;
        States = states;
        Candidates = candidates;
        CandidateQuestions = candidateQuestions;
    }
}