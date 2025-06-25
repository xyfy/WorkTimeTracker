using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using WorkTimeTracker.Core.Interfaces;

namespace WorkTimeTracker.UI.Services
{
    public class ReminderService
    {
        private readonly IWorkTimeService _workTimeService;
        private readonly INotificationService _notificationService;

        public event Action<TimeSpan>? OnTimeRemainingChanged;
        public event Action<string>? OnSegmentChanged;

        public ReminderService(IWorkTimeService workTimeService, INotificationService notificationService)
        {
            _workTimeService = workTimeService;
            _notificationService = notificationService;
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
            await _notificationService.ShowWorkStartNotificationAsync();
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
