using Xunit;
using Moq;
using WorkTimeTracker.UI.Services;
using WorkTimeTracker.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace WorkTimeTracker.Tests
{
    public class ReminderServiceTests
    {
        [Fact]
        public void ReminderService_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockWorkTimeService = new Mock<IWorkTimeService>();
            
            // Act
            var reminderService = new ReminderService(mockWorkTimeService.Object);

            // Assert
            Assert.NotNull(reminderService);
            Assert.False(reminderService.IsWorking);
        }

        [Fact]
        public async Task StartWorkAsync_ShouldCallUnderlyingService()
        {
            // Arrange
            var mockWorkTimeService = new Mock<IWorkTimeService>();
            var reminderService = new ReminderService(mockWorkTimeService.Object);

            // Act
            await reminderService.StartWorkAsync();

            // Assert
            mockWorkTimeService.Verify(x => x.StartWorkAsync(), Times.Once);
        }

        [Fact]
        public async Task StopWorkAsync_ShouldCallUnderlyingService()
        {
            // Arrange
            var mockWorkTimeService = new Mock<IWorkTimeService>();
            var reminderService = new ReminderService(mockWorkTimeService.Object);

            // Act
            await reminderService.StopWorkAsync();

            // Assert
            mockWorkTimeService.Verify(x => x.StopWorkAsync(), Times.Once);
        }

        [Fact]
        public async Task GetDailyWorkTimeAsync_ShouldReturnFromUnderlyingService()
        {
            // Arrange
            var mockWorkTimeService = new Mock<IWorkTimeService>();
            var expectedTime = "02:30";
            mockWorkTimeService.Setup(x => x.GetDailyWorkTimeAsync())
                              .ReturnsAsync(expectedTime);
            
            var reminderService = new ReminderService(mockWorkTimeService.Object);

            // Act
            var result = await reminderService.GetDailyWorkTimeAsync();

            // Assert
            Assert.Equal(expectedTime, result);
        }

        [Fact]
        public void ConfiguredWorkDuration_GetSet_ShouldWorkCorrectly()
        {
            // Arrange
            var mockWorkTimeService = new Mock<IWorkTimeService>();
            var reminderService = new ReminderService(mockWorkTimeService.Object);
            var testDuration = TimeSpan.FromMinutes(45);

            // Act
            reminderService.ConfiguredWorkDuration = testDuration;

            // Assert
            mockWorkTimeService.VerifySet(x => x.ConfiguredWorkDuration = testDuration, Times.Once);
        }

        [Fact]
        public void ConfiguredRestDuration_GetSet_ShouldWorkCorrectly()
        {
            // Arrange
            var mockWorkTimeService = new Mock<IWorkTimeService>();
            var reminderService = new ReminderService(mockWorkTimeService.Object);
            var testDuration = TimeSpan.FromMinutes(15);

            // Act
            reminderService.ConfiguredRestDuration = testDuration;

            // Assert
            mockWorkTimeService.VerifySet(x => x.ConfiguredRestDuration = testDuration, Times.Once);
        }

        [Fact]
        public void StartWork_ShouldNotThrow()
        {
            // Arrange
            var mockWorkTimeService = new Mock<IWorkTimeService>();
            var reminderService = new ReminderService(mockWorkTimeService.Object);

            // Act & Assert
            var exception = Record.Exception(() => reminderService.StartWork());
            Assert.Null(exception);
        }

        [Fact]
        public void EndWork_ShouldNotThrow()
        {
            // Arrange
            var mockWorkTimeService = new Mock<IWorkTimeService>();
            var reminderService = new ReminderService(mockWorkTimeService.Object);

            // Act & Assert
            var exception = Record.Exception(() => reminderService.EndWork());
            Assert.Null(exception);
        }

        [Fact]
        public void ResetTimer_ShouldNotThrow()
        {
            // Arrange
            var mockWorkTimeService = new Mock<IWorkTimeService>();
            var reminderService = new ReminderService(mockWorkTimeService.Object);

            // Act & Assert
            var exception = Record.Exception(() => reminderService.ResetTimer());
            Assert.Null(exception);
        }

        [Fact]
        public async Task SpeakAsync_ShouldNotThrow()
        {
            // Arrange
            var mockWorkTimeService = new Mock<IWorkTimeService>();
            var reminderService = new ReminderService(mockWorkTimeService.Object);

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () => 
                await reminderService.SpeakAsync("测试消息"));
            Assert.Null(exception);
        }
    }
}