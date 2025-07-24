using System;
using System.Threading.Tasks;

namespace WorkTimeTracker.Core.Interfaces
{
    public interface IWorkTimeService
    {
        TimeSpan ConfiguredWorkDuration { get; set; }
        TimeSpan ConfiguredRestDuration { get; set; }
        bool IsWorking { get; }
        
        event Action<TimeSpan> OnTimeRemainingChanged;
        event Action<string> OnSegmentCompleted; // 新增：段落完成事件
        
        Task StartWorkAsync();
        Task StopWorkAsync();
        Task<string> GetDailyWorkTimeAsync();
    }
}
