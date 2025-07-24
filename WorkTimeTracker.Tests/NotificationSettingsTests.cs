using Xunit;
using WorkTimeTracker.Core.Models;
using System;

namespace WorkTimeTracker.Tests
{
    public class NotificationSettingsTests
    {
        [Fact]
        public void NotificationSettings_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var settings = new NotificationSettings();

            // Assert
            Assert.True(settings.VoiceReminderEnabled);
            Assert.True(settings.SystemNotificationEnabled);
            Assert.False(settings.ForegroundPopupEnabled);
            Assert.True(settings.DesktopNotificationEnabled);
            Assert.Equal(string.Empty, settings.CustomVoiceMessage);
            Assert.Equal(5, settings.ReminderFrequencyMinutes);
            Assert.False(settings.DoNotDisturbEnabled);
            Assert.Equal(new TimeSpan(22, 0, 0), settings.DoNotDisturbStartTime);
            Assert.Equal(new TimeSpan(8, 0, 0), settings.DoNotDisturbEndTime);
            Assert.Equal(0.8, settings.VoiceVolume);
            Assert.Equal("zh-CN", settings.VoiceLanguage);
            Assert.Equal("开始工作", settings.WorkStartMessage);
            Assert.Equal("工作结束", settings.WorkEndMessage);
            Assert.Equal("开始休息", settings.RestStartMessage);
            Assert.Equal("休息结束", settings.RestEndMessage);
        }

        [Fact]
        public void NotificationSettings_Properties_CanBeModified()
        {
            // Arrange
            var settings = new NotificationSettings();

            // Act
            settings.VoiceReminderEnabled = false;
            settings.SystemNotificationEnabled = false;
            settings.ForegroundPopupEnabled = true;
            settings.DesktopNotificationEnabled = false;
            settings.CustomVoiceMessage = "自定义消息";
            settings.ReminderFrequencyMinutes = 10;
            settings.DoNotDisturbEnabled = true;
            settings.DoNotDisturbStartTime = new TimeSpan(20, 0, 0);
            settings.DoNotDisturbEndTime = new TimeSpan(6, 0, 0);
            settings.VoiceVolume = 0.5;
            settings.VoiceLanguage = "en-US";
            settings.WorkStartMessage = "Start Working";
            settings.WorkEndMessage = "Work Finished";
            settings.RestStartMessage = "Start Break";
            settings.RestEndMessage = "Break Finished";

            // Assert
            Assert.False(settings.VoiceReminderEnabled);
            Assert.False(settings.SystemNotificationEnabled);
            Assert.True(settings.ForegroundPopupEnabled);
            Assert.False(settings.DesktopNotificationEnabled);
            Assert.Equal("自定义消息", settings.CustomVoiceMessage);
            Assert.Equal(10, settings.ReminderFrequencyMinutes);
            Assert.True(settings.DoNotDisturbEnabled);
            Assert.Equal(new TimeSpan(20, 0, 0), settings.DoNotDisturbStartTime);
            Assert.Equal(new TimeSpan(6, 0, 0), settings.DoNotDisturbEndTime);
            Assert.Equal(0.5, settings.VoiceVolume);
            Assert.Equal("en-US", settings.VoiceLanguage);
            Assert.Equal("Start Working", settings.WorkStartMessage);
            Assert.Equal("Work Finished", settings.WorkEndMessage);
            Assert.Equal("Start Break", settings.RestStartMessage);
            Assert.Equal("Break Finished", settings.RestEndMessage);
        }

        [Theory]
        [InlineData(22, 0, 8, 0, 23, 0, true)]  // 23:00 在 22:00-08:00 之间
        [InlineData(22, 0, 8, 0, 7, 0, true)]   // 07:00 在 22:00-08:00 之间
        [InlineData(22, 0, 8, 0, 12, 0, false)] // 12:00 不在 22:00-08:00 之间
        [InlineData(9, 0, 17, 0, 12, 0, true)]  // 12:00 在 09:00-17:00 之间
        [InlineData(9, 0, 17, 0, 18, 0, false)] // 18:00 不在 09:00-17:00 之间
        public void IsInDoNotDisturbPeriod_ShouldCalculateCorrectly(
            int startHour, int startMinute, 
            int endHour, int endMinute,
            int currentHour, int currentMinute,
            bool expectedResult)
        {
            // Arrange
            var settings = new NotificationSettings
            {
                DoNotDisturbEnabled = true,
                DoNotDisturbStartTime = new TimeSpan(startHour, startMinute, 0),
                DoNotDisturbEndTime = new TimeSpan(endHour, endMinute, 0)
            };

            var currentTime = new TimeSpan(currentHour, currentMinute, 0);

            // Act
            bool result = IsInDoNotDisturbPeriodTestHelper(settings, currentTime);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        // 辅助方法来测试免打扰逻辑（模拟 NotificationService 中的逻辑）
        private bool IsInDoNotDisturbPeriodTestHelper(NotificationSettings settings, TimeSpan currentTime)
        {
            if (!settings.DoNotDisturbEnabled)
                return false;

            var start = settings.DoNotDisturbStartTime;
            var end = settings.DoNotDisturbEndTime;

            // 处理跨天的情况（例如 22:00 到次日 08:00）
            if (start > end)
            {
                return currentTime >= start || currentTime <= end;
            }
            else
            {
                return currentTime >= start && currentTime <= end;
            }
        }

        [Fact]
        public void NotificationSettings_VolumeRange_ShouldBeValid()
        {
            // Arrange
            var settings = new NotificationSettings();

            // Act & Assert - 测试有效范围
            settings.VoiceVolume = 0.0;
            Assert.Equal(0.0, settings.VoiceVolume);

            settings.VoiceVolume = 0.5;
            Assert.Equal(0.5, settings.VoiceVolume);

            settings.VoiceVolume = 1.0;
            Assert.Equal(1.0, settings.VoiceVolume);
        }

        [Fact]
        public void NotificationSettings_ReminderFrequency_ShouldBePositive()
        {
            // Arrange
            var settings = new NotificationSettings();

            // Act
            settings.ReminderFrequencyMinutes = 1;

            // Assert
            Assert.Equal(1, settings.ReminderFrequencyMinutes);
        }
    }
}
