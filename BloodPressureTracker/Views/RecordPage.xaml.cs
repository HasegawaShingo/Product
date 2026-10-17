namespace BloodPressureTracker.Views;

using System;
using BloodPressureTracker.ViewModels;
using Microsoft.Maui.Controls;

[QueryProperty(nameof(RecordId), "RecordId")]
[QueryProperty(nameof(Date), "date")]
[QueryProperty(nameof(NewRecord), "new")]
public partial class RecordPage : ContentPage
{
    private bool _hasLoaded = false;

    public string RecordId 
    { 
        get => _recordId; 
        set 
        { 
            _recordId = value; 
            System.Diagnostics.Debug.WriteLine($"RecordPage: QueryProperty RecordId set to '{value}'");
            TryLoadAsync();
        } 
    }
    private string _recordId = string.Empty;

    public string Date 
    { 
        get => _date; 
        set 
        { 
            _date = value; 
            System.Diagnostics.Debug.WriteLine($"RecordPage: QueryProperty Date set to '{value}'");
            TryLoadAsync();
        } 
    }
    private string _date = string.Empty;

    public string NewRecord 
    { 
        get => _newRecord; 
        set 
        { 
            _newRecord = value; 
            System.Diagnostics.Debug.WriteLine($"RecordPage: QueryProperty NewRecord set to '{value}'");
            TryLoadAsync();
        } 
    }
    private string _newRecord = string.Empty;

    public RecordPage(RecordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        System.Diagnostics.Debug.WriteLine($"RecordPage: Constructor");
    }

    void TryLoadAsync()
    {
        if (!_hasLoaded && BindingContext is RecordViewModel vm)
        {
            _hasLoaded = true;
            System.Diagnostics.Debug.WriteLine($"RecordPage: TryLoadAsync - RecordId='{RecordId}', Date='{Date}', NewRecord='{NewRecord}'");
            _ = vm.LoadAsync(RecordId, Date, NewRecord);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine($"RecordPage: OnAppearing - RecordId='{RecordId}', Date='{Date}', NewRecord='{NewRecord}'");
        // Load is now triggered by QueryProperty setters
        // OnAppearing is kept for any UI initialization if needed
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        System.Diagnostics.Debug.WriteLine($"RecordPage: OnNavigatedTo");

        // Preference fallback for "new record" (when not using query params)
        try
        {
            if (Microsoft.Maui.Storage.Preferences.ContainsKey("RecordNew") && Microsoft.Maui.Storage.Preferences.Get("RecordNew", false))
            {
                if (BindingContext is RecordViewModel vm)
                {
                    System.Diagnostics.Debug.WriteLine($"RecordPage.OnNavigatedTo: RecordNew preference found");
                    vm.InitializeForNew(DateTime.Now);
                    Microsoft.Maui.Storage.Preferences.Remove("RecordNew");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RecordPage.OnNavigatedTo: Preferences check failed: {ex}");
        }
    }

    // Focused handlers: open TimePicker with rounded current time as default
    void MorningTimePicker_Focused(object sender, FocusEventArgs e)
    {
        if (sender is TimePicker tp)
        {
            tp.Time = RoundToNearestMinutes(DateTime.Now, 15).TimeOfDay;
        }
    }

    void EveningTimePicker_Focused(object sender, FocusEventArgs e)
    {
        if (sender is TimePicker tp)
        {
            tp.Time = RoundToNearestMinutes(DateTime.Now, 15).TimeOfDay;
        }
    }

    static DateTime RoundToNearestMinutes(DateTime dt, int minutes)
    {
        if (minutes <= 0) return dt;
        var ticks = TimeSpan.TicksPerMinute * minutes;
        var roundedTicks = ((dt.Ticks + ticks / 2) / ticks) * ticks;
        return new DateTime(roundedTicks, dt.Kind);
    }
}