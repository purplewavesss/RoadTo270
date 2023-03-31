using System;

namespace RoadTo270.Models;

public class Party: NamedObject
{
    public readonly Tuple<int, int, int> Color;

    public Party(string name, Tuple<int, int, int> color) : base(name)
    {
        Color = color;
    }
}