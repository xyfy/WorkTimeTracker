using Xunit;
using WorkTimeTracker.Data;
using WorkTimeTracker.Data.Repositories;
using WorkTimeTracker.Core.Models;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace WorkTimeTracker.Tests
{
    public class WorkRecordRepositoryTests
    {
        private string GetTestDatabasePath()
        {
            return Path.Combine(Path.GetTempPath(), $"test_workrecords_{Guid.NewGuid()}.db");
        }

        [Fact]
        public async Task SaveWorkRecordAsync_NewRecord_ShouldInsert()
        {
            // Arrange
            var dbPath = GetTestDatabasePath();
            var database = new WorkRecordDatabase(dbPath);
            var repository = new WorkRecordRepository(database);
            
            var record = new WorkRecord
            {
                Day = DateTime.Today.ToString("yyyy-MM-dd"),
                WorkSeconds = 3600,
                RestSeconds = 600,
                TotalWorkSeconds = 3600
            };

            try
            {
                // Act
                await repository.SaveWorkRecordAsync(record);

                // Assert
                var savedRecord = await repository.GetWorkRecordAsync(DateTime.Today);
                Assert.NotNull(savedRecord);
                Assert.Equal(record.Day, savedRecord.Day);
                Assert.Equal(record.WorkSeconds, savedRecord.WorkSeconds);
                Assert.Equal(record.RestSeconds, savedRecord.RestSeconds);
                Assert.Equal(record.TotalWorkSeconds, savedRecord.TotalWorkSeconds);
            }
            finally
            {
                // Cleanup
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }

        [Fact]
        public async Task GetWorkRecordAsync_NonExistentRecord_ShouldReturnNull()
        {
            // Arrange
            var dbPath = GetTestDatabasePath();
            var database = new WorkRecordDatabase(dbPath);
            var repository = new WorkRecordRepository(database);
            var testDate = new DateTime(2020, 1, 1); // 一个肯定不存在的日期

            try
            {
                // Act
                var result = await repository.GetWorkRecordAsync(testDate);

                // Assert
                Assert.Null(result);
            }
            finally
            {
                // Cleanup
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }

        [Fact]
        public async Task SaveWorkRecordAsync_UpdateExistingRecord_ShouldUpdate()
        {
            // Arrange
            var dbPath = GetTestDatabasePath();
            var database = new WorkRecordDatabase(dbPath);
            var repository = new WorkRecordRepository(database);
            
            var originalRecord = new WorkRecord
            {
                Day = DateTime.Today.ToString("yyyy-MM-dd"),
                WorkSeconds = 1800,
                RestSeconds = 300,
                TotalWorkSeconds = 1800
            };

            var updatedRecord = new WorkRecord
            {
                Day = DateTime.Today.ToString("yyyy-MM-dd"),
                WorkSeconds = 3600,
                RestSeconds = 600,
                TotalWorkSeconds = 3600
            };

            try
            {
                // Act
                await repository.SaveWorkRecordAsync(originalRecord);
                await repository.SaveWorkRecordAsync(updatedRecord); // 应该更新而不是插入

                // Assert
                var savedRecord = await repository.GetWorkRecordAsync(DateTime.Today);
                Assert.NotNull(savedRecord);
                Assert.Equal(updatedRecord.WorkSeconds, savedRecord.WorkSeconds);
                Assert.Equal(updatedRecord.RestSeconds, savedRecord.RestSeconds);
                Assert.Equal(updatedRecord.TotalWorkSeconds, savedRecord.TotalWorkSeconds);
            }
            finally
            {
                // Cleanup
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }

        [Fact]
        public async Task GetAllWorkRecordsAsync_EmptyDatabase_ShouldReturnEmptyList()
        {
            // Arrange
            var dbPath = GetTestDatabasePath();
            var database = new WorkRecordDatabase(dbPath);
            var repository = new WorkRecordRepository(database);

            try
            {
                // Act
                var records = await repository.GetAllWorkRecordsAsync();

                // Assert
                Assert.NotNull(records);
                Assert.Empty(records);
            }
            finally
            {
                // Cleanup
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }

        [Fact]
        public async Task GetAllWorkRecordsAsync_WithRecords_ShouldReturnAllRecords()
        {
            // Arrange
            var dbPath = GetTestDatabasePath();
            var database = new WorkRecordDatabase(dbPath);
            var repository = new WorkRecordRepository(database);
            
            var record1 = new WorkRecord
            {
                Day = DateTime.Today.ToString("yyyy-MM-dd"),
                WorkSeconds = 3600,
                RestSeconds = 600,
                TotalWorkSeconds = 3600
            };

            var record2 = new WorkRecord
            {
                Day = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
                WorkSeconds = 7200,
                RestSeconds = 1200,
                TotalWorkSeconds = 7200
            };

            try
            {
                // Act
                await repository.SaveWorkRecordAsync(record1);
                await repository.SaveWorkRecordAsync(record2);
                var records = await repository.GetAllWorkRecordsAsync();

                // Assert
                Assert.NotNull(records);
                Assert.Equal(2, records.Count());
            }
            finally
            {
                // Cleanup
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }
    }
}
