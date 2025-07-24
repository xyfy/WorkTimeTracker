using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Microsoft.Maui.LifecycleEvents;
using WorkTimeTracker.Data;
using WorkTimeTracker.Data.Repositories;
using WorkTimeTracker.Core.Interfaces;
using WorkTimeTracker.Core.Services;
using WorkTimeTracker.UI.Services;
using Plugin.LocalNotification;

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
		// 注册通知服务
		builder.Services.AddSingleton<WorkTimeTracker.Core.Interfaces.INotificationService, EnhancedNotificationService>();

		// 注册 UI 服务
		builder.Services.AddSingleton<ReminderService>();

		// 注册页面
		builder.Services.AddSingleton<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		
		// 初始化通知服务
		_ = Task.Run(async () =>
		{
			try
			{
				await LocalNotificationCenter.Current.RequestNotificationPermission();
				System.Diagnostics.Debug.WriteLine("通知权限请求完成");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"通知权限请求失败: {ex.Message}");
			}
		});
		
		return app;
	}
}
