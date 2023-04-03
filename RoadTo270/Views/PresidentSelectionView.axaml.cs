using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RoadTo270.Views;

public partial class CandidateSelectionView : UserControl
{
    public CandidateSelectionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}