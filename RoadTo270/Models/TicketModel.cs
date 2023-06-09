﻿using System;

namespace RoadTo270.Models;

public class Ticket: NamedObject
{
    public readonly Candidate President;
    public readonly Candidate VicePresident;
    public readonly Party Affiliation;
    public readonly double[] IssueScores;
    public readonly double[] StateModifiers;
    public readonly bool Playable;

    public Ticket(string name, Candidate president, Candidate vicePresident, Party affiliation, bool playable) : base(name)
    {
        President = president;
        VicePresident = vicePresident;
        Affiliation = affiliation;
        Playable = playable;

        if (President.HomeState == VicePresident.HomeState) throw new InvalidOperationException("Two candidates " +
            "can not have the same home state!");
        
        IssueScores = new double[President.IssueScores.Length];
        StateModifiers = new double[President.StateModifiers.Count];

        for (var scoreIndex = 0; scoreIndex < President.IssueScores.Length; scoreIndex++)
        {
            IssueScores[scoreIndex] = President.IssueScores[scoreIndex] * 0.9 + VicePresident.IssueScores[scoreIndex] * 0.1;
        }
    }
}