using Microsoft.Extensions.DependencyInjection;
using WorkTimeTracker.UI.Services;
using Plugin.LocalNotification;

namespace WorkTimeTracker.UI;

public partial class App : Application
{
    // 新增属性，用于保存 IServiceProvider
    public IServiceProvider Services { get; }

    // 修改构造函数，接收 IServiceProvider 参数
    public App(IServiceProvider services)
    {
        InitializeComponent();
        Services = services;
        var reminderService = services.GetService<ReminderService>();
        if (reminderService == null)
        {
            throw new ArgumentNullException(nameof(reminderService), "ReminderService cannot be null");
        }
    }

    protected override async void OnStart()
    {
        base.OnStart();
        
        // 请求通知权限
        try
        {
#if MACCATALYST
            // macOS 特定的权限请求
            System.Diagnostics.Debug.WriteLine("请求 macOS 通知权限");
            await WorkTimeTracker.UI.Platforms.MacCatalyst.MacNotificationHelper.RequestPermissionAsync();
#endif
            
            // 通用的权限请求
            System.Diagnostics.Debug.WriteLine("请求通用通知权限");
            await LocalNotificationCenter.Current.RequestNotificationPermission();
            System.Diagnostics.Debug.WriteLine("通知权限请求完成");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"通知权限请求失败: {ex.Message}");
        }
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        //_reminderService.Stop();
    }

    protected override void OnResume()
    {
        base.OnResume();
        //_reminderService.Start();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var reminderService = Services.GetService<ReminderService>();
        if (reminderService == null)
            throw new InvalidOperationException("ReminderService not found in services");
            
        return new Window(new NavigationPage(new MainPage(reminderService)));
    }
}