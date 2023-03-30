namespace RoadTo270.ViewModels;

public class MainMenuViewModel: ViewModelBase
{
    public App GameApp { get; }

    public MainMenuViewModel(App gameApp)
    {
        GameApp = gameApp;
    }
}