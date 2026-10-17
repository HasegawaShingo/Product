using Microsoft.Extensions.Logging;
using BloodPressureTracker.Services;
using BloodPressureTracker.ViewModels;
using BloodPressureTracker.Views;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace BloodPressureTracker;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "bloodpressure.db3");
		builder.Services.AddSingleton(new DatabaseService(dbPath));
		builder.Services.AddTransient<RecordViewModel>();
		builder.Services.AddTransient<ListViewModel>();
		builder.Services.AddTransient<StatisticsViewModel>();
		builder.Services.AddTransient<RecordPage>();
		builder.Services.AddTransient<ListPage>();
		builder.Services.AddTransient<StatisticsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}