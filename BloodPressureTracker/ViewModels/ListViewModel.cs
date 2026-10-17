namespace BloodPressureTracker.ViewModels;

using BloodPressureTracker.Models;
using BloodPressureTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public partial class ListViewModel : BaseViewModel, IRecipient<RecordPageLoadedMessage>, IRecipient<RecordPageCancelMessage>
{
    private readonly DatabaseService _databaseService;
    private TaskCompletionSource<bool>? _pageLoadTcs;
    private bool _skipSpinnerOnLoad = false;

    [ObservableProperty]
    ObservableCollection<DailyRecordSummary> records = new();

    // Dedicated property for pull-to-refresh (RefreshView)
    [ObservableProperty]
    bool isRefreshing = false;

    public ListViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        Title = "記録一覧";
        
        // Register for RecordPageLoaded message
        WeakReferenceMessenger.Default.Register<RecordPageLoadedMessage>(this);
        WeakReferenceMessenger.Default.Register<RecordPageCancelMessage>(this);
    }

    [RelayCommand]
    public async Task LoadRecordsAsync()
    {
        // Skip spinner if returning from cancel
        if (_skipSpinnerOnLoad)
        {
            _skipSpinnerOnLoad = false;
            await LoadRecordsInternalAsync();
            return;
        }

        // Only use IsRefreshing for pull-to-refresh; IsBusy is for navigation overlay only
        isRefreshing = true;
        try
        {
            await LoadRecordsInternalAsync();
        }
        finally
        {
            isRefreshing = false;
        }
    }

    private async Task LoadRecordsInternalAsync()
    {
        var list = await _databaseService.GetRecordsAsync();
        records.Clear();

        var grouped = list.GroupBy(r => r.RecordDate.Date).OrderByDescending(g => g.Key);
        foreach (var g in grouped)
        {
            var dayAvgSystolic = g.Select(r => r.AvgSystolic).DefaultIfEmpty(0).Average();
            var dayAvgDiastolic = g.Select(r => r.AvgDiastolic).DefaultIfEmpty(0).Average();
            var dayAvgPulse = g.Select(r => r.AvgPulse).DefaultIfEmpty(0).Average();

            var morning = g.Where(r => r.TimeCategory == TimeCategory.Morning).ToList();
            var evening = g.Where(r => r.TimeCategory == TimeCategory.Evening).ToList();

            var item = new DailyRecordSummary
            {
                RecordDate = g.Key,
                DayAvgSystolic = dayAvgSystolic > 0 ? (int?)Math.Round(dayAvgSystolic) : null,
                DayAvgDiastolic = dayAvgDiastolic > 0 ? (int?)Math.Round(dayAvgDiastolic) : null,
                DayAvgPulse = dayAvgPulse > 0 ? (int?)Math.Round(dayAvgPulse) : null,

                MorningAvgSystolic = morning.Any() ? (int?)Math.Round(morning.Average(r => r.AvgSystolic)) : null,
                MorningAvgDiastolic = morning.Any() ? (int?)Math.Round(morning.Average(r => r.AvgDiastolic)) : null,
                MorningAvgPulse = morning.Any() ? (int?)Math.Round(morning.Average(r => r.AvgPulse)) : null,

                EveningAvgSystolic = evening.Any() ? (int?)Math.Round(evening.Average(r => r.AvgSystolic)) : null,
                EveningAvgDiastolic = evening.Any() ? (int?)Math.Round(evening.Average(r => r.AvgDiastolic)) : null,
                EveningAvgPulse = evening.Any() ? (int?)Math.Round(evening.Average(r => r.AvgPulse)) : null,

                Notes = string.Join("; ", g.Select(r => r.Notes).Where(n => !string.IsNullOrWhiteSpace(n)))
            };

            records.Add(item);
        }
    }

    [RelayCommand]
    async Task AddRecordAsync()
    {
        try
        {
            Microsoft.Maui.Storage.Preferences.Set("RecordNew", true);
        }
        catch { }

        IsBusy = true;
        try
        {
            var tcs = new TaskCompletionSource<bool>();
            _pageLoadTcs = tcs;

            await Shell.Current.GoToAsync("///record?new=true");

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(5000));
            if (completedTask == tcs.Task)
            {
                System.Diagnostics.Debug.WriteLine($"ListViewModel: RecordPage (new) loaded successfully");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"ListViewModel: Timeout waiting for RecordPage (new) to load");
            }
        }
        finally
        {
            _pageLoadTcs = null;
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task OpenDayAsync(DailyRecordSummary item)
    {
        if (item == null) return;

        IsBusy = true;
        try
        {
            var tcs = new TaskCompletionSource<bool>();
            _pageLoadTcs = tcs;

            await Shell.Current.GoToAsync($"///record?date={item.RecordDate:yyyy-MM-dd}");

            // Wait for page to load or timeout (5 seconds)
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(5000));
            if (completedTask == tcs.Task)
            {
                System.Diagnostics.Debug.WriteLine($"ListViewModel: RecordPage loaded successfully");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"ListViewModel: Timeout waiting for RecordPage to load");
            }
        }
        finally
        {
            _pageLoadTcs = null;
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task DeleteRecordAsync(DailyRecordSummary item)
    {
        if (item == null) return;

        // Show confirmation dialog
        var confirm = await Shell.Current.DisplayAlertAsync("削除確認", 
            $"{item.RecordDate:M/d (ddd)} の記録を削除しますか？", 
            "削除", "キャンセル");
        if (!confirm) return;

        try
        {
            // Delete all records for this date (both morning and evening)
            var start = item.RecordDate.Date;
            var end = item.RecordDate.Date.AddDays(1).AddTicks(-1);
            var records = await _databaseService.GetRecordsByDateRangeAsync(start, end);

            foreach (var record in records)
            {
                await _databaseService.DeleteRecordAsync(record);
            }

            // Refresh the list
            await LoadRecordsInternalAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ListViewModel: DeleteRecordAsync error: {ex}");
        }
    }

    // Handle the message from RecordViewModel
    public void Receive(RecordPageLoadedMessage message)
    {
        _pageLoadTcs?.TrySetResult(true);
    }

    public void Receive(RecordPageCancelMessage message)
    {
        _skipSpinnerOnLoad = true;
    }
}

// Message classes for communication
public class RecordPageLoadedMessage
{
    public string Status { get; set; } = "Loaded";
}

public class RecordPageCancelMessage
{
    public string Status { get; set; } = "Cancelled";
}