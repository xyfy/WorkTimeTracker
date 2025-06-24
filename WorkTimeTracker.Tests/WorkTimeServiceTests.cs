using Xunit;
using Moq;
using WorkTimeTracker.Core.Services;
using WorkTimeTracker.Core.Interfaces;
using WorkTimeTracker.Core.Models;
using System;
using System.Threading.Tasks;

namespace WorkTimeTracker.Tests
{
    public class WorkTimeServiceTests
    {
        [Fact]
        public async Task GetDailyWorkTimeAsync_NoRecord_Returns0000()
        {
            // Arrange
            var mockRepo = new Mock<IWorkRecordRepository>();
            mockRepo.Setup(x => x.GetWorkRecordAsync(It.IsAny<DateTime>()))
                   .ReturnsAsync((WorkRecord?)null);
            
            var service = new WorkTimeService(mockRepo.Object);

            // Act
            var result = await service.GetDailyWorkTimeAsync();

            // Assert
            Assert.Equal("00:00", result);
        }

        [Fact]
        public async Task GetDailyWorkTimeAsync_WithRecord_ReturnsFormattedTime()
        {
            // Arrange
            var mockRepo = new Mock<IWorkRecordRepository>();
            var record = new WorkRecord
            {
                Day = DateTime.Now.ToString("yyyy-MM-dd"),
                TotalWorkSeconds = 3661 // 1 hour, 1 minute, 1 second
            };
            
            mockRepo.Setup(x => x.GetWorkRecordAsync(It.IsAny<DateTime>()))
                   .ReturnsAsync(record);
            
            var service = new WorkTimeService(mockRepo.Object);

            // Act
            var result = await service.GetDailyWorkTimeAsync();

            // Assert
            Assert.Equal("01:01", result);
        }

        [Fact]
        public void IsWorking_InitialState_ReturnsFalse()
        {
            // Arrange
            var mockRepo = new Mock<IWorkRecordRepository>();
            var service = new WorkTimeService(mockRepo.Object);

            // Act & Assert
            Assert.False(service.IsWorking);
        }
    }
}
