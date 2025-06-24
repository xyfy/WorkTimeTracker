using Microsoft.Extensions.DependencyInjection;
using WorkTimeTracker.UI.Services;
using WorkTimeTracker.Data;

namespace WorkTimeTracker.UI;

public partial class MainPage : ContentPage
{
    private readonly ReminderService _reminderService;
    private System.Threading.Timer? _timer;  // 使用问号声明，可为 null

    public MainPage(ReminderService reminderService)
    {
        InitializeComponent();
        _reminderService = reminderService;
        _reminderService.OnTimeRemainingChanged += UpdateRemainingTime;
    }

    private void UpdateRemainingTime(TimeSpan remainingTime)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            RemainingTimeLabel.Text = remainingTime.ToString(@"mm\:ss");
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // 加载配置
        WorkDurationEntry.Text = Preferences.Get("WorkDuration", "50");
        RestDurationEntry.Text = Preferences.Get("RestDuration", "10");

        // 计算距离下一整分钟的延迟
        DateTime now = DateTime.Now;
        DateTime nextMinute = now.AddMinutes(1).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        TimeSpan initialDelay = nextMinute - now;

        _timer = new System.Threading.Timer(async state =>
        {
            var time = await _reminderService.GetDailyWorkTimeAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TotalWorkTimeLabel.Text = time;
            });
        }, null, initialDelay, TimeSpan.FromMinutes(1));
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer?.Dispose();
    }

    private async void OnToggleWork(object sender, EventArgs e)
    {
        if (!_reminderService.IsWorking)
        {
            // 工作开始前允许编辑配置，取出输入的工作/休息分钟数，并赋值给服务
            if (int.TryParse(WorkDurationEntry.Text, out int workMins) &&
                int.TryParse(RestDurationEntry.Text, out int restMins))
            {
                _reminderService.ConfiguredWorkDuration = TimeSpan.FromMinutes(workMins);
                _reminderService.ConfiguredRestDuration = TimeSpan.FromMinutes(restMins);
                // 保存配置，以便下次启动时加载
                Preferences.Set("WorkDuration", WorkDurationEntry.Text);
                Preferences.Set("RestDuration", RestDurationEntry.Text);
            }
            // 锁定配置，禁止编辑
            WorkDurationEntry.IsEnabled = false;
            RestDurationEntry.IsEnabled = false;

            _reminderService.StartWork();
            ToggleWorkButton.Text = "结束工作";
        }
        else
        {
            _reminderService.EndWork();
            ToggleWorkButton.Text = "开始工作";
            // 解除配置锁定
            WorkDurationEntry.IsEnabled = true;
            RestDurationEntry.IsEnabled = true;
            TotalWorkTimeLabel.Text = await _reminderService.GetDailyWorkTimeAsync();
        }
    }

    // 保留重置按钮（如需要）
    private void OnResetTimer(object sender, EventArgs e)
    {
        _reminderService.ResetTimer();
    }

    // 修改事件处理方法，通过 App.Current 获取依赖服务
    private async void OnViewScheduleButtonClicked(object sender, EventArgs e)
    {
        var workRecordDb = (Application.Current as App)?.Services.GetService<WorkRecordDatabase>();
        if (workRecordDb != null)
        {
            await Navigation.PushAsync(new SchedulePage(workRecordDb));
        }
    }
}

