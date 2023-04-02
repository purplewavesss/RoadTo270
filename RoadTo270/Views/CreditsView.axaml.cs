using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RoadTo270.ViewModels;

namespace RoadTo270.Views;

public partial class CreditsView : UserControl
{
    public CreditsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void Back(object? sender, RoutedEventArgs e)
    {
        var context = Functions.GetMainWindow(DataContext as CreditsViewModel).DataContext as MainWindowViewModel;
        context.Content = context.MainWindowMainMenuViewModel;
    }
}