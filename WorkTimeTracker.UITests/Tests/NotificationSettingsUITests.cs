using FluentAssertions;
using WorkTimeTracker.UITests.Base;
using WorkTimeTracker.UITests.Helpers;
using WorkTimeTracker.UITests.PageObjects;
using Xunit;

namespace WorkTimeTracker.UITests.Tests
{
    /// <summary>
    /// 通知设置页面 UI 自动化测试
    /// </summary>
    public class NotificationSettingsUITests : UITestBase
    {
        private MainPageObject? _mainPage;
        private NotificationSettingsPageObject? _notificationSettingsPage;

        [Fact]
        public async Task NotificationSettingsPage_ShouldLoadSuccessfully()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // Act
            var isLoaded = await _notificationSettingsPage!.WaitForPageLoadAsync();

            // Assert
            isLoaded.Should().BeTrue("通知设置页面应该成功加载");

            // 截取屏幕截图
            await TakeScreenshotAsync("NotificationSettingsPage_LoadSuccessfully");
        }

        [Fact]
        public async Task EnableNotifications_ShouldToggleSuccessfully()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // Act
            var result = await _notificationSettingsPage!.SetNotificationsEnabledAsync(true);

            // Assert
            result.Should().BeTrue("启用通知应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync("EnableNotifications_Success");
        }

        [Fact]
        public async Task SetReminderTypes_ShouldWork()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // 先启用通知
            await _notificationSettingsPage!.SetNotificationsEnabledAsync(true);

            // Act
            var workStartResult = await _notificationSettingsPage.SetWorkStartReminderAsync(true);
            var workEndResult = await _notificationSettingsPage.SetWorkEndReminderAsync(true);
            var breakResult = await _notificationSettingsPage.SetBreakReminderAsync(true);

            // Assert
            workStartResult.Should().BeTrue("设置工作开始提醒应该成功");
            workEndResult.Should().BeTrue("设置工作结束提醒应该成功");
            breakResult.Should().BeTrue("设置休息提醒应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync("SetReminderTypes_Success");
        }

        [Fact]
        public async Task SetNotificationTypes_ShouldWork()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // 先启用通知
            await _notificationSettingsPage!.SetNotificationsEnabledAsync(true);

            // Act
            var result = await _notificationSettingsPage.SetNotificationTypesAsync(
                sound: true, 
                popup: true, 
                vibration: false);

            // Assert
            result.Should().BeTrue("设置通知类型应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync("SetNotificationTypes_Success");
        }

        [Fact]
        public async Task SetVolume_ShouldWork()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // 先启用通知
            await _notificationSettingsPage!.SetNotificationsEnabledAsync(true);

            // Act
            var result = await _notificationSettingsPage.SetVolumeAsync(0.8f);

            // Assert
            result.Should().BeTrue("设置音量应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync("SetVolume_Success");
        }

        [Theory]
        [InlineData("zh-CN")]
        [InlineData("en-US")]
        [InlineData("ja-JP")]
        public async Task SetLanguage_ShouldWork(string language)
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // Act
            var result = await _notificationSettingsPage!.SetLanguageAsync(language);

            // Assert
            result.Should().BeTrue($"设置语言为 {language} 应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync($"SetLanguage_{language}_Success");
        }

        [Fact]
        public async Task SetDoNotDisturb_ShouldWork()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // 先启用通知
            await _notificationSettingsPage!.SetNotificationsEnabledAsync(true);

            // Act
            var result = await _notificationSettingsPage.SetDoNotDisturbAsync(
                enabled: true, 
                startTime: "22:00", 
                endTime: "08:00");

            // Assert
            result.Should().BeTrue("设置免打扰模式应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync("SetDoNotDisturb_Success");
        }

        [Theory]
        [InlineData(5)]
        [InlineData(15)]
        [InlineData(30)]
        [InlineData(60)]
        public async Task SetReminderFrequency_ShouldWork(int minutes)
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // 先启用通知
            await _notificationSettingsPage!.SetNotificationsEnabledAsync(true);

            // Act
            var result = await _notificationSettingsPage.SetReminderFrequencyAsync(minutes);

            // Assert
            result.Should().BeTrue($"设置提醒频率为 {minutes} 分钟应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync($"SetReminderFrequency_{minutes}min_Success");
        }

        [Fact]
        public async Task SetCustomMessage_ShouldWork()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            var customMessage = "这是一条自定义的工作提醒消息！";

            // 先启用通知
            await _notificationSettingsPage!.SetNotificationsEnabledAsync(true);

            // Act
            var result = await _notificationSettingsPage.SetCustomMessageAsync(customMessage);

            // Assert
            result.Should().BeTrue("设置自定义消息应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync("SetCustomMessage_Success");
        }

        [Fact]
        public async Task SaveSettings_ShouldSucceed()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // 配置一些设置
            await _notificationSettingsPage!.SetNotificationsEnabledAsync(true);
            await _notificationSettingsPage.SetWorkStartReminderAsync(true);
            await _notificationSettingsPage.SetReminderFrequencyAsync(15);

            // Act
            var result = await _notificationSettingsPage.SaveSettingsAsync();

            // Assert
            result.Should().BeTrue("保存设置应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync("SaveSettings_Success");
        }

        [Fact]
        public async Task ResetSettings_ShouldSucceed()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // 先做一些设置更改
            await _notificationSettingsPage!.SetNotificationsEnabledAsync(true);
            await _notificationSettingsPage.SetWorkStartReminderAsync(true);
            await _notificationSettingsPage.SetReminderFrequencyAsync(30);

            // Act
            var result = await _notificationSettingsPage.ResetSettingsAsync();

