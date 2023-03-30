using System;

namespace RoadTo270.Models;

public class Ticket
{
    public readonly Candidate President;
    public readonly Candidate VicePresident;
    public readonly Party Affiliation;
    public readonly double[] IssueScores;
    public readonly double[] StateModifiers;

    public Ticket(Candidate president, Candidate vicePresident, Party affiliation)
    {
        President = president;
        VicePresident = vicePresident;
        Affiliation = affiliation;

        if (President.HomeState == VicePresident.HomeState) throw new InvalidOperationException("Two candidates " +
            "can not have the same home state!");
        
        IssueScores = new double[3];
        StateModifiers = new double[3];

        for (var scoreIndex = 0; scoreIndex < President.IssueScores.Length; scoreIndex++)
        {
            IssueScores[scoreIndex] = President.IssueScores[scoreIndex] * 0.9 + VicePresident.IssueScores[scoreIndex] * 0.1;
        }
    }
}