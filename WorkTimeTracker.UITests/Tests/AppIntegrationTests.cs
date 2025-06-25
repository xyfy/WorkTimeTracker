using FluentAssertions;
using WorkTimeTracker.UITests.Base;
using WorkTimeTracker.UITests.Helpers;
using WorkTimeTracker.UITests.PageObjects;
using Xunit;

namespace WorkTimeTracker.UITests.Tests
{
    /// <summary>
    /// 应用程序集成测试 - 测试完整的用户工作流程
    /// </summary>
    public class AppIntegrationTests : UITestBase
    {
        [Fact]
        public async Task CompleteWorkSession_WithNotifications_ShouldWork()
        {
            // Arrange
            await StartAppAsync();
            
            var mainPage = new MainPageObject(_appHost, _logger);
            var notificationSettingsPage = new NotificationSettingsPageObject(_appHost, _logger);
            
            await mainPage.WaitForPageLoadAsync();

            var workDescription = "完整工作会话测试";
            var projectName = "集成测试项目";

            // Act & Assert - 完整的用户工作流程

            // 1. 首先配置通知设置
            await TakeScreenshotAsync("CompleteWorkSession", "start");
            
            var navigateToSettingsResult = await mainPage.NavigateToNotificationSettingsAsync();
            navigateToSettingsResult.Should().BeTrue("导航到通知设置应该成功");

            await notificationSettingsPage.WaitForPageLoadAsync();
            await TakeScreenshotAsync("CompleteWorkSession", "notification_settings_loaded");

            // 2. 配置通知
            await notificationSettingsPage.SetNotificationsEnabledAsync(true);
            await notificationSettingsPage.SetWorkStartReminderAsync(true);
            await notificationSettingsPage.SetWorkEndReminderAsync(true);
            await notificationSettingsPage.SetNotificationTypesAsync(true, true, false);
            await notificationSettingsPage.SetReminderFrequencyAsync(10);
            await notificationSettingsPage.SaveSettingsAsync();

            await TakeScreenshotAsync("CompleteWorkSession", "notifications_configured");

            // 3. 返回主页面
            var backToMainResult = await notificationSettingsPage.GoBackAsync();
            backToMainResult.Should().BeTrue("返回主页面应该成功");

            await mainPage.WaitForPageLoadAsync();
            await TakeScreenshotAsync("CompleteWorkSession", "back_to_main");

            // 4. 开始工作会话
            var startWorkResult = await mainPage.StartWorkAsync(workDescription, projectName);
            startWorkResult.Should().BeTrue("开始工作会话应该成功");

            await TakeScreenshotAsync("CompleteWorkSession", "work_started");

            // 5. 验证工作状态
            var stopButtonVisible = await mainPage.IsStopWorkButtonVisibleAsync();
            stopButtonVisible.Should().BeTrue("工作开始后，停止按钮应该可见");

            // 6. 模拟工作时间
            await Task.Delay(3000); // 模拟工作3秒
            await TakeScreenshotAsync("CompleteWorkSession", "working");

            // 7. 停止工作会话
            var stopWorkResult = await mainPage.StopWorkAsync();
            stopWorkResult.Should().BeTrue("停止工作会话应该成功");

            await TakeScreenshotAsync("CompleteWorkSession", "work_stopped");

            // 8. 验证最终状态
            var startButtonVisible = await mainPage.IsStartWorkButtonVisibleAsync();
            startButtonVisible.Should().BeTrue("工作停止后，开始按钮应该重新可见");

            await TakeScreenshotAsync("CompleteWorkSession", "final_state");
        }

