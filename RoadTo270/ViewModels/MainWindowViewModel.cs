using ReactiveUI;
using RoadTo270.Models;
using RoadTo270.Views;

namespace RoadTo270.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public App GameApp { get; }
    public Scenario Game { get; set; }
    
    public ViewModelBase Content
    {
        get => content;
        private set => this.RaiseAndSetIfChanged(ref content, value);
    }
    
    public readonly MapView MainWindowMapView;
    
    private ViewModelBase content;

    public MainWindowViewModel(App gameApp)
    {
        GameApp = gameApp;
        Content = new MainMenuViewModel(GameApp);
        MainWindowMapView = new MapView();
    }
}