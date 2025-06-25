using Microsoft.Maui.Controls;
using WorkTimeTracker.Core.Interfaces;
using WorkTimeTracker.Core.Models;

namespace WorkTimeTracker.UI
{
    public partial class NotificationSettingsPage : ContentPage
    {
        private readonly INotificationService _notificationService;

        public NotificationSettingsPage(INotificationService notificationService)
        {
            InitializeComponent();
            _notificationService = notificationService;
            
            LoadSettings();
            SetupEventHandlers();
        }

        private void LoadSettings()
        {
            var settings = _notificationService.Settings;

            // 加载提醒方式设置
            VoiceReminderCheckBox.IsChecked = settings.VoiceReminderEnabled;
            SystemNotificationCheckBox.IsChecked = settings.SystemNotificationEnabled;
            ForegroundPopupCheckBox.IsChecked = settings.ForegroundPopupEnabled;
            DesktopNotificationCheckBox.IsChecked = settings.DesktopNotificationEnabled;

            // 加载语音设置
            VolumeSlider.Value = settings.VoiceVolume;
            VolumeLabel.Text = $"{(int)(settings.VoiceVolume * 100)}%";
            LanguagePicker.SelectedItem = settings.VoiceLanguage;

            // 加载免打扰设置
            DoNotDisturbCheckBox.IsChecked = settings.DoNotDisturbEnabled;
            DoNotDisturbStartTimePicker.Time = settings.DoNotDisturbStartTime;
            DoNotDisturbEndTimePicker.Time = settings.DoNotDisturbEndTime;
            DoNotDisturbTimeLayout.IsVisible = settings.DoNotDisturbEnabled;

            // 加载提醒频率
            ReminderFrequencyEntry.Text = settings.ReminderFrequencyMinutes.ToString();

            // 加载自定义消息
            WorkStartMessageEntry.Text = settings.WorkStartMessage;
            WorkEndMessageEntry.Text = settings.WorkEndMessage;
            RestStartMessageEntry.Text = settings.RestStartMessage;
            RestEndMessageEntry.Text = settings.RestEndMessage;
        }

        private void SetupEventHandlers()
        {
            // 音量滑块事件
            VolumeSlider.ValueChanged += (s, e) =>
            {
                VolumeLabel.Text = $"{(int)(e.NewValue * 100)}%";
            };

            // 免打扰复选框事件
            DoNotDisturbCheckBox.CheckedChanged += (s, e) =>
            {
                DoNotDisturbTimeLayout.IsVisible = e.Value;
            };
        }

        private async void OnSaveSettingsClicked(object sender, EventArgs e)
        {
            try
            {
                var settings = _notificationService.Settings;

                // 保存提醒方式设置
                settings.VoiceReminderEnabled = VoiceReminderCheckBox.IsChecked;
                settings.SystemNotificationEnabled = SystemNotificationCheckBox.IsChecked;
                settings.ForegroundPopupEnabled = ForegroundPopupCheckBox.IsChecked;
                settings.DesktopNotificationEnabled = DesktopNotificationCheckBox.IsChecked;

                // 保存语音设置
                settings.VoiceVolume = VolumeSlider.Value;
                settings.VoiceLanguage = LanguagePicker.SelectedItem?.ToString() ?? "zh-CN";

                // 保存免打扰设置
                settings.DoNotDisturbEnabled = DoNotDisturbCheckBox.IsChecked;
                settings.DoNotDisturbStartTime = DoNotDisturbStartTimePicker.Time;
                settings.DoNotDisturbEndTime = DoNotDisturbEndTimePicker.Time;

                // 保存提醒频率
                if (int.TryParse(ReminderFrequencyEntry.Text, out int frequency))
                {
                    settings.ReminderFrequencyMinutes = Math.Max(1, frequency);
                }

                // 保存自定义消息
                settings.WorkStartMessage = WorkStartMessageEntry.Text?.Trim() ?? "开始工作";
                settings.WorkEndMessage = WorkEndMessageEntry.Text?.Trim() ?? "工作结束";
                settings.RestStartMessage = RestStartMessageEntry.Text?.Trim() ?? "开始休息";
                settings.RestEndMessage = RestEndMessageEntry.Text?.Trim() ?? "休息结束";

                await _notificationService.SaveSettingsAsync();

                await DisplayAlert("成功", "设置已保存", "确定");
            }
            catch (Exception ex)
            {
                await DisplayAlert("错误", $"保存设置时出错：{ex.Message}", "确定");
            }
        }

        private async void OnTestNotificationClicked(object sender, EventArgs e)
        {
            try
            {
                await _notificationService.ShowCustomNotificationAsync("这是一条测试通知");
                await DisplayAlert("测试", "测试通知已发送", "确定");
            }
            catch (Exception ex)
            {
                await DisplayAlert("错误", $"发送测试通知时出错：{ex.Message}", "确定");
            }
        }

        private async void OnResetToDefaultClicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("确认", "确定要重置为默认设置吗？", "确定", "取消");
            if (result)
            {
                // 重置为默认设置
                var settings = _notificationService.Settings;
                
                settings.VoiceReminderEnabled = true;
                settings.SystemNotificationEnabled = true;
                settings.ForegroundPopupEnabled = false;
                settings.DesktopNotificationEnabled = true;
                settings.VoiceVolume = 0.8;
                settings.VoiceLanguage = "zh-CN";
                settings.DoNotDisturbEnabled = false;
                settings.DoNotDisturbStartTime = new TimeSpan(22, 0, 0);
                settings.DoNotDisturbEndTime = new TimeSpan(8, 0, 0);
                settings.ReminderFrequencyMinutes = 5;
                settings.WorkStartMessage = "开始工作";
                settings.WorkEndMessage = "工作结束";
                settings.RestStartMessage = "开始休息";
                settings.RestEndMessage = "休息结束";

                await _notificationService.SaveSettingsAsync();
                LoadSettings();

                await DisplayAlert("成功", "已重置为默认设置", "确定");
            }
        }
    }
}
