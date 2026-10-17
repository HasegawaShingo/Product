namespace BloodPressureTracker.ViewModels;

using BloodPressureTracker.Models;
using BloodPressureTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

public partial class StatisticsViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    int selectedYear = DateTime.Today.Year;

    [ObservableProperty]
    int selectedMonth = DateTime.Today.Month;

    [ObservableProperty]
    DateTime selectedWeekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

    [ObservableProperty]
    WeeklyAverage weeklyAverage = new();

    [ObservableProperty]
    MonthlyAverage monthlyAverage = new();

    [ObservableProperty]
    ChartData weeklyChartData = new();

    [ObservableProperty]
    ChartData monthlyChartData = new();

    [ObservableProperty]
    string weeklyDateRange = string.Empty;

    [ObservableProperty]
    string monthlyDateRange = string.Empty;

    // 週平均のテキスト表示用
    [ObservableProperty]
    string weeklySystolicText = string.Empty;

    [ObservableProperty]
    string weeklyDiastolicText = string.Empty;

    [ObservableProperty]
    string weeklyPulseText = string.Empty;

    // 月平均のテキスト表示用
    [ObservableProperty]
    string monthlySystolicText = string.Empty;

    [ObservableProperty]
    string monthlyDiastolicText = string.Empty;

    [ObservableProperty]
    string monthlyPulseText = string.Empty;

    public int[] Years { get; } = Enumerable.Range(DateTime.Today.Year - 5, 11).ToArray();
    public int[] Months { get; } = Enumerable.Range(1, 12).ToArray();

    public StatisticsViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        Title = "統計";

        _ = LoadDataAsync();
    }

    [RelayCommand]
    async Task LoadWeeklyAsync()
    {
        IsBusy = true;
        try
        {
            var avg = await _databaseService.GetWeeklyAverageAsync(SelectedWeekStart);
            WeeklyAverage = avg ?? new WeeklyAverage { WeekStart = SelectedWeekStart };
            await CreateWeeklyChartDataAsync();
            UpdateWeeklyDateRange();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task LoadMonthlyAsync()
    {
        IsBusy = true;
        try
        {
            var avg = await _databaseService.GetMonthlyAverageAsync(SelectedYear, SelectedMonth);
            MonthlyAverage = avg ?? new MonthlyAverage { Year = SelectedYear, Month = SelectedMonth };
            await CreateMonthlyChartDataAsync();
            UpdateMonthlyDateRange();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    void PreviousWeek()
    {
        SelectedWeekStart = SelectedWeekStart.AddDays(-7);
        _ = LoadWeeklyAsync();
    }

    [RelayCommand]
    void NextWeek()
    {
        SelectedWeekStart = SelectedWeekStart.AddDays(7);
        _ = LoadWeeklyAsync();
    }

    [RelayCommand]
    void PreviousMonth()
    {
        if (SelectedMonth == 1)
        {
            SelectedMonth = 12;
            SelectedYear--;
        }
        else
        {
            SelectedMonth--;
        }
        _ = LoadMonthlyAsync();
    }

    [RelayCommand]
    void NextMonth()
    {
        if (SelectedMonth == 12)
        {
            SelectedMonth = 1;
            SelectedYear++;
        }
        else
        {
            SelectedMonth++;
        }
        _ = LoadMonthlyAsync();
    }

    public async Task LoadDataAsync()
    {
        await LoadWeeklyAsync();
        await LoadMonthlyAsync();
    }

    private void UpdateWeeklyDateRange()
    {
        var weekEnd = SelectedWeekStart.AddDays(6);
        WeeklyDateRange = $"{SelectedWeekStart:M/d} ～ {weekEnd:M/d}";
    }

    private void UpdateMonthlyDateRange()
    {
        MonthlyDateRange = $"{SelectedYear}年 {SelectedMonth}月";
    }

    private async Task CreateWeeklyChartDataAsync()
    {
        var records = await _databaseService.GetRecordsByDateRangeAsync(SelectedWeekStart, SelectedWeekStart.AddDays(6));

        var dailyData = records
            .GroupBy(r => r.RecordDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Date = g.Key,
                AvgSystolic = (int)Math.Round(g.Average(r => r.AvgSystolic)),
                AvgDiastolic = (int)Math.Round(g.Average(r => r.AvgDiastolic)),
                AvgPulse = (int)Math.Round(g.Average(r => r.AvgPulse))
            })
            .ToList();

        var allDates = new List<DateTime>();
        for (int i = 0; i < 7; i++)
        {
            allDates.Add(SelectedWeekStart.AddDays(i));
        }

        var labels = allDates.Select(d => d.ToString("M/d")).ToArray();

        var systolicValues = new List<double?>();
        var diastolicValues = new List<double?>();
        var pulseValues = new List<double?>();

        for (int i = 0; i < 7; i++)
        {
            var date = allDates[i];
            var dayData = dailyData.FirstOrDefault(d => d.Date == date);
            if (dayData != null)
            {
                systolicValues.Add(dayData.AvgSystolic);
                diastolicValues.Add(dayData.AvgDiastolic);
                pulseValues.Add(dayData.AvgPulse);
            }
            else
            {
                systolicValues.Add(null);
                diastolicValues.Add(null);
                pulseValues.Add(null);
            }
        }

        WeeklyChartData = new ChartData
        {
            Labels = labels,
            SystolicValues = systolicValues,
            DiastolicValues = diastolicValues,
            PulseValues = pulseValues,
            Title = "週平均",
            MinValue = 50,
            MaxValue = 200
        };

        // テキスト表示用（平均値を整数表示）
        var weeklySystolic = dailyData.Count > 0 ? Math.Round(dailyData.Average(d => d.AvgSystolic)) : 0;
        var weeklyDiastolic = dailyData.Count > 0 ? Math.Round(dailyData.Average(d => d.AvgDiastolic)) : 0;
        var weeklyPulse = dailyData.Count > 0 ? Math.Round(dailyData.Average(d => d.AvgPulse)) : 0;
        WeeklySystolicText = $"収縮期 {weeklySystolic} mmHg";
        WeeklyDiastolicText = $"拡張期 {weeklyDiastolic} mmHg";
        WeeklyPulseText = $"脈拍 {weeklyPulse} bpm";

        OnPropertyChanged(nameof(WeeklyChartData));
    }

    private async Task CreateMonthlyChartDataAsync()
    {
        var start = new DateTime(SelectedYear, SelectedMonth, 1);
        var end = start.AddMonths(1).AddDays(-1);
        var records = await _databaseService.GetRecordsByDateRangeAsync(start, end);

        var dailyData = records
            .GroupBy(r => r.RecordDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Date = g.Key,
                AvgSystolic = (int)Math.Round(g.Average(r => r.AvgSystolic)),
                AvgDiastolic = (int)Math.Round(g.Average(r => r.AvgDiastolic)),
                AvgPulse = (int)Math.Round(g.Average(r => r.AvgPulse))
            })
            .ToList();

        var daysInMonth = DateTime.DaysInMonth(SelectedYear, SelectedMonth);
        var allDates = new List<DateTime>();
        for (int i = 1; i <= daysInMonth; i++)
        {
            allDates.Add(new DateTime(SelectedYear, SelectedMonth, i));
        }

        var labels = allDates.Select(d => d.ToString("M/d")).ToArray();

        var systolicValues = new List<double?>();
        var diastolicValues = new List<double?>();
        var pulseValues = new List<double?>();

        for (int i = 0; i < daysInMonth; i++)
        {
            var date = allDates[i];
            var dayData = dailyData.FirstOrDefault(d => d.Date == date);
            if (dayData != null)
            {
                systolicValues.Add(dayData.AvgSystolic);
                diastolicValues.Add(dayData.AvgDiastolic);
                pulseValues.Add(dayData.AvgPulse);
            }
            else
            {
                systolicValues.Add(null);
                diastolicValues.Add(null);
                pulseValues.Add(null);
            }
        }

        MonthlyChartData = new ChartData
        {
            Labels = labels,
            SystolicValues = systolicValues,
            DiastolicValues = diastolicValues,
            PulseValues = pulseValues,
            Title = "月平均",
            MinValue = 50,
            MaxValue = 200
        };

        // テキスト表示用（平均値を整数表示）
        var monthlySystolic = dailyData.Count > 0 ? Math.Round(dailyData.Average(d => d.AvgSystolic)) : 0;
        var monthlyDiastolic = dailyData.Count > 0 ? Math.Round(dailyData.Average(d => d.AvgDiastolic)) : 0;
        var monthlyPulse = dailyData.Count > 0 ? Math.Round(dailyData.Average(d => d.AvgPulse)) : 0;
        MonthlySystolicText = $"収縮期 {monthlySystolic} mmHg";
        MonthlyDiastolicText = $"拡張期 {monthlyDiastolic} mmHg";
        MonthlyPulseText = $"脈拍 {monthlyPulse} bpm";

        OnPropertyChanged(nameof(MonthlyChartData));
    }
}

public class ChartData
{
    public string[] Labels { get; set; } = [];
    public List<double?> SystolicValues { get; set; } = [];
    public List<double?> DiastolicValues { get; set; } = [];
    public List<double?> PulseValues { get; set; } = [];
    public string Title { get; set; } = "";
    public double MinValue { get; set; } = 50;
    public double MaxValue { get; set; } = 200;
}