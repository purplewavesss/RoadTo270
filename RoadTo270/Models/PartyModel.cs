using System;

namespace RoadTo270.Models;

public class Party
{
    public readonly string Name;
    public readonly Tuple<int, int, int> Color;

    public Party(string name, Tuple<int, int, int> color)
    {
        Name = name;
        Color = color;
    }
}