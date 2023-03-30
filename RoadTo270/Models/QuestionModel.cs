using System.Collections.Immutable;

namespace RoadTo270.Models;

public class Question
{
    public readonly string Text;
    public readonly Candidate AskedCandidate;
    public readonly ImmutableArray<Option> Options;
    public readonly bool Randomize;
    public readonly ImmutableArray<string> Responses;

    public Question(string text, Candidate askedCandidate, ImmutableArray<Option> options, bool randomize, 
        ImmutableArray<string> responses)
    {
        Text = text;
        AskedCandidate = askedCandidate;
        Options = options;
        Randomize = randomize;
        Responses = responses;
    }
}