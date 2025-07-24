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
            // 同步加载设置
            LoadSettingsSync();
        }

        private void LoadSettingsSync()
        {
            try
            {
                var settingsJson = Preferences.Get(SETTINGS_KEY, string.Empty);
                if (!string.IsNullOrEmpty(settingsJson))
                {
                    _settings = JsonSerializer.Deserialize<NotificationSettings>(settingsJson) ?? new NotificationSettings();
                }
                System.Diagnostics.Debug.WriteLine($"设置加载完成: WorkStartMessage='{_settings.WorkStartMessage}'");
            }
            catch (Exception ex)
            {
                // 如果加载失败，使用默认设置
                _settings = new NotificationSettings();
                System.Diagnostics.Debug.WriteLine($"Failed to load notification settings: {ex.Message}");
            }
        }

        public Task LoadSettingsAsync()
        {
            LoadSettingsSync();
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
            System.Diagnostics.Debug.WriteLine("=== ShowWorkStartNotificationAsync 被调用 ===");
            System.Diagnostics.Debug.WriteLine($"WorkStartMessage: '{_settings.WorkStartMessage}'");
            System.Diagnostics.Debug.WriteLine($"VoiceReminderEnabled: {_settings.VoiceReminderEnabled}");
            System.Diagnostics.Debug.WriteLine($"SystemNotificationEnabled: {_settings.SystemNotificationEnabled}");
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
            await ShowNotificationAsync(message, "工作计时器");
        }

        private async Task ShowNotificationAsync(string message, string subtitle)
        {
            System.Diagnostics.Debug.WriteLine($"=== ShowNotificationAsync 被调用 ===");
            System.Diagnostics.Debug.WriteLine($"消息: {message}");
            System.Diagnostics.Debug.WriteLine($"副标题: {subtitle}");
            System.Diagnostics.Debug.WriteLine($"语音提醒启用: {_settings.VoiceReminderEnabled}");
            System.Diagnostics.Debug.WriteLine($"系统通知启用: {_settings.SystemNotificationEnabled}");
            System.Diagnostics.Debug.WriteLine($"音量: {_settings.VoiceVolume}");
            
            if (IsInDoNotDisturbPeriod())
            {
                System.Diagnostics.Debug.WriteLine("当前处于免打扰时段，跳过通知");
                return;
            }

            // 语音提醒
            if (_settings.VoiceReminderEnabled)
            {
                System.Diagnostics.Debug.WriteLine("开始语音提醒");
                await ShowVoiceNotificationAsync(message);
            }

            // 系统通知
            if (_settings.SystemNotificationEnabled)
            {
                System.Diagnostics.Debug.WriteLine("开始系统通知");
                await ShowSystemNotificationAsync(message, subtitle);
            }

            // 前台弹窗提醒
            if (_settings.ForegroundPopupEnabled)
            {
                System.Diagnostics.Debug.WriteLine("开始前台弹窗");
                await ShowForegroundPopupAsync(message);
            }

            // 桌面通知（在系统通知中实现）
            if (_settings.DesktopNotificationEnabled && !_settings.SystemNotificationEnabled)
            {
                System.Diagnostics.Debug.WriteLine("开始桌面通知");
                await ShowSystemNotificationAsync(message, subtitle);
            }
        }

        private async Task ShowVoiceNotificationAsync(string message)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== ShowVoiceNotificationAsync 开始 ===");
                System.Diagnostics.Debug.WriteLine($"消息: {message}");
                System.Diagnostics.Debug.WriteLine($"音量: {_settings.VoiceVolume}");
                
#if MACCATALYST
                // 在 macOS 上使用自定义的语音服务
                System.Diagnostics.Debug.WriteLine("使用 macOS 自定义语音服务");
                await WorkTimeTracker.UI.Platforms.MacCatalyst.MacTextToSpeech.SpeakAsync(message, (float)_settings.VoiceVolume);
#else
                // 在其他平台使用 MAUI 内置语音服务
                System.Diagnostics.Debug.WriteLine("使用 MAUI 内置语音服务");
                var speechSettings = new SpeechOptions
                {
                    Volume = (float)_settings.VoiceVolume
                };
                await TextToSpeech.SpeakAsync(message, speechSettings);
#endif
                System.Diagnostics.Debug.WriteLine("语音播放完成");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Voice notification failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"异常详情: {ex}");
                // 如果语音失败，回退到系统通知
                await ShowSystemNotificationAsync(message, "语音提醒失败");
            }
        }

        private async Task ShowSystemNotificationAsync(string message, string subtitle)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== ShowSystemNotificationAsync 开始 ===");
                System.Diagnostics.Debug.WriteLine($"消息: {message}");
                System.Diagnostics.Debug.WriteLine($"副标题: {subtitle}");

#if MACCATALYST
                // 在 macOS 上使用原生通知
                System.Diagnostics.Debug.WriteLine("使用 macOS 原生通知");
                var success = await WorkTimeTracker.UI.Platforms.MacCatalyst.MacNotificationHelper.ShowNotificationAsync(
                    "工作计时器", subtitle, message);
                
                if (!success)
                {
                    System.Diagnostics.Debug.WriteLine("macOS 原生通知失败，尝试 Plugin.LocalNotification");
                    await ShowPluginNotificationAsync(message, subtitle);
                }
#else
                // 在其他平台使用 Plugin.LocalNotification
                System.Diagnostics.Debug.WriteLine("使用 Plugin.LocalNotification");
                await ShowPluginNotificationAsync(message, subtitle);
#endif
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"System notification failed: {ex.Message}");
            }
        }

        private async Task ShowPluginNotificationAsync(string message, string subtitle)
        {
            try
            {
                var notification = new NotificationRequest
                {
                    NotificationId = DateTime.Now.GetHashCode(),
                    Title = "工作计时器",
                    Subtitle = subtitle,
                    Description = message,
                    BadgeNumber = 1,
                    CategoryType = NotificationCategoryType.Status
                };

                await LocalNotificationCenter.Current.Show(notification);
                System.Diagnostics.Debug.WriteLine("Plugin 通知已发送");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Plugin notification failed: {ex.Message}");
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
                            await currentPage.DisplayAlert("工作计时器", message, "确定");
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
