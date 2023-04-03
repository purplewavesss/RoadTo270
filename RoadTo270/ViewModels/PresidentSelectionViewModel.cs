using System.Collections.Generic;
using Avalonia.Controls;
using RoadTo270.Models;

namespace RoadTo270.ViewModels;

public class CandidateSelectionViewModel
{
    public readonly Dictionary<string, Candidate> PlayableCandidates;
    public readonly Dictionary<Candidate, List<Candidate>> VicePresidents;
    private readonly List<Ticket> Tickets;

    public CandidateSelectionViewModel(List<Ticket> tickets)
    {
        PlayableCandidates = new Dictionary<string, Candidate>();
        VicePresidents = new Dictionary<Candidate, List<Candidate>>();
        Tickets = tickets;

        foreach (var ticket in Tickets)
        {
            var presidentialCandidate = ticket.President;
            
            if (!PlayableCandidates.ContainsKey(presidentialCandidate.Name)) 
                PlayableCandidates.Add(presidentialCandidate.Name, presidentialCandidate);
            
            if (!VicePresidents.ContainsKey(presidentialCandidate)) 
                VicePresidents.Add(presidentialCandidate, new List<Candidate>());
            
            VicePresidents[presidentialCandidate].Add(ticket.VicePresident);
        }
    }
}