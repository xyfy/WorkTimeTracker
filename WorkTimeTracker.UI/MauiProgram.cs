using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Microsoft.Maui.LifecycleEvents;
using WorkTimeTracker.Data;
using WorkTimeTracker.Data.Repositories;
using WorkTimeTracker.Core.Interfaces;
using WorkTimeTracker.Core.Services;
using WorkTimeTracker.UI.Services;

namespace WorkTimeTracker.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureEssentials(essentials =>
			{
				essentials.UseVersionTracking();
			});

		// 注册数据库
		builder.Services.AddSingleton<WorkRecordDatabase>(s =>
		{
			var dbPath = Path.Combine(FileSystem.AppDataDirectory, "workrecord.db3");
			return new WorkRecordDatabase(dbPath);
		});

		// 注册仓库
		builder.Services.AddSingleton<IWorkRecordRepository, WorkRecordRepository>();

		// 注册核心服务
		builder.Services.AddSingleton<IWorkTimeService, WorkTimeService>();

		// 注册 UI 服务
		builder.Services.AddSingleton<ReminderService>();

		// 注册页面
		builder.Services.AddSingleton<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		return app;
	}
}
