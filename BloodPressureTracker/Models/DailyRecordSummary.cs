namespace BloodPressureTracker.Models;

using System;

public class DailyRecordSummary
{
    public DateTime RecordDate { get; set; }

    // Day averages
    public int? DayAvgSystolic { get; set; }
    public int? DayAvgDiastolic { get; set; }
    public int? DayAvgPulse { get; set; }

    // Morning averages (may be null)
    public int? MorningAvgSystolic { get; set; }
    public int? MorningAvgDiastolic { get; set; }
    public int? MorningAvgPulse { get; set; }

    // Evening averages (may be null)
    public int? EveningAvgSystolic { get; set; }
    public int? EveningAvgDiastolic { get; set; }
    public int? EveningAvgPulse { get; set; }

    public string DayAverageReading => DayAvgSystolic.HasValue && DayAvgDiastolic.HasValue ? $"{DayAvgSystolic} / {DayAvgDiastolic} mmHg" : "-";
    public string MorningReading => MorningAvgSystolic.HasValue && MorningAvgDiastolic.HasValue ? $"{MorningAvgSystolic} / {MorningAvgDiastolic} mmHg" : "-";
    public string EveningReading => EveningAvgSystolic.HasValue && EveningAvgDiastolic.HasValue ? $"{EveningAvgSystolic} / {EveningAvgDiastolic} mmHg" : "-";

    public string DayAvgPulseDisplay => DayAvgPulse.HasValue ? $"{DayAvgPulse} bpm" : "-";
    public string MorningPulseDisplay => MorningAvgPulse.HasValue ? $"{MorningAvgPulse} bpm" : "-";
    public string EveningPulseDisplay => EveningAvgPulse.HasValue ? $"{EveningAvgPulse} bpm" : "-";

    public string Notes { get; set; } = string.Empty;
}
