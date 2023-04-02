using RoadTo270.Models.Interfaces;

namespace RoadTo270.ViewModels;

public class CreditsViewModel : ViewModelBase, IAccessApplication
{
    public App GameApp { get; }
    public readonly MainMenuViewModel PreviousView;

    public CreditsViewModel(App gameApp, MainMenuViewModel previousView)
    {
        GameApp = gameApp;
        PreviousView = previousView;
    }
}