using ReactiveUI;
using RoadTo270.Models;
using RoadTo270.Models.Interfaces;
using RoadTo270.Views;

namespace RoadTo270.ViewModels;

public class MainWindowViewModel : ViewModelBase, IAccessApplication
{
    public App GameApp { get; }
    public Scenario Game { get; set; }
    
    public ViewModelBase Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public readonly MapView MainWindowMapView;
    public readonly MainMenuViewModel MainWindowMainMenuViewModel;
    public readonly CreditsViewModel MainWindowCreditsViewModel;
    private ViewModelBase _content;

    public MainWindowViewModel(App gameApp)
    {
        GameApp = gameApp;
        MainWindowMainMenuViewModel = new MainMenuViewModel(GameApp);
        Content = MainWindowMainMenuViewModel;
        MainWindowMapView = new MapView();
        MainWindowCreditsViewModel = new CreditsViewModel(GameApp, MainWindowMainMenuViewModel);
    }
}