namespace BloodPressureTracker.Converters;

using System.Globalization;

public class EnumToListConverter : IValueConverter
{
    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (value is Enum enumValue)
        {
            return Enum.GetValues(enumValue.GetType());
        }
        return Array.Empty<object>();
    }

    public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        => throw new NotImplementedException();
}

public class TimeCategoryToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        return value switch
        {
            BloodPressureTracker.Models.TimeCategory.Morning => "朝",
            BloodPressureTracker.Models.TimeCategory.Evening => "晩",
            _ => ""
        };
    }

    public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        => throw new NotImplementedException();
}

public class StringToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        => !string.IsNullOrWhiteSpace(value as string);

    public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        => throw new NotImplementedException();
}

public class BoolInverterConverter : IValueConverter
{
    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        => value is bool b && !b;

    public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        => throw new NotImplementedException();
}

public class WeekRangeConverter : IValueConverter
{
    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (value is DateTime start)
        {
            var end = start.AddDays(6);
            return $"{start:M/d} 〜 {end:M/d}";
        }
        return "";
    }

    public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        => throw new NotImplementedException();
}

public class MonthYearConverter : IValueConverter
{
    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (value is int year && parameter is int month)
        {
            return $"{year}年 {month}月";
        }
        return "";
    }

    public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        => throw new NotImplementedException();
}