namespace BloodPressureTracker.ViewModels;

using BloodPressureTracker.Models;
using BloodPressureTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class RecordViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private BloodPressureRecord? _editingMorning;
    private BloodPressureRecord? _editingEvening;

[ObservableProperty]
DateTime recordDate = DateTime.Today;

    [ObservableProperty]
    TimeSpan morningTime = default;

    [ObservableProperty]
    TimeSpan eveningTime = default;

    // Morning measurements
    [ObservableProperty]
    int? morningSystolic1;
    [ObservableProperty]
    int? morningDiastolic1;
    [ObservableProperty]
    int? morningPulse1;
    [ObservableProperty]
    int? morningSystolic2;
    [ObservableProperty]
    int? morningDiastolic2;
    [ObservableProperty]
    int? morningPulse2;

    // Evening measurements
    [ObservableProperty]
    int? eveningSystolic1;
    [ObservableProperty]
    int? eveningDiastolic1;
    [ObservableProperty]
    int? eveningPulse1;
    [ObservableProperty]
    int? eveningSystolic2;
    [ObservableProperty]
    int? eveningDiastolic2;
    [ObservableProperty]
    int? eveningPulse2;

    [ObservableProperty]
    string notes = string.Empty;

    public int? MorningAvgSystolic => ComputeAverage(MorningSystolic1, MorningSystolic2);
    public int? MorningAvgDiastolic => ComputeAverage(MorningDiastolic1, MorningDiastolic2);
    public int? MorningAvgPulse => ComputeAverage(MorningPulse1, MorningPulse2);

    public int? EveningAvgSystolic => ComputeAverage(EveningSystolic1, EveningSystolic2);
    public int? EveningAvgDiastolic => ComputeAverage(EveningDiastolic1, EveningDiastolic2);
    public int? EveningAvgPulse => ComputeAverage(EveningPulse1, EveningPulse2);

    public int? DayAvgSystolic => ComputeAverage(MorningAvgSystolic, EveningAvgSystolic);
    public int? DayAvgDiastolic => ComputeAverage(MorningAvgDiastolic, EveningAvgDiastolic);
    public int? DayAvgPulse => ComputeAverage(MorningAvgPulse, EveningAvgPulse);

    private static int? ComputeAverage(int? a, int? b)
    {
        if (a.HasValue && b.HasValue) return (int?)((a.Value + b.Value) / 2);
        if (a.HasValue) return a.Value;
        if (b.HasValue) return b.Value;
        return null;
    }

    private static DateTime RoundToNearestMinutes(DateTime dt, int minutes)
    {
        if (minutes <= 0) return dt;
        var ticks = TimeSpan.TicksPerMinute * minutes;
        var roundedTicks = ((dt.Ticks + ticks / 2) / ticks) * ticks;
        return new DateTime(roundedTicks, dt.Kind);
    }

    public RecordViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        Title = "測定記録";

        // Start busy - spinner shows immediately on page appearance
        IsBusy = true;

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(IsBusy))
                OnPropertyChanged(nameof(IsNotBusy));
        };
    }

    public bool IsNotBusy => !IsBusy;

    /// <summary>
    /// Called from RecordPage.OnAppearing to load data based on navigation parameters.
    /// </summary>
    public async Task LoadAsync(string? recordId = null, string? date = null, string? newRecord = null)
    {
        System.Diagnostics.Debug.WriteLine($"RecordViewModel.LoadAsync called - recordId='{recordId}', date='{date}', newRecord='{newRecord}'");
        
        try
        {
            // Priority: RecordId > Date > NewRecord > Default (today)
            if (!string.IsNullOrWhiteSpace(recordId) && int.TryParse(recordId, out var id))
            {
                System.Diagnostics.Debug.WriteLine($"RecordViewModel: LoadAsync RecordId={id}");
                await LoadRecordAsync(id);
            }
            else if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                System.Diagnostics.Debug.WriteLine($"RecordViewModel: LoadAsync Date={parsedDate:yyyy-MM-dd}");
                await LoadForDateAsync(parsedDate);
            }
            else if (!string.IsNullOrWhiteSpace(newRecord) && newRecord == "true")
            {
                System.Diagnostics.Debug.WriteLine($"RecordViewModel: LoadAsync NewRecord=true");
                InitializeForNew(DateTime.Now);
            }
            else
            {
                // Default: load today's records
                System.Diagnostics.Debug.WriteLine($"RecordViewModel: LoadAsync default (today)");
                await LoadForDateAsync(DateTime.Today);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RecordViewModel.LoadAsync error: {ex}");
        }
        finally
        {
            IsBusy = false;
            // Notify ListViewModel that loading is complete
            WeakReferenceMessenger.Default.Send(new RecordPageLoadedMessage());
        }
    }

    // RecordPage.xaml.cs expects LoadRecordAsync(int) and LoadForDateAsync(DateTime)
    public async Task LoadRecordAsync(int id)
    {
        var record = await _databaseService.GetRecordAsync(id);
        if (record == null) return;
        var start = record.RecordDate.Date;
        var end = record.RecordDate.Date.AddDays(1).AddTicks(-1);
        var records = await _databaseService.GetRecordsByDateRangeAsync(start, end);
        PopulateFromRecords(records);
        RecordDate = record.RecordDate.Date;
    }

    public async Task LoadForDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = date.Date.AddDays(1).AddTicks(-1);
        var records = await _databaseService.GetRecordsByDateRangeAsync(start, end);
        PopulateFromRecords(records);
        RecordDate = date.Date;
    }

    /// <summary>
    /// Prepare the ViewModel for a new empty record form (clear all inputs).
    /// </summary>
    public void InitializeForNew(DateTime date)
    {
        _editingMorning = null;
        _editingEvening = null;
        RecordDate = date.Date;
        MorningTime = default;
        EveningTime = default;
        MorningSystolic1 = MorningDiastolic1 = MorningPulse1 = null;
        MorningSystolic2 = MorningDiastolic2 = MorningPulse2 = null;
        EveningSystolic1 = EveningDiastolic1 = EveningPulse1 = null;
        EveningSystolic2 = EveningDiastolic2 = EveningPulse2 = null;
        Notes = string.Empty;
        RaiseAverages();
    }

    void PopulateFromRecords(List<BloodPressureRecord> records)
    {
        _editingMorning = records.FirstOrDefault(r => r.TimeCategory == TimeCategory.Morning);
        _editingEvening = records.FirstOrDefault(r => r.TimeCategory == TimeCategory.Evening);

        if (_editingMorning != null)
        {
            MorningSystolic1 = _editingMorning.Systolic1;
            MorningDiastolic1 = _editingMorning.Diastolic1;
            MorningPulse1 = _editingMorning.Pulse1;
            MorningSystolic2 = _editingMorning.Systolic2;
            MorningDiastolic2 = _editingMorning.Diastolic2;
            MorningPulse2 = _editingMorning.Pulse2;
            MorningTime = _editingMorning.RecordDate.TimeOfDay;
        }
        else
        {
            MorningSystolic1 = MorningDiastolic1 = MorningPulse1 = null;
            MorningSystolic2 = MorningDiastolic2 = MorningPulse2 = null;
            MorningTime = default;
        }

        if (_editingEvening != null)
        {
            EveningSystolic1 = _editingEvening.Systolic1;
            EveningDiastolic1 = _editingEvening.Diastolic1;
            EveningPulse1 = _editingEvening.Pulse1;
            EveningSystolic2 = _editingEvening.Systolic2;
            EveningDiastolic2 = _editingEvening.Diastolic2;
            EveningPulse2 = _editingEvening.Pulse2;
            EveningTime = _editingEvening.RecordDate.TimeOfDay;
        }
        else
        {
            EveningSystolic1 = EveningDiastolic1 = EveningPulse1 = null;
            EveningSystolic2 = EveningDiastolic2 = EveningPulse2 = null;
            EveningTime = default;
        }

        RaiseAverages();
    }

    [RelayCommand]
    async Task SaveAsync()
    {
        if (!await ValidateBeforeSave()) return;

        IsBusy = true;
        try
        {
            var existing = await _databaseService.GetRecordsByDateRangeAsync(RecordDate.Date, RecordDate.Date.AddDays(1).AddTicks(-1));

            if ((existing?.Count ?? 0) > 0 && _editingMorning == null && _editingEvening == null)
            {
                var overwrite = await Shell.Current.DisplayAlertAsync("上書き確認", "この日には既に記録があります。上書きしますか？", "上書きする", "キャンセル");
                if (!overwrite) return;
            }

            // Morning
            if (HasAnyMorningInput())
            {
                var morning = existing?.FirstOrDefault(r => r.TimeCategory == TimeCategory.Morning) ?? new BloodPressureRecord();
                morning.RecordDate = RecordDate.Date + MorningTime;
                morning.TimeCategory = TimeCategory.Morning;
                morning.Systolic1 = MorningSystolic1.GetValueOrDefault();
                morning.Diastolic1 = MorningDiastolic1.GetValueOrDefault();
                morning.Pulse1 = MorningPulse1.GetValueOrDefault();
                morning.Systolic2 = MorningSystolic2 ?? MorningSystolic1.GetValueOrDefault();
                morning.Diastolic2 = MorningDiastolic2 ?? MorningDiastolic1.GetValueOrDefault();
                morning.Pulse2 = MorningPulse2 ?? MorningPulse1.GetValueOrDefault();
                morning.Notes = Notes;
                await _databaseService.SaveRecordAsync(morning);
            }
            else
            {
            var morning = existing?.FirstOrDefault(r => r.TimeCategory == TimeCategory.Morning);
            if (morning != null) await _databaseService.DeleteRecordAsync(morning);
            }

            // Evening
            if (HasAnyEveningInput())
            {
                var evening = existing?.FirstOrDefault(r => r.TimeCategory == TimeCategory.Evening) ?? new BloodPressureRecord();
                evening.RecordDate = RecordDate.Date + EveningTime;
                evening.TimeCategory = TimeCategory.Evening;
                evening.Systolic1 = EveningSystolic1.GetValueOrDefault();
                evening.Diastolic1 = EveningDiastolic1.GetValueOrDefault();
                evening.Pulse1 = EveningPulse1.GetValueOrDefault();
                evening.Systolic2 = EveningSystolic2 ?? EveningSystolic1.GetValueOrDefault();
                evening.Diastolic2 = EveningDiastolic2 ?? EveningDiastolic1.GetValueOrDefault();
                evening.Pulse2 = EveningPulse2 ?? EveningPulse1.GetValueOrDefault();
                evening.Notes = Notes;
                await _databaseService.SaveRecordAsync(evening);
            }
            else
            {
            var evening = existing?.FirstOrDefault(r => r.TimeCategory == TimeCategory.Evening);
            if (evening != null) await _databaseService.DeleteRecordAsync(evening);
            }

            var mainPage = Application.Current?.Windows[0]?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlertAsync("保存", "保存しました", "OK");
            }

            await Shell.Current.GoToAsync("///list");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task CancelAsync()
    {
        var confirm = await Shell.Current.DisplayAlertAsync("確認", "編集を取り消し、最初の画面に戻りますか？", "はい", "いいえ");
        if (!confirm) return;

        // Notify ListViewModel to skip spinner on return
        WeakReferenceMessenger.Default.Send(new RecordPageCancelMessage());
        
        await Shell.Current.GoToAsync("///list");
    }

    bool HasAnyMorningInput() => MorningSystolic1.HasValue || MorningSystolic2.HasValue || MorningDiastolic1.HasValue || MorningDiastolic2.HasValue || MorningPulse1.HasValue || MorningPulse2.HasValue;
    bool HasAnyEveningInput() => EveningSystolic1.HasValue || EveningSystolic2.HasValue || EveningDiastolic1.HasValue || EveningDiastolic2.HasValue || EveningPulse1.HasValue || EveningPulse2.HasValue;

    async Task<bool> ValidateBeforeSave()
    {
        if (!HasAnyMorningInput() && !HasAnyEveningInput())
            return true;

        if (HasAnyMorningInput())
        {
            if (!MorningSystolic1.HasValue || !MorningDiastolic1.HasValue || !MorningPulse1.HasValue)
            {
                await Shell.Current.DisplayAlertAsync("入力エラー", "朝の測定は1回目の値をすべて入力してください", "OK");
                return false;
            }
        }

        if (HasAnyEveningInput())
        {
            if (!EveningSystolic1.HasValue || !EveningDiastolic1.HasValue || !EveningPulse1.HasValue)
            {
                await Shell.Current.DisplayAlertAsync("入力エラー", "夜の測定は1回目の値をすべて入力してください", "OK");
                return false;
            }
        }

        if (MorningSystolic1.HasValue && (MorningSystolic1 < 30 || MorningSystolic1 > 300))
        {
                await Shell.Current.DisplayAlertAsync("入力エラー", "朝の収縮期血圧が不正です", "OK");
            return false;
        }

        return true;
    }

    void RaiseAverages()
    {
        OnPropertyChanged(nameof(MorningAvgSystolic));
        OnPropertyChanged(nameof(MorningAvgDiastolic));
        OnPropertyChanged(nameof(MorningAvgPulse));
        OnPropertyChanged(nameof(EveningAvgSystolic));
        OnPropertyChanged(nameof(EveningAvgDiastolic));
        OnPropertyChanged(nameof(EveningAvgPulse));
        OnPropertyChanged(nameof(DayAvgSystolic));
        OnPropertyChanged(nameof(DayAvgDiastolic));
        OnPropertyChanged(nameof(DayAvgPulse));
}
}