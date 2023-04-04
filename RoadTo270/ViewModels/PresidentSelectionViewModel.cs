using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ReactiveUI;
using RoadTo270.Models;

namespace RoadTo270.ViewModels;

public class PresidentSelectionViewModel: ViewModelBase
{
    public App GameApp { get; }
    
    public string CandidateImage 
    {
        get => _candidateImage;
        set => this.RaiseAndSetIfChanged(ref _candidateImage, value);
    }
    
    public string CandidateName 
    {
        get => _candidateName;
        set => this.RaiseAndSetIfChanged(ref _candidateName, value);
    }
    
    public string CandidateParty 
    {
        get => _candidateParty;
        set => this.RaiseAndSetIfChanged(ref _candidateParty, value);
    }
    
    public string CandidateHomeState 
    {
        get => _candidateHomeState;
        set => this.RaiseAndSetIfChanged(ref _candidateHomeState, value);
    }
    
    public string CandidateDescription 
    {
        get => _candidateDescription;
        set => this.RaiseAndSetIfChanged(ref _candidateDescription, value);
    }
    
    public Dictionary<string, Candidate> PlayableCandidates { get; }
    public Dictionary<Candidate, List<Candidate>> VicePresidents { get; }
    public readonly Scenario Game;
    public readonly ImmutableArray<Ticket> Tickets;
    public readonly MainMenuViewModel PreviousView;
    private string _candidateName;
    private string _candidateParty;
    private string _candidateHomeState;
    private string _candidateDescription;
    private string _candidateImage;

    public PresidentSelectionViewModel(App gameApp, Scenario game, MainMenuViewModel previousView)
    {
        GameApp = gameApp;
        Game = game;
        PlayableCandidates = new Dictionary<string, Candidate>();
        VicePresidents = new Dictionary<Candidate, List<Candidate>>();
        Tickets = Game.Tickets;
        PreviousView = previousView;

        foreach (var ticket in Tickets)
        {
            if (!PlayableCandidates.ContainsKey(ticket.President.Name)) 
                PlayableCandidates.Add(ticket.President.Name, ticket.President);
            
            if (!VicePresidents.ContainsKey(ticket.President)) 
                VicePresidents.Add(ticket.President, new List<Candidate>());
            
            VicePresidents[ticket.President].Add(ticket.VicePresident);
        }

        Candidate firstCandidate = PlayableCandidates[PlayableCandidates.Keys.ToList()[0]];
        
        CandidateImage = firstCandidate.ImagePath;
        CandidateName = firstCandidate.Name;
        CandidateParty = firstCandidate.Affiliation.Name;
        CandidateHomeState = firstCandidate.HomeState.Name;
        CandidateDescription = firstCandidate.Description;
    }
}