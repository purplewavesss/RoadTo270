using System.Collections.Generic;

namespace RoadTo270.Models;

public class Game
{
    public readonly List<Party> Parties;
    public readonly List<Issue> Issues;
    public readonly List<State> States;
    public readonly List<Candidate> Candidates;
    public readonly List<List<Question>> CandidateQuestions;

    public Game(List<Party> parties, List<Issue> issues, List<State> states, List<Candidate> candidates, List<List<Question>> candidateQuestions)
    {
        Parties = parties;
        Issues = issues;
        States = states;
        Candidates = candidates;
        CandidateQuestions = candidateQuestions;
    }
}