namespace RoadTo270;

public static class Functions
{
    public static string RemoveSpaces(string input)
    {
        string[] words = input.Split(" ");
        string combinedWord = "";

        foreach (var word in words) combinedWord += word;

        return combinedWord;
    }
}