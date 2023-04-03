using System.Collections.Immutable;

namespace RoadTo270.Models;

public class Question
{
    public readonly string Text;
    public readonly ImmutableDictionary<string, bool> Prerequisites;
    public readonly ImmutableArray<Ticket> AskedTickets;
    public readonly ImmutableArray<Option> Options;
    public readonly bool Randomize;

    public Question(string text, ImmutableDictionary<string, bool> prerequisites, ImmutableArray<Ticket> askedTickets, 
        ImmutableArray<Option> options, bool randomize)
    {
        Text = text;
        Prerequisites = prerequisites;
        AskedTickets = askedTickets;
        Options = options;
        Randomize = randomize;
    }
}