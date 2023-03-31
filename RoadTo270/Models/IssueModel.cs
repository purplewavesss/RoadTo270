using System;
using System.Collections.Immutable;

namespace RoadTo270.Models;

public class Issue: NamedObject
{
    public readonly ImmutableArray<string> Positions;
    public readonly ImmutableArray<int> Constraints;

    public Issue(string name, ImmutableArray<string> positions, ImmutableArray<int> constraints) : base(name)
    {
        Positions = positions;
        Constraints = constraints;
    }
}