        [Fact]
        public async Task NavigationFlow_BetweenAllPages_ShouldWork()
        {
            // Arrange
            await StartAppAsync();
            
            var mainPage = new MainPageObject(_appHost, _logger);
            var notificationSettingsPage = new NotificationSettingsPageObject(_appHost, _logger);
            
            await mainPage.WaitForPageLoadAsync();

            // Act & Assert - 测试所有页面间的导航

            // 1. 主页面状态
            await TakeScreenshotAsync("NavigationFlow", "main_page");
            var mainPageLoaded = await mainPage.IsPageLoadedAsync();
            mainPageLoaded.Should().BeTrue("主页面应该加载成功");

            // 2. 导航到日程页面
            var navigateToScheduleResult = await mainPage.NavigateToSchedulePageAsync();
            navigateToScheduleResult.Should().BeTrue("导航到日程页面应该成功");
            await TakeScreenshotAsync("NavigationFlow", "schedule_page");

            // 注意：这里假设日程页面有返回主页面的功能
            // 实际实现中需要根据日程页面的具体设计来调整

            // 3. 返回主页面（模拟）
            await Task.Delay(1000);
            await TakeScreenshotAsync("NavigationFlow", "back_from_schedule");

            // 4. 导航到通知设置页面
            var navigateToNotificationResult = await mainPage.NavigateToNotificationSettingsAsync();
            navigateToNotificationResult.Should().BeTrue("导航到通知设置页面应该成功");

            await notificationSettingsPage.WaitForPageLoadAsync();
            await TakeScreenshotAsync("NavigationFlow", "notification_settings_page");

            // 5. 从通知设置返回主页面
            var backFromNotificationResult = await notificationSettingsPage.GoBackAsync();
            backFromNotificationResult.Should().BeTrue("从通知设置返回应该成功");

            await mainPage.WaitForPageLoadAsync();
            await TakeScreenshotAsync("NavigationFlow", "back_from_notifications");

            // 6. 验证回到主页面
            var finalMainPageLoaded = await mainPage.IsPageLoadedAsync();
            finalMainPageLoaded.Should().BeTrue("最终应该回到主页面");
        }

        [Fact]
        public async Task MultipleWorkSessions_ShouldWork()
        {
            // Arrange
            await StartAppAsync();
            
            var mainPage = new MainPageObject(_appHost, _logger);
            await mainPage.WaitForPageLoadAsync();

            var sessions = new[]
            {
                ("第一个工作会话", "项目A"),
                ("第二个工作会话", "项目B"), 
                ("第三个工作会话", "项目C")
            };

            // Act & Assert - 测试多个连续的工作会话

            await TakeScreenshotAsync("MultipleWorkSessions", "start");

            for (int i = 0; i < sessions.Length; i++)
            {
                var (description, project) = sessions[i];
                
                _logger.Information($"开始第 {i + 1} 个工作会话: {description} - {project}");

                // 开始工作会话
                var startResult = await mainPage.StartWorkAsync(description, project);
                startResult.Should().BeTrue($"第 {i + 1} 个工作会话应该成功开始");

                await TakeScreenshotAsync("MultipleWorkSessions", $"session_{i + 1}_started");

                // 模拟工作时间
                await Task.Delay(2000);

                // 停止工作会话
                var stopResult = await mainPage.StopWorkAsync();
                stopResult.Should().BeTrue($"第 {i + 1} 个工作会话应该成功停止");

                await TakeScreenshotAsync("MultipleWorkSessions", $"session_{i + 1}_stopped");

                // 验证状态重置
                var startButtonVisible = await mainPage.IsStartWorkButtonVisibleAsync();
                startButtonVisible.Should().BeTrue($"第 {i + 1} 个工作会话结束后，开始按钮应该可见");

                // 会话间短暂间隔
                await Task.Delay(1000);
            }

            await TakeScreenshotAsync("MultipleWorkSessions", "all_sessions_completed");
        }

