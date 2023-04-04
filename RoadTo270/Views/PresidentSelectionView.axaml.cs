using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using RoadTo270.Models;
using RoadTo270.ViewModels;

namespace RoadTo270.Views;

public partial class PresidentSelectionView : UserControl
{
    private bool SelectBoxInitialized { get; set; }

    public PresidentSelectionView()
    {
        SelectBoxInitialized = false;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void Back(object? sender, RoutedEventArgs e)
    {
        SelectBoxInitialized = false;
        var context = DataContext as PresidentSelectionViewModel;
        var windowContext = Functions.GetMainWindow(context).DataContext as MainWindowViewModel;
        windowContext.Content = context.PreviousView;
    }
    
    private void CandidateSelectBoxInitialized(object? sender, EventArgs e)
    {
        var context = DataContext as PresidentSelectionViewModel;
        CandidateSelectBox = sender as ComboBox;
        
        SelectBoxInitialized = true;
        CandidateSelectBox.SelectedItem = context.PlayableCandidates.Keys.ToList()[0];
    }

    private void CandidateSelectionChange(object? sender, SelectionChangedEventArgs e)
    {
        if (SelectBoxInitialized)
        {
            var context = DataContext as PresidentSelectionViewModel;

            Candidate selectedCandidate = context.PlayableCandidates[CandidateSelectBox.SelectedItem as string];

            context.CandidateImage = new Bitmap(selectedCandidate.ImagePath);
            context.CandidateName = selectedCandidate.Name;
            context.CandidateParty = selectedCandidate.Affiliation.Name;
            context.CandidateHomeState = selectedCandidate.HomeState.Name;
            context.CandidateDescription = selectedCandidate.Description;
        }
    }
}