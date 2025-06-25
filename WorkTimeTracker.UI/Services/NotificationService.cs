using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using WorkTimeTracker.Core.Models;
using System.Text.Json;
using INotificationService = WorkTimeTracker.Core.Interfaces.INotificationService;

namespace WorkTimeTracker.UI.Services
{
    public class EnhancedNotificationService : INotificationService
    {
        private const string SETTINGS_KEY = "NotificationSettings";
        private NotificationSettings _settings;

        public NotificationSettings Settings => _settings;

        public EnhancedNotificationService()
        {
            _settings = new NotificationSettings();
            _ = LoadSettingsAsync();
        }

        public Task LoadSettingsAsync()
        {
            try
            {
                var settingsJson = Preferences.Get(SETTINGS_KEY, string.Empty);
                if (!string.IsNullOrEmpty(settingsJson))
                {
                    _settings = JsonSerializer.Deserialize<NotificationSettings>(settingsJson) ?? new NotificationSettings();
                }
            }
            catch (Exception ex)
            {
                // 如果加载失败，使用默认设置
                _settings = new NotificationSettings();
                System.Diagnostics.Debug.WriteLine($"Failed to load notification settings: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        public Task SaveSettingsAsync()
        {
            try
            {
                var settingsJson = JsonSerializer.Serialize(_settings);
                Preferences.Set(SETTINGS_KEY, settingsJson);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save notification settings: {ex.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        public async Task ShowWorkStartNotificationAsync()
        {
            await ShowNotificationAsync(_settings.WorkStartMessage, "开始工作周期");
        }

        public async Task ShowWorkEndNotificationAsync()
        {
            await ShowNotificationAsync(_settings.WorkEndMessage, "工作周期结束");
        }

        public async Task ShowRestStartNotificationAsync()
        {
            await ShowNotificationAsync(_settings.RestStartMessage, "开始休息周期");
        }

        public async Task ShowRestEndNotificationAsync()
        {
            await ShowNotificationAsync(_settings.RestEndMessage, "休息周期结束");
        }

        public async Task ShowCustomNotificationAsync(string message)
        {
            await ShowNotificationAsync(message, "WorkTimeTracker");
        }

        private async Task ShowNotificationAsync(string message, string subtitle)
        {
            if (IsInDoNotDisturbPeriod())
                return;

            // 语音提醒
            if (_settings.VoiceReminderEnabled)
            {
                await ShowVoiceNotificationAsync(message);
            }

            // 系统通知
            if (_settings.SystemNotificationEnabled)
            {
                await ShowSystemNotificationAsync(message, subtitle);
            }

            // 前台弹窗提醒
            if (_settings.ForegroundPopupEnabled)
            {
                await ShowForegroundPopupAsync(message);
            }

            // 桌面通知（在系统通知中实现）
            if (_settings.DesktopNotificationEnabled && !_settings.SystemNotificationEnabled)
            {
                await ShowSystemNotificationAsync(message, subtitle);
            }
        }

        private async Task ShowVoiceNotificationAsync(string message)
        {
            try
            {
                var speechSettings = new SpeechOptions
                {
                    Volume = (float)_settings.VoiceVolume
                };

                await TextToSpeech.SpeakAsync(message, speechSettings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Voice notification failed: {ex.Message}");
                // 如果语音失败，回退到系统通知
                await ShowSystemNotificationAsync(message, "语音提醒失败");
            }
        }

        private async Task ShowSystemNotificationAsync(string message, string subtitle)
        {
            try
            {
                var notification = new NotificationRequest
                {
                    NotificationId = DateTime.Now.GetHashCode(),
                    Title = "WorkTimeTracker",
                    Subtitle = subtitle,
                    Description = message,
                    BadgeNumber = 1,
                    CategoryType = NotificationCategoryType.Status
                };

                await LocalNotificationCenter.Current.Show(notification);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"System notification failed: {ex.Message}");
            }
        }

        private async Task ShowForegroundPopupAsync(string message)
        {
            try
            {
                // 在主线程上显示弹窗
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var app = Application.Current;
                    if (app?.Windows?.Count > 0)
                    {
                        var currentPage = app.Windows[0].Page;
                        if (currentPage != null)
                        {
                            await currentPage.DisplayAlert("WorkTimeTracker", message, "确定");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Foreground popup failed: {ex.Message}");
            }
        }

        public bool IsInDoNotDisturbPeriod()
        {
            if (!_settings.DoNotDisturbEnabled)
                return false;

            var now = DateTime.Now.TimeOfDay;
            var start = _settings.DoNotDisturbStartTime;
            var end = _settings.DoNotDisturbEndTime;

            // 处理跨天的情况（例如 22:00 到次日 08:00）
            if (start > end)
            {
                return now >= start || now <= end;
            }
            else
            {
                return now >= start && now <= end;
            }
        }
    }
}
