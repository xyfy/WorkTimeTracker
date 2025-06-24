using System;
using System.Threading.Tasks;
using WorkTimeTracker.Core.Models;
using System.Collections.Generic;

namespace WorkTimeTracker.Core.Interfaces
{
    public interface IWorkRecordRepository
    {
        Task<WorkRecord?> GetWorkRecordAsync(DateTime day);
        Task<int> SaveWorkRecordAsync(WorkRecord record);
        Task<IEnumerable<WorkRecord>> GetAllWorkRecordsAsync();
    }
}
