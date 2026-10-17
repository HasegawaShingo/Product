namespace BloodPressureTracker.Views;

using BloodPressureTracker.ViewModels;
using BloodPressureTracker.Models;
using System;

public partial class ListPage : ContentPage
{
    public ListPage(ListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadRecordsAsync();
    }

    private ListViewModel viewModel => (BindingContext as ListViewModel)!;

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0) return;
        if (e.CurrentSelection[0] is not DailyRecordSummary item) return;

        // Clear selection to allow re-select
        ((CollectionView)sender).SelectedItem = null;

        // Call VM command
        if (viewModel.OpenDayCommand.CanExecute(item))
        {
            viewModel.OpenDayCommand.Execute(item);
        }
    }

    private void OnCardTapped(object sender, EventArgs e)
    {
        if (sender is not VisualElement ve) return;
        if (ve.BindingContext is not DailyRecordSummary item) return;

        if (viewModel.OpenDayCommand.CanExecute(item))
        {
            viewModel.OpenDayCommand.Execute(item);
        }
    }
}
