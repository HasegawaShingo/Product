namespace BloodPressureTracker.Services;

using BloodPressureTracker.Models;
using SQLite;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        // テーブル作成を同期的に待機（起動時の競合を避けるため）
        _database.CreateTableAsync<BloodPressureRecord>().GetAwaiter().GetResult();
    }

    public Task<List<BloodPressureRecord>> GetRecordsAsync()
        => _database.Table<BloodPressureRecord>()
            .OrderByDescending(r => r.RecordDate)
            .ThenBy(r => r.TimeCategory)
            .ToListAsync();

    public Task<List<BloodPressureRecord>> GetRecordsByDateRangeAsync(DateTime start, DateTime end)
        {
            // Caller may pass date.Date as both start and end. Ensure end covers the full day when start==end.
            if (start.Date == end.Date && start.TimeOfDay == TimeSpan.Zero && end.TimeOfDay == TimeSpan.Zero)
            {
                end = end.Date.AddDays(1).AddTicks(-1);
            }
            return _database.Table<BloodPressureRecord>()
                .Where(r => r.RecordDate >= start && r.RecordDate <= end)
                .OrderBy(r => r.RecordDate)
                .ThenBy(r => r.TimeCategory)
                .ToListAsync();
        }

    public async Task<BloodPressureRecord?> GetRecordAsync(int id)
        => await _database.Table<BloodPressureRecord>().FirstOrDefaultAsync(r => r.Id == id);

    public Task<int> SaveRecordAsync(BloodPressureRecord record)
    {
        if (record.Id != 0)
            return _database.UpdateAsync(record);
        return _database.InsertAsync(record);
    }

    public Task<int> DeleteRecordAsync(BloodPressureRecord record)
        => _database.DeleteAsync(record);

    public async Task<WeeklyAverage?> GetWeeklyAverageAsync(DateTime weekStart)
    {
        var weekEnd = weekStart.AddDays(6);
        var records = await GetRecordsByDateRangeAsync(weekStart, weekEnd);
        if (records.Count == 0) return null;

        return new WeeklyAverage
        {
            WeekStart = weekStart,
            MorningAvgSystolic = records.Where(r => r.TimeCategory == TimeCategory.Morning).DefaultIfEmpty().Average(r => r?.AvgSystolic ?? 0),
            MorningAvgDiastolic = records.Where(r => r.TimeCategory == TimeCategory.Morning).DefaultIfEmpty().Average(r => r?.AvgDiastolic ?? 0),
            MorningAvgPulse = records.Where(r => r.TimeCategory == TimeCategory.Morning).DefaultIfEmpty().Average(r => r?.AvgPulse ?? 0),
            EveningAvgSystolic = records.Where(r => r.TimeCategory == TimeCategory.Evening).DefaultIfEmpty().Average(r => r?.AvgSystolic ?? 0),
            EveningAvgDiastolic = records.Where(r => r.TimeCategory == TimeCategory.Evening).DefaultIfEmpty().Average(r => r?.AvgDiastolic ?? 0),
            EveningAvgPulse = records.Where(r => r.TimeCategory == TimeCategory.Evening).DefaultIfEmpty().Average(r => r?.AvgPulse ?? 0),
            RecordCount = records.Count
        };
    }

    public async Task<MonthlyAverage?> GetMonthlyAverageAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        var records = await GetRecordsByDateRangeAsync(start, end);
        if (records.Count == 0) return null;

        return new MonthlyAverage
        {
            Year = year,
            Month = month,
            MorningAvgSystolic = records.Where(r => r.TimeCategory == TimeCategory.Morning).DefaultIfEmpty().Average(r => r?.AvgSystolic ?? 0),
            MorningAvgDiastolic = records.Where(r => r.TimeCategory == TimeCategory.Morning).DefaultIfEmpty().Average(r => r?.AvgDiastolic ?? 0),
            MorningAvgPulse = records.Where(r => r.TimeCategory == TimeCategory.Morning).DefaultIfEmpty().Average(r => r?.AvgPulse ?? 0),
            EveningAvgSystolic = records.Where(r => r.TimeCategory == TimeCategory.Evening).DefaultIfEmpty().Average(r => r?.AvgSystolic ?? 0),
            EveningAvgDiastolic = records.Where(r => r.TimeCategory == TimeCategory.Evening).DefaultIfEmpty().Average(r => r?.AvgDiastolic ?? 0),
            EveningAvgPulse = records.Where(r => r.TimeCategory == TimeCategory.Evening).DefaultIfEmpty().Average(r => r?.AvgPulse ?? 0),
            RecordCount = records.Count
        };
    }
}

public class WeeklyAverage
{
    public DateTime WeekStart { get; set; }
    public double MorningAvgSystolic { get; set; }
    public double MorningAvgDiastolic { get; set; }
    public double MorningAvgPulse { get; set; }
    public double EveningAvgSystolic { get; set; }
    public double EveningAvgDiastolic { get; set; }
    public double EveningAvgPulse { get; set; }
    public int RecordCount { get; set; }

    public string MorningReading => $"{MorningAvgSystolic:F0}/{MorningAvgDiastolic:F0}";
    public string EveningReading => $"{EveningAvgSystolic:F0}/{EveningAvgDiastolic:F0}";
}

public class MonthlyAverage
{
    public int Year { get; set; }
    public int Month { get; set; }
    public double MorningAvgSystolic { get; set; }
    public double MorningAvgDiastolic { get; set; }
    public double MorningAvgPulse { get; set; }
    public double EveningAvgSystolic { get; set; }
    public double EveningAvgDiastolic { get; set; }
    public double EveningAvgPulse { get; set; }
    public int RecordCount { get; set; }

    public string MorningReading => $"{MorningAvgSystolic:F0}/{MorningAvgDiastolic:F0}";
    public string EveningReading => $"{EveningAvgSystolic:F0}/{EveningAvgDiastolic:F0}";
}