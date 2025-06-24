using SQLite;
using System.Threading.Tasks;
using WorkTimeTracker.Core.Models;
using System.Collections.Generic;
using System;

namespace WorkTimeTracker.Data
{
    public class WorkRecordDatabase
    {
        readonly SQLiteAsyncConnection database;
        
        public WorkRecordDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<WorkRecord>().Wait();
        }
        
        public async Task<WorkRecord?> GetWorkRecordAsync(DateTime day)
        {
            var key = day.ToString("yyyy-MM-dd");
            return await database.Table<WorkRecord>().Where(r => r.Day == key).FirstOrDefaultAsync();
        }
        
        public Task<int> SaveWorkRecordAsync(WorkRecord record)
        {
            return database.InsertOrReplaceAsync(record);
        }

        public async Task<IEnumerable<WorkRecord>> GetAllWorkRecordsAsync()
        {
            return await database.Table<WorkRecord>().ToListAsync();
        }
    }
}