        [Fact]
        public async Task ErrorRecovery_InvalidInputs_ShouldHandle()
        {
            // Arrange
            await StartAppAsync();
            
            var mainPage = new MainPageObject(_appHost, _logger);
            var notificationSettingsPage = new NotificationSettingsPageObject(_appHost, _logger);
            
            await mainPage.WaitForPageLoadAsync();

            // Act & Assert - 测试错误处理和恢复

            // 1. 测试空输入的工作会话
            await TakeScreenshotAsync("ErrorRecovery", "start");
            
            var emptyStartResult = await mainPage.StartWorkAsync("", "");
            // 注意：这里的期望结果取决于应用的具体验证逻辑
            // 可能会成功（允许空输入）或失败（需要输入验证）
            await TakeScreenshotAsync("ErrorRecovery", "empty_input_attempt");

            // 2. 测试非常长的输入
            var longDescription = new string('X', 1000);
            var longProject = new string('Y', 1000);
            
            var longInputResult = await mainPage.StartWorkAsync(longDescription, longProject);
            await TakeScreenshotAsync("ErrorRecovery", "long_input_attempt");

            // 3. 如果开始了工作，先停止
            var isWorking = await mainPage.IsStopWorkButtonVisibleAsync();
            if (isWorking)
            {
                await mainPage.StopWorkAsync();
                await TakeScreenshotAsync("ErrorRecovery", "stopped_after_long_input");
            }

            // 4. 测试通知设置页面的错误恢复
            var navigateResult = await mainPage.NavigateToNotificationSettingsAsync();
            navigateResult.Should().BeTrue("导航到通知设置应该成功");

            await notificationSettingsPage.WaitForPageLoadAsync();
            await TakeScreenshotAsync("ErrorRecovery", "notification_settings_loaded");

            // 5. 测试无效的提醒频率输入
            var invalidFrequencyResult = await notificationSettingsPage.SetReminderFrequencyAsync(-1);
            await TakeScreenshotAsync("ErrorRecovery", "invalid_frequency");

            var zeroFrequencyResult = await notificationSettingsPage.SetReminderFrequencyAsync(0);
            await TakeScreenshotAsync("ErrorRecovery", "zero_frequency");

            // 6. 重置到有效状态
            var resetResult = await notificationSettingsPage.ResetSettingsAsync();
            resetResult.Should().BeTrue("重置设置应该成功");
            await TakeScreenshotAsync("ErrorRecovery", "settings_reset");

            // 7. 返回主页面
            var backResult = await notificationSettingsPage.GoBackAsync();
            backResult.Should().BeTrue("返回主页面应该成功");

            await mainPage.WaitForPageLoadAsync();
            await TakeScreenshotAsync("ErrorRecovery", "recovery_complete");
        }

        [Fact]
        public async Task PerformanceTest_QuickOperations_ShouldRespond()
        {
            // Arrange
            await StartAppAsync();
            
            var mainPage = new MainPageObject(_appHost, _logger);
            await mainPage.WaitForPageLoadAsync();

            var startTime = DateTime.Now;

            // Act & Assert - 测试快速操作的响应性能

            await TakeScreenshotAsync("PerformanceTest", "start");

            // 1. 快速启动/停止工作会话
            for (int i = 0; i < 3; i++)
            {
                var sessionStart = DateTime.Now;
                
                var startResult = await mainPage.StartWorkAsync($"性能测试会话 {i + 1}", "性能测试");
                startResult.Should().BeTrue($"快速启动第 {i + 1} 个会话应该成功");

                var startDuration = DateTime.Now - sessionStart;
                _logger.Information($"启动会话 {i + 1} 耗时: {startDuration.TotalMilliseconds} ms");

                var sessionStop = DateTime.Now;
                
                var stopResult = await mainPage.StopWorkAsync();
                stopResult.Should().BeTrue($"快速停止第 {i + 1} 个会话应该成功");

                var stopDuration = DateTime.Now - sessionStop;
                _logger.Information($"停止会话 {i + 1} 耗时: {stopDuration.TotalMilliseconds} ms");

                // 性能断言：操作应该在合理时间内完成（例如 5 秒）
                startDuration.Should().BeLessThan(TimeSpan.FromSeconds(5), $"启动会话 {i + 1} 应该在 5 秒内完成");
                stopDuration.Should().BeLessThan(TimeSpan.FromSeconds(5), $"停止会话 {i + 1} 应该在 5 秒内完成");
            }

            await TakeScreenshotAsync("PerformanceTest", "sessions_completed");

            // 2. 快速页面导航
            var navigationStart = DateTime.Now;
            
            var navigateToSettingsResult = await mainPage.NavigateToNotificationSettingsAsync();
            navigateToSettingsResult.Should().BeTrue("导航到设置页面应该成功");

            var notificationSettingsPage = new NotificationSettingsPageObject(_appHost, _logger);
            await notificationSettingsPage.WaitForPageLoadAsync();

            var backResult = await notificationSettingsPage.GoBackAsync();
            backResult.Should().BeTrue("返回主页面应该成功");

            await mainPage.WaitForPageLoadAsync();

            var navigationDuration = DateTime.Now - navigationStart;
            _logger.Information($"完整导航流程耗时: {navigationDuration.TotalMilliseconds} ms");

            // 导航性能断言
            navigationDuration.Should().BeLessThan(TimeSpan.FromSeconds(10), "完整导航流程应该在 10 秒内完成");

            var totalDuration = DateTime.Now - startTime;
            _logger.Information($"性能测试总耗时: {totalDuration.TotalMilliseconds} ms");

            await TakeScreenshotAsync("PerformanceTest", "completed");
        }
    }
}
