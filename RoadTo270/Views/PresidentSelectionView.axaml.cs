﻿using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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

            context.CandidateImage = selectedCandidate.ImagePath;
            context.CandidateName = selectedCandidate.Name;
            context.CandidateParty = selectedCandidate.Affiliation.Name;
            context.CandidateHomeState = selectedCandidate.HomeState.Name;
            context.CandidateDescription = selectedCandidate.Description;
        }
    }
}