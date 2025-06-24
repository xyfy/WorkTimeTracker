using System;
using System.Threading.Tasks;
using WorkTimeTracker.Core.Interfaces;
using WorkTimeTracker.Core.Models;
using System.Collections.Generic;

namespace WorkTimeTracker.Data.Repositories
{
    public class WorkRecordRepository : IWorkRecordRepository
    {
        private readonly WorkRecordDatabase _database;

        public WorkRecordRepository(WorkRecordDatabase database)
        {
            _database = database;
        }

        public async Task<WorkRecord?> GetWorkRecordAsync(DateTime day)
        {
            return await _database.GetWorkRecordAsync(day);
        }

        public async Task<int> SaveWorkRecordAsync(WorkRecord record)
        {
            return await _database.SaveWorkRecordAsync(record);
        }

        public async Task<IEnumerable<WorkRecord>> GetAllWorkRecordsAsync()
        {
            return await _database.GetAllWorkRecordsAsync();
        }
    }
}
