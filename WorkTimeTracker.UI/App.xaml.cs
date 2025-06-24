using Microsoft.Extensions.DependencyInjection;
using WorkTimeTracker.UI.Services;

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

    protected override void OnStart()
    {
        base.OnStart();
        //_reminderService.Start();
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
        return new Window(new NavigationPage(new MainPage(reminderService)));
    }
}