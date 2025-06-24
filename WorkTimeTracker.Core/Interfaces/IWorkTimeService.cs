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
        
        Task StartWorkAsync();
        Task StopWorkAsync();
        Task<string> GetDailyWorkTimeAsync();
    }
}
