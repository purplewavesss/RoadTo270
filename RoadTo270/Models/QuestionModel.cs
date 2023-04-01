using System.Collections.Immutable;

namespace RoadTo270.Models;

public class Question
{
    public readonly string Text;
    public readonly ImmutableArray<Ticket> AskedTickets;
    public readonly ImmutableArray<Option> Options;
    public readonly bool Randomize;

    public Question(string text, ImmutableArray<Ticket> askedTickets, ImmutableArray<Option> options, bool randomize)
    {
        Text = text;
        AskedTickets = askedTickets;
        Options = options;
        Randomize = randomize;
    }
}