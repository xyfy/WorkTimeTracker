using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using WorkTimeTracker.Core.Interfaces;

namespace WorkTimeTracker.UI.Services
{
    public class ReminderService
    {
        private readonly IWorkTimeService _workTimeService;

        public event Action<TimeSpan>? OnTimeRemainingChanged;
        public event Action<string>? OnSegmentChanged;

        public ReminderService(IWorkTimeService workTimeService)
        {
            _workTimeService = workTimeService;
            _workTimeService.OnTimeRemainingChanged += (timeSpan) =>
            {
                OnTimeRemainingChanged?.Invoke(timeSpan);
            };
        }

        public TimeSpan ConfiguredWorkDuration 
        { 
            get => _workTimeService.ConfiguredWorkDuration;
            set => _workTimeService.ConfiguredWorkDuration = value;
        }

        public TimeSpan ConfiguredRestDuration 
        { 
            get => _workTimeService.ConfiguredRestDuration;
            set => _workTimeService.ConfiguredRestDuration = value;
        }

        public bool IsWorking => _workTimeService.IsWorking;

        public async Task StartWorkAsync()
        {
            await _workTimeService.StartWorkAsync();
            await SpeakAsync("开始工作");
        }

        public async Task StopWorkAsync()
        {
            await _workTimeService.StopWorkAsync();
            await SpeakAsync("工作结束");
        }

        public async Task<string> GetDailyWorkTimeAsync()
        {
            return await _workTimeService.GetDailyWorkTimeAsync();
        }

        public async Task SpeakAsync(string text)
        {
            try
            {
                await TextToSpeech.SpeakAsync(text);
            }
            catch
            {
                // 如果 TTS 失败，使用本地通知作为备选
                var notification = new NotificationRequest
                {
                    NotificationId = 1001,
                    Title = "WorkTimeTracker",
                    Subtitle = text,
                    Description = text,
                    BadgeNumber = 1
                };
                await LocalNotificationCenter.Current.Show(notification);
            }
        }

        public void StartWork()
        {
            _ = Task.Run(async () => await StartWorkAsync());
        }

        public void EndWork()
        {
            _ = Task.Run(async () => await StopWorkAsync());
        }

        public void ResetTimer()
        {
            // 实现重置计时器逻辑 - 停止当前工作
            _ = Task.Run(async () => await StopWorkAsync());
        }
    }
}
