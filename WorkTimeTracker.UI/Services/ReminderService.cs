using Microsoft.Maui.Controls;
using WorkTimeTracker.Core.Interfaces;

namespace WorkTimeTracker.UI.Services
{
    public class ReminderService
    {
        private readonly IWorkTimeService _workTimeService;
        private readonly INotificationService _notificationService;

        public event Action<TimeSpan>? OnTimeRemainingChanged;
        public event Action<string>? OnSegmentCompleted; // 段落完成事件

        public ReminderService(IWorkTimeService workTimeService, INotificationService notificationService)
        {
            _workTimeService = workTimeService;
            _notificationService = notificationService;
            _workTimeService.OnTimeRemainingChanged += (timeSpan) =>
            {
                OnTimeRemainingChanged?.Invoke(timeSpan);
            };
            _workTimeService.OnSegmentCompleted += async (message) =>
            {
                OnSegmentCompleted?.Invoke(message);
                // 自动发送通知和语音播报
                if (_notificationService != null)
                {
                    await _notificationService.ShowCustomNotificationAsync(message);
                }
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
            try
            {
                await _workTimeService.StartWorkAsync();
                
                if (_notificationService != null)
                {
                    await _notificationService.ShowWorkStartNotificationAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ReminderService.StartWorkAsync 异常: {ex.Message}");
                throw;
            }
        }

        public async Task StopWorkAsync()
        {
            await _workTimeService.StopWorkAsync();
            await _notificationService.ShowWorkEndNotificationAsync();
        }

        public async Task<string> GetDailyWorkTimeAsync()
        {
            return await _workTimeService.GetDailyWorkTimeAsync();
        }

        public async Task SpeakAsync(string text)
        {
            await _notificationService.ShowCustomNotificationAsync(text);
        }

        public void StartWork()
        {
            try
            {
                _ = Task.Run(async () => await StartWorkAsync());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StartWork 同步方法异常: {ex.Message}");
                throw;
            }
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
