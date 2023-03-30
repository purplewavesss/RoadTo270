using ReactiveUI;

namespace RoadTo270.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public App GameApp { get; }
    
    public ViewModelBase Content
    {
        get => content;
        private set => this.RaiseAndSetIfChanged(ref content, value);
    }
    
    private ViewModelBase content;

    public MainWindowViewModel(App gameApp)
    {
        GameApp = gameApp;
        Content = new MainMenuViewModel(GameApp);
    }
}