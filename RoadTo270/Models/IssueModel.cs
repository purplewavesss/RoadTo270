using System;
using System.Collections.Immutable;

namespace RoadTo270.Models;

public class Issue
{
    public readonly string Name;
    public readonly ImmutableArray<string> Positions;
    public readonly ImmutableArray<int> Constraints;

    public Issue(string name, ImmutableArray<string> positions, ImmutableArray<int> constraints)
    {
        Name = name;
        Positions = positions;
        Constraints = constraints;
    }
}