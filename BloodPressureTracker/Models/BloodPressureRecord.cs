namespace BloodPressureTracker.Models;

using SQLite;

[Table("blood_pressure_records")]
public class BloodPressureRecord
{
    [PrimaryKey, AutoIncrement, Column("id")]
    public int Id { get; set; }

    [Column("record_date")]
    public DateTime RecordDate { get; set; }

    [Column("time_category")]
    public TimeCategory TimeCategory { get; set; }

    [Column("systolic_1")]
    public int Systolic1 { get; set; }

    [Column("diastolic_1")]
    public int Diastolic1 { get; set; }

    [Column("pulse_1")]
    public int Pulse1 { get; set; }

    [Column("systolic_2")]
    public int Systolic2 { get; set; }

    [Column("diastolic_2")]
    public int Diastolic2 { get; set; }

    [Column("pulse_2")]
    public int Pulse2 { get; set; }

    [Ignore]
    public int AvgSystolic => (Systolic1 + Systolic2) / 2;

    [Ignore]
    public int AvgDiastolic => (Diastolic1 + Diastolic2) / 2;

    [Ignore]
    public int AvgPulse => (Pulse1 + Pulse2) / 2;

    [Ignore]
    public string FirstReading => $"{Systolic1}/{Diastolic1}";

    [Ignore]
    public string SecondReading => $"{Systolic2}/{Diastolic2}";

    [Ignore]
    public string AverageReading => $"{AvgSystolic}/{AvgDiastolic}";

    [Ignore]
    public string AverageWithPulse => $"{AvgSystolic}/{AvgDiastolic} mmHg, {AvgPulse} bpm";

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum TimeCategory
{
    Morning = 0,
    Evening = 1
}