using System;
using System.Collections.Immutable;

namespace RoadTo270.Models;

public class Issue
{
    public readonly string Name;
    public readonly byte Index;
    public readonly ImmutableArray<string> Positions;
    public readonly ImmutableArray<int> Constraints;

    public Issue(string name, byte index, ImmutableArray<string> positions, ImmutableArray<int> constraints)
    {
        Name = name;
        Index = index;

        if (index > 3) throw new IndexOutOfRangeException("Issue index does not exist in range!");
        
        Positions = positions;
        Constraints = constraints;
    }
}