using RoadTo270.Models.Interfaces;

namespace RoadTo270.ViewModels;

public class MainMenuViewModel: ViewModelBase, IAccessApplication
{
    public App GameApp { get; }

    public MainMenuViewModel(App gameApp)
    {
        GameApp = gameApp;
    }
}