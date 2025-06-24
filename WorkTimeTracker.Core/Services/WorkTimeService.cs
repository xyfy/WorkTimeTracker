using System;
using System.Threading;
using System.Threading.Tasks;
using WorkTimeTracker.Core.Interfaces;
using WorkTimeTracker.Core.Models;

namespace WorkTimeTracker.Core.Services
{
    public class WorkTimeService : IWorkTimeService
    {
        public TimeSpan ConfiguredWorkDuration { get; set; } = TimeSpan.FromMinutes(50);
        public TimeSpan ConfiguredRestDuration { get; set; } = TimeSpan.FromMinutes(10);

        private readonly IWorkRecordRepository _workRecordRepository;
        private int _lastRecordedSeconds = 0;

        private CancellationTokenSource? cancellationTokenSource;
        private DateTime periodEndTime;
        private TimeSpan currentSegmentDuration;
        private string currentSegmentName = string.Empty;

        private bool isWorking = false;
        private TimeSpan sessionWorkTime = TimeSpan.Zero;

        public bool IsWorking => isWorking;

        public event Action<TimeSpan>? OnTimeRemainingChanged;

        public WorkTimeService(IWorkRecordRepository workRecordRepository)
        {
            _workRecordRepository = workRecordRepository;
        }

        public async Task StartWorkAsync()
        {
            if (isWorking) return;
            isWorking = true;
            sessionWorkTime = TimeSpan.Zero;
            cancellationTokenSource = new CancellationTokenSource();
            await Task.Run(() => ProcessSegments(cancellationTokenSource.Token));
        }

        public async Task StopWorkAsync()
        {
            if (!isWorking) return;
            isWorking = false;
            cancellationTokenSource?.Cancel();
            await Task.CompletedTask;
        }

        public async Task<string> GetDailyWorkTimeAsync()
        {
            var record = await _workRecordRepository.GetWorkRecordAsync(DateTime.Now);
            if (record == null)
                return "00:00";
            TimeSpan ts = TimeSpan.FromSeconds(record.TotalWorkSeconds);
            return ts.ToString(@"hh\:mm");
        }

        private async Task ProcessSegments(CancellationToken token)
        {
            while (!token.IsCancellationRequested && isWorking)
            {
                // 工作倒计时周期
                currentSegmentName = "工作时间";
                currentSegmentDuration = ConfiguredWorkDuration;
                periodEndTime = DateTime.Now.Add(currentSegmentDuration);
                
                DateTime workSegmentStart = DateTime.Now;
                _lastRecordedSeconds = 0;
                
                while (DateTime.Now <= periodEndTime && !token.IsCancellationRequested)
                {
                    TimeSpan remaining = periodEndTime - DateTime.Now;
                    OnTimeRemainingChanged?.Invoke(remaining);
                    
                    int secondsElapsed = (int)(DateTime.Now - workSegmentStart).TotalSeconds;
                    if (secondsElapsed > _lastRecordedSeconds)
                    {
                        int delta = secondsElapsed - _lastRecordedSeconds;
                        _lastRecordedSeconds = secondsElapsed;
                        sessionWorkTime += TimeSpan.FromSeconds(delta);
                        await UpdateDBWorkRecordForWorkDeltaAsync(delta);
                    }
                    await Task.Delay(5000, token);
                }

                if (!isWorking) break;

                // 休息倒计时周期
                currentSegmentName = "休息时间";
                currentSegmentDuration = ConfiguredRestDuration;
                periodEndTime = DateTime.Now.Add(currentSegmentDuration);
                
                DateTime restSegmentStart = DateTime.Now;
                _lastRecordedSeconds = 0;
                
                while (DateTime.Now <= periodEndTime && !token.IsCancellationRequested)
                {
                    TimeSpan remaining = periodEndTime - DateTime.Now;
                    OnTimeRemainingChanged?.Invoke(remaining);
                    
                    int secondsElapsed = (int)(DateTime.Now - restSegmentStart).TotalSeconds;
                    if (secondsElapsed > _lastRecordedSeconds)
                    {
                        int delta = secondsElapsed - _lastRecordedSeconds;
                        _lastRecordedSeconds = secondsElapsed;
                        sessionWorkTime += TimeSpan.FromSeconds(delta);
                        await UpdateDBWorkRecordForRestDeltaAsync(delta);
                    }
                    await Task.Delay(5000, token);
                }
                
                if (!isWorking) break;
            }
        }

        private async Task UpdateDBWorkRecordForWorkDeltaAsync(int deltaSeconds)
        {
            var key = DateTime.Now.ToString("yyyy-MM-dd");
            var record = await _workRecordRepository.GetWorkRecordAsync(DateTime.Now);
            if (record == null)
            {
                record = new WorkRecord(key, deltaSeconds, 0);
            }
            else
            {
                record.WorkSeconds += deltaSeconds;
                record.TotalWorkSeconds = record.WorkSeconds + record.RestSeconds;
            }
            await _workRecordRepository.SaveWorkRecordAsync(record);
        }

        private async Task UpdateDBWorkRecordForRestDeltaAsync(int deltaSeconds)
        {
            var key = DateTime.Now.ToString("yyyy-MM-dd");
            var record = await _workRecordRepository.GetWorkRecordAsync(DateTime.Now);
            if (record == null)
            {
                record = new WorkRecord(key, 0, deltaSeconds);
            }
            else
            {
                record.RestSeconds += deltaSeconds;
                record.TotalWorkSeconds = record.WorkSeconds + record.RestSeconds;
            }
            await _workRecordRepository.SaveWorkRecordAsync(record);
        }
    }
}
