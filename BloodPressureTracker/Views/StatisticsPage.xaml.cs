namespace BloodPressureTracker.Views;

using BloodPressureTracker.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Maui;

public partial class StatisticsPage : ContentPage
{
    private static readonly SKColor SystolicColor = SKColor.Parse("#e74c3c");
    private static readonly SKColor DiastolicColor = SKColor.Parse("#3498db");
    private static readonly SKColor PulseColor = SKColor.Parse("#27ae60");
    private static readonly SKColor TextColor = SKColors.White;
    private static readonly SKColor GridColor = SKColor.Parse("#d3d3d3");
    private static readonly SKColor ChartBackgroundColor = SKColor.Parse("#16213e");

    // チャート描画用のマージン
    private readonly float _margin = 60f;

    // 日本語フォント: Androidシステムフォントを直接読み込み（最優先）
    private static readonly SKTypeface JapaneseTypeface = 
        SKTypeface.FromFile("/system/fonts/NotoSansCJK-Regular.ttc") ??
        SKTypeface.FromFile("/system/fonts/NotoSansJP-Regular.otf") ??
        SKTypeface.FromFile("/system/fonts/DroidSansJapanese.ttf") ??
        SKTypeface.FromFamilyName("sans-serif-japanese") ??
        SKTypeface.FromFamilyName("Noto Sans CJK JP") ??
        SKTypeface.FromFamilyName("Noto Sans JP") ??
        SKTypeface.FromFamilyName("sans-serif") ??
        SKTypeface.Default;

    // フォント: 日本語Typefaceを使用
    private readonly SKFont _axisFont = new(JapaneseTypeface, 12);
    private readonly SKFont _legendFont = new(JapaneseTypeface, 22);
    private readonly SKFont _axisLabelFont = new(JapaneseTypeface, 14);
    private readonly SKFont _textFont = new(JapaneseTypeface, 22);

