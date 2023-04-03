using Avalonia.Controls.ApplicationLifetimes;
using RoadTo270.Models.Interfaces;
using RoadTo270.Views;

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
    
    public static MainWindow GetMainWindow(IAccessApplication viewModel)
    {
        var window = viewModel.GameApp.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return (window!.MainWindow as MainWindow)!;
    }

    public static string GetFolder(string file)
    {
        var pathComponents = file.Split("/");
        string folder = "";

        for (int pathIndex = 0; pathIndex < pathComponents.Length - 1; pathIndex++) folder += pathComponents[pathIndex] + "/";

        return folder;
    }
}