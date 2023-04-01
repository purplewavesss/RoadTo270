using System.Collections.Generic;
using System.Collections.Immutable;

namespace RoadTo270.Models;

public class Scenario
{
    public readonly ImmutableArray<Party> Parties;
    public readonly ImmutableArray<Issue> Issues;
    public readonly ImmutableArray<State> States;
    public readonly ImmutableArray<Candidate> Candidates;
    public readonly ImmutableArray<Ticket> Tickets;
    public readonly ImmutableDictionary<Ticket, List<Question>> CandidateQuestions;
    public int CurrentQuestion { get; set; }

    public Scenario(ImmutableArray<Party> parties, ImmutableArray<Issue> issues, ImmutableArray<State> states, 
        ImmutableArray<Candidate> candidates, ImmutableArray<Ticket> tickets, ImmutableDictionary<Ticket, 
            List<Question>> candidateQuestions, int currentQuestion)
    {
        Parties = parties;
        Issues = issues;
        States = states;
        Candidates = candidates;
        Tickets = tickets;
        CandidateQuestions = candidateQuestions;
        CurrentQuestion = currentQuestion;
    }
}