    public StatisticsPage(StatisticsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private StatisticsViewModel ViewModel => (BindingContext as StatisticsViewModel)!;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            await ViewModel.LoadDataAsync();
        }
        WeeklyCanvasView?.InvalidateSurface();
        MonthlyCanvasView?.InvalidateSurface();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(StatisticsViewModel.WeeklyChartData))
        {
            WeeklyCanvasView?.InvalidateSurface();
        }
        else if (e.PropertyName == nameof(StatisticsViewModel.MonthlyChartData))
        {
            MonthlyCanvasView?.InvalidateSurface();
        }
    }

    private void OnWeeklyPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (ViewModel?.WeeklyChartData != null)
        {
            DrawChart(e.Surface.Canvas, e.Info.Width, e.Info.Height, ViewModel.WeeklyChartData, false,
                ViewModel.WeeklySystolicText, ViewModel.WeeklyDiastolicText, ViewModel.WeeklyPulseText);
        }
    }

    private void OnMonthlyPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (ViewModel?.MonthlyChartData != null)
        {
            DrawChart(e.Surface.Canvas, e.Info.Width, e.Info.Height, ViewModel.MonthlyChartData, true,
                ViewModel.MonthlySystolicText, ViewModel.MonthlyDiastolicText, ViewModel.MonthlyPulseText);
        }
    }

    private void DrawChart(SKCanvas canvas, int width, int height, ChartData data, bool isMonthly, string systolicText, string diastolicText, string pulseText)
    {
        try
        {
            canvas.Clear(ChartBackgroundColor);

            if (data.Labels == null || data.Labels.Length == 0)
                return;

            var chartWidth = width - 2 * _margin;
            var chartHeight = height - 2 * _margin - 50; // 下部に統計値テキスト用のスペースを確保
            
            if (chartWidth <= 0 || chartHeight <= 0)
                return;

            var plotLeft = _margin;
            var plotTop = _margin;
            var plotRight = width - _margin;
            var plotBottom = height - _margin - 50; // 下部マージンを拡張

            // Grid lines
            using var gridPaint = new SKPaint
            {
                Color = GridColor,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            var yStepCount = 5;
            for (int i = 0; i <= yStepCount; i++)
            {
                var y = plotTop + (chartHeight * i / yStepCount);
                canvas.DrawLine(plotLeft, y, plotRight, y, gridPaint);
            }

            // Y axis labels (固定値: 50, 80, 110, 140, 170, 200)
            var yLabels = new[] { 200, 170, 140, 110, 80, 50 };
            using var yAxisPaint = new SKPaint { Color = TextColor, IsAntialias = true };
            for (int i = 0; i < yLabels.Length; i++)
            {
                var value = yLabels[i];
                var y = plotTop + (chartHeight * i / (yLabels.Length - 1));
                var text = $"{value}";
                // Y軸ラベルを左にずらしてX軸ラベルと重ならないようにする
                canvas.DrawText(text, _margin - 45, y + _axisLabelFont.Size / 3, SKTextAlign.Left, _axisLabelFont, new SKPaint { Color = TextColor, IsAntialias = true });
            }

            // X axis labels
            var pointCount = data.Labels.Length;
            using var axisPaint = new SKPaint { Color = TextColor, IsAntialias = true };
            if (pointCount > 1)
            {
                var stepX = chartWidth / (pointCount - 1);
                var labelInterval = isMonthly ? 5 : 1;
                
                for (int i = 0; i < pointCount; i += labelInterval)
                {
                    var x = plotLeft + i * stepX;
                    var y = plotBottom + 20;
                    canvas.DrawText(data.Labels[i], x, y + _axisFont.Size, SKTextAlign.Left, _axisFont, axisPaint);
                }
            }
            else if (pointCount == 1)
            {
                canvas.DrawText(data.Labels[0], plotLeft, plotBottom + 20 + _axisFont.Size, SKTextAlign.Left, _axisFont, new SKPaint { Color = TextColor, IsAntialias = true });
            }

            // Draw series
            DrawSeries(canvas, data.SystolicValues, SystolicColor, plotLeft, plotTop, chartWidth, chartHeight, data.MinValue, data.MaxValue, pointCount, isMonthly);
            DrawSeries(canvas, data.DiastolicValues, DiastolicColor, plotLeft, plotTop, chartWidth, chartHeight, data.MinValue, data.MaxValue, pointCount, isMonthly);
            DrawSeries(canvas, data.PulseValues, PulseColor, plotLeft, plotTop, chartWidth, chartHeight, data.MinValue, data.MaxValue, pointCount, isMonthly);

            // Legend (top center)
            DrawLegend(canvas, width, height);

            // Stats text at bottom: "収縮期/拡張期 mmHg, nn bpm"
            DrawStatsText(canvas, height, systolicText, diastolicText, pulseText);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DrawChart error: {ex.Message}");
        }
    }

private void DrawStatsText(SKCanvas canvas, int height, string systolicText, string diastolicText, string pulseText)
    {
        var systolicValue = systolicText.Replace("収縮期 ", "").Replace(" mmHg", "");
        var diastolicValue = diastolicText.Replace("拡張期 ", "").Replace(" mmHg", "");
        var pulseValue = pulseText.Replace("脈拍 ", "").Replace(" bpm", "");
        
        var text = $"平均値 収縮期/拡張期 {systolicValue}/{diastolicValue} mmHg, {pulseValue} bpm";
        
        using var paint = new SKPaint { Color = TextColor, IsAntialias = true };
        canvas.DrawText(text, _margin, height - 5, SKTextAlign.Left, _textFont, new SKPaint { Color = TextColor, IsAntialias = true });
    }

    private void DrawSeries(SKCanvas canvas, List<double?> values, SKColor color, float plotLeft, float plotTop, float chartWidth, float chartHeight, double minValue, double maxValue, int pointCount, bool isMonthly)
    {
        if (values == null || values.Count == 0)
            return;

        using var linePaint = new SKPaint
        {
            Color = color,
            StrokeWidth = 3,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        var pointSize = isMonthly ? 3f : 6f;
        var pointInterval = isMonthly ? 3 : 1;

        using var pointPaint = new SKPaint
        {
            Color = color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using var pointStrokePaint = new SKPaint
        {
            Color = SKColors.White,
            StrokeWidth = 2,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        var pathBuilder = new SKPathBuilder();
        var firstPoint = true;
        var stepX = pointCount > 1 ? chartWidth / (pointCount - 1) : 0;

        for (int i = 0; i < values.Count; i++)
        {
            if (!values[i].HasValue)
                continue;

            var x = _margin + i * stepX;
            var normalizedY = (float)((values[i]!.Value - minValue) / (maxValue - minValue));
            var y = plotTop + chartHeight * (1 - normalizedY);

            if (firstPoint)
            {
                pathBuilder.MoveTo(x, y);
                firstPoint = false;
            }
            else
            {
                pathBuilder.LineTo(x, y);
            }
        }

        var path = pathBuilder.Detach();

        canvas.DrawPath(path, linePaint);

        // Draw points
        var lastValidIndex = -1;
        for (int i = values.Count - 1; i >= 0; i--)
        {
            if (values[i].HasValue)
            {
                lastValidIndex = i;
                break;
            }
        }

        for (int i = 0; i < values.Count; i += pointInterval)
        {
            if (!values[i].HasValue)
                continue;

            var x = _margin + i * stepX;
            var normalizedY = (float)((values[i]!.Value - minValue) / (maxValue - minValue));
            var y = plotTop + chartHeight * (1 - normalizedY);

            canvas.DrawCircle(x, y, pointSize + 4, pointStrokePaint);
            canvas.DrawCircle(x, y, pointSize, pointPaint);
        }

        // 月間の場合、最終有効ポイントが間引きで描画されていない場合は追加描画
        if (isMonthly && lastValidIndex >= 0 && lastValidIndex % pointInterval != 0)
        {
            var x = _margin + lastValidIndex * stepX;
            var normalizedY = (float)((values[lastValidIndex]!.Value - minValue) / (maxValue - minValue));
            var y = plotTop + chartHeight * (1 - normalizedY);

            canvas.DrawCircle(x, y, pointSize + 4, pointStrokePaint);
            canvas.DrawCircle(x, y, pointSize, pointPaint);
        }
    }

    private void DrawLegend(SKCanvas canvas, int width, int height)
    {
        try
        {
            using var legendPaint = new SKPaint { Color = TextColor, IsAntialias = true };

            using var legendBgPaint = new SKPaint
            {
                Color = ChartBackgroundColor.WithAlpha(204),
                IsAntialias = true
            };

            var legendItems = new[]
            {
                ("収縮期", SystolicColor),
                ("拡張期", DiastolicColor),
                ("脈拍", PulseColor)
            };

            var totalWidth = legendItems.Length * 80f + (legendItems.Length - 1) * 20f;
            var legendX = (width - totalWidth) / 2;

            var legendRect = new SKRect(-8, 0, width + 8, 40);
            canvas.DrawRoundRect(legendRect, 8, 8, new SKPaint { Color = ChartBackgroundColor.WithAlpha(204), IsAntialias = true });

            for (int i = 0; i < legendItems.Length; i++)
            {
                var itemX = (width - totalWidth) / 2 + i * (80f + 20f);
                var (label, color) = legendItems[i];

                canvas.DrawCircle(20 + i * 100, 20f, 6, new SKPaint { Color = color, IsAntialias = true });
                canvas.DrawText(label, 30 + i * 100, 28, SKTextAlign.Left, _legendFont, new SKPaint { Color = TextColor, IsAntialias = true });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DrawLegend error: {ex.Message}");
        }
    }
}