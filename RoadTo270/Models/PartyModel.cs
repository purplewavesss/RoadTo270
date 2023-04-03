using System;

namespace RoadTo270.Models;

public class Party: NamedObject
{
    public readonly Tuple<int, int, int> Color;
    public readonly string Position;

    public Party(string name, Tuple<int, int, int> color, string position) : base(name)
    {
        Color = color;
        Position = position;
    }
}