            // Assert
            result.Should().BeTrue("重置设置应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync("ResetSettings_Success");
        }

        [Fact]
        public async Task GoBack_ShouldReturnToMainPage()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // Act
            var result = await _notificationSettingsPage!.GoBackAsync();

            // Assert
            result.Should().BeTrue("返回主页面应该成功");

            // 验证是否回到了主页面
            var mainPageLoaded = await _mainPage!.WaitForPageLoadAsync();
            mainPageLoaded.Should().BeTrue("应该成功返回到主页面");

            // 截取屏幕截图
            await TakeScreenshotAsync("GoBack_Success");
        }

        [Fact]
        public async Task CompleteNotificationSetup_ShouldWork()
        {
            // Arrange
            await StartAppAsync();
            await NavigateToNotificationSettingsPageAsync();

            // 使用新的测试数据生成器
            var isEnabled = TestDataGenerator.NotificationTestData.GenerateIsEnabled();
            var workStartReminder = TestDataGenerator.NotificationTestData.GenerateWorkStartReminder();
            var volume = TestDataGenerator.NotificationTestData.GenerateVolume();
            var language = TestDataGenerator.NotificationTestData.GenerateLanguage();
            var isDoNotDisturbEnabled = TestDataGenerator.NotificationTestData.GenerateIsDoNotDisturbEnabled();
            var doNotDisturbStart = TestDataGenerator.NotificationTestData.GenerateDoNotDisturbStart();
            var doNotDisturbEnd = TestDataGenerator.NotificationTestData.GenerateDoNotDisturbEnd();
            var customMessage = TestDataGenerator.NotificationTestData.GenerateCustomMessage();
            var reminderFrequency = TestDataGenerator.NotificationTestData.GenerateReminderFrequencyMinutes();
            var notificationTypes = TestDataGenerator.NotificationTestData.GenerateNotificationTypes();

            // Act & Assert - 完整的通知设置流程

            // 1. 启用通知
            await TakeScreenshotAsync("CompleteNotificationSetup", "initial_state");
            
            var enableResult = await _notificationSettingsPage!.SetNotificationsEnabledAsync(isEnabled);
            enableResult.Should().BeTrue("启用通知应该成功");

            // 2. 设置提醒类型
            var reminderResult = await _notificationSettingsPage.SetWorkStartReminderAsync(workStartReminder);
            reminderResult.Should().BeTrue("设置工作开始提醒应该成功");

            await TakeScreenshotAsync("CompleteNotificationSetup", "reminders_set");

            // 3. 设置通知类型
            var notificationTypeResult = await _notificationSettingsPage.SetNotificationTypesAsync(
                sound: notificationTypes.Contains("Sound"),
                popup: notificationTypes.Contains("Popup"),
                vibration: notificationTypes.Contains("Vibration"));
            notificationTypeResult.Should().BeTrue("设置通知类型应该成功");

            // 4. 设置音量
            var volumeResult = await _notificationSettingsPage.SetVolumeAsync(volume);
            volumeResult.Should().BeTrue("设置音量应该成功");

            await TakeScreenshotAsync("CompleteNotificationSetup", "volume_set");

            // 5. 设置语言
            var languageResult = await _notificationSettingsPage.SetLanguageAsync(language);
            languageResult.Should().BeTrue("设置语言应该成功");

            // 6. 设置免打扰
            if (isDoNotDisturbEnabled)
            {
                var dndResult = await _notificationSettingsPage.SetDoNotDisturbAsync(
                    enabled: true,
                    startTime: doNotDisturbStart,
                    endTime: doNotDisturbEnd);
                dndResult.Should().BeTrue("设置免打扰应该成功");
            }

            await TakeScreenshotAsync("CompleteNotificationSetup", "do_not_disturb_set");

            // 7. 设置提醒频率
            var frequencyResult = await _notificationSettingsPage.SetReminderFrequencyAsync(reminderFrequency);
            frequencyResult.Should().BeTrue("设置提醒频率应该成功");

            // 8. 设置自定义消息
            if (!string.IsNullOrEmpty(customMessage))
            {
                var messageResult = await _notificationSettingsPage.SetCustomMessageAsync(customMessage);
                messageResult.Should().BeTrue("设置自定义消息应该成功");
            }

            await TakeScreenshotAsync("CompleteNotificationSetup", "all_settings_configured");

            // 9. 保存设置
            var saveResult = await _notificationSettingsPage.SaveSettingsAsync();
            saveResult.Should().BeTrue("保存设置应该成功");

            await TakeScreenshotAsync("CompleteNotificationSetup", "settings_saved");

            // 10. 返回主页面
            var backResult = await _notificationSettingsPage.GoBackAsync();
            backResult.Should().BeTrue("返回主页面应该成功");

            await TakeScreenshotAsync("CompleteNotificationSetup", "returned_to_main");
        }

        /// <summary>
        /// 导航到通知设置页面的辅助方法
        /// </summary>
        private async Task NavigateToNotificationSettingsPageAsync()
        {
            _mainPage = new MainPageObject(_appHost, _logger);
            await _mainPage.WaitForPageLoadAsync();

            var navigationResult = await _mainPage.NavigateToNotificationSettingsAsync();
            navigationResult.Should().BeTrue("导航到通知设置页面应该成功");

            _notificationSettingsPage = new NotificationSettingsPageObject(_appHost, _logger);
            await _notificationSettingsPage.WaitForPageLoadAsync();
        }

        public override void Dispose()
        {
            if (!_isDisposed)
            {
                _mainPage = null;
                _notificationSettingsPage = null;
                base.Dispose();
            }
        }
    }
}
