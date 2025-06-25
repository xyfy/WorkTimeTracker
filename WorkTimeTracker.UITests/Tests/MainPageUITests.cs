using FluentAssertions;
using WorkTimeTracker.UITests.Base;
using WorkTimeTracker.UITests.Helpers;
using WorkTimeTracker.UITests.PageObjects;
using Xunit;

namespace WorkTimeTracker.UITests.Tests
{
    /// <summary>
    /// 主页面 UI 自动化测试
    /// </summary>
    public class MainPageUITests : UITestBase
    {
        private MainPageObject? _mainPage;

        [Fact]
        public async Task MainPage_ShouldLoadSuccessfully()
        {
            // Arrange
            await StartAppAsync();
            _mainPage = new MainPageObject(_appHost, _logger);

            // Act
            var isLoaded = await _mainPage.WaitForPageLoadAsync();

            // Assert
            isLoaded.Should().BeTrue("主页面应该成功加载");

            // 截取屏幕截图
            await TakeScreenshotAsync("MainPage_LoadSuccessfully");
        }

        [Fact]
        public async Task StartWorkButton_ShouldBeVisible()
        {
            // Arrange
            await StartAppAsync();
            _mainPage = new MainPageObject(_appHost, _logger);
            await _mainPage.WaitForPageLoadAsync();

            // Act
            var isVisible = await _mainPage.IsStartWorkButtonVisibleAsync();

            // Assert
            isVisible.Should().BeTrue("开始工作按钮应该可见");
        }

        [Fact]
        public async Task StartWork_WithValidInput_ShouldSucceed()
        {
            // Arrange
            await StartAppAsync();
            _mainPage = new MainPageObject(_appHost, _logger);
            await _mainPage.WaitForPageLoadAsync();

            var description = TestDataGenerator.GenerateWorkDescription();
            var projectName = TestDataGenerator.GenerateProjectName();

            // Act
            var result = await _mainPage.StartWorkAsync(description, projectName);

            // Assert
            result.Should().BeTrue("开始工作操作应该成功");

            // 验证停止工作按钮变为可见
            var stopButtonVisible = await _mainPage.IsStopWorkButtonVisibleAsync();
            stopButtonVisible.Should().BeTrue("开始工作后，停止工作按钮应该变为可见");

            // 截取屏幕截图
            await TakeScreenshotAsync("StartWork_Success", "after_start");
        }

        [Fact]
        public async Task StopWork_AfterStarting_ShouldSucceed()
        {
            // Arrange
            await StartAppAsync();
            _mainPage = new MainPageObject(_appHost, _logger);
            await _mainPage.WaitForPageLoadAsync();

            // 先开始工作
            await _mainPage.StartWorkAsync("测试工作", "测试项目");

            // Act
            var result = await _mainPage.StopWorkAsync();

            // Assert
            result.Should().BeTrue("停止工作操作应该成功");

            // 验证开始工作按钮重新变为可见
            var startButtonVisible = await _mainPage.IsStartWorkButtonVisibleAsync();
            startButtonVisible.Should().BeTrue("停止工作后，开始工作按钮应该重新变为可见");

            // 截取屏幕截图
            await TakeScreenshotAsync("StopWork_Success", "after_stop");
        }

        [Fact]
        public async Task NavigateToSchedulePage_ShouldSucceed()
        {
            // Arrange
            await StartAppAsync();
            _mainPage = new MainPageObject(_appHost, _logger);
            await _mainPage.WaitForPageLoadAsync();

            // Act
            var result = await _mainPage.NavigateToSchedulePageAsync();

            // Assert
            result.Should().BeTrue("导航到日程页面应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync("NavigateToSchedulePage_Success");
        }

        [Fact]
        public async Task NavigateToNotificationSettings_ShouldSucceed()
        {
            // Arrange
            await StartAppAsync();
            _mainPage = new MainPageObject(_appHost, _logger);
            await _mainPage.WaitForPageLoadAsync();

            // Act
            var result = await _mainPage.NavigateToNotificationSettingsAsync();

            // Assert
            result.Should().BeTrue("导航到通知设置页面应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync("NavigateToNotificationSettings_Success");
        }

        [Fact]
        public async Task WorkDescriptionEntry_ShouldAcceptText()
        {
            // Arrange
            await StartAppAsync();
            _mainPage = new MainPageObject(_appHost, _logger);
            await _mainPage.WaitForPageLoadAsync();

            var testDescription = TestDataGenerator.GenerateWorkDescription();

            // Act
            var result = await _mainPage.SetWorkDescriptionAsync(testDescription);

            // Assert
            result.Should().BeTrue("设置工作描述应该成功");

            // 验证输入的文本
            var actualDescription = await _mainPage.GetWorkDescriptionAsync();
            actualDescription.Should().NotBeEmpty("工作描述不应为空");

            // 截取屏幕截图
            await TakeScreenshotAsync("WorkDescriptionEntry_SetText");
        }

        [Fact]
        public async Task ProjectNameEntry_ShouldAcceptText()
        {
            // Arrange
            await StartAppAsync();
            _mainPage = new MainPageObject(_appHost, _logger);
            await _mainPage.WaitForPageLoadAsync();

            var testProjectName = TestDataGenerator.GenerateProjectName();

            // Act
            var result = await _mainPage.SetProjectNameAsync(testProjectName);

            // Assert
            result.Should().BeTrue("设置项目名称应该成功");

            // 验证输入的文本
            var actualProjectName = await _mainPage.GetProjectNameAsync();
            actualProjectName.Should().NotBeEmpty("项目名称不应为空");

            // 截取屏幕截图
            await TakeScreenshotAsync("ProjectNameEntry_SetText");
        }

        [Fact]
        public async Task WorkFlow_CompleteStartStopCycle_ShouldWork()
        {
            // Arrange
            await StartAppAsync();
            _mainPage = new MainPageObject(_appHost, _logger);
            await _mainPage.WaitForPageLoadAsync();

            var description = "完整工作流程测试";
            var projectName = "UI自动化测试项目";

            // Act & Assert - 完整的工作流程
            
            // 1. 设置工作信息
            await TakeScreenshotAsync("WorkFlow_Start", "initial_state");
            
            var setDescriptionResult = await _mainPage.SetWorkDescriptionAsync(description);
            setDescriptionResult.Should().BeTrue("设置工作描述应该成功");

            var setProjectResult = await _mainPage.SetProjectNameAsync(projectName);
            setProjectResult.Should().BeTrue("设置项目名称应该成功");

            await TakeScreenshotAsync("WorkFlow_Start", "info_filled");

            // 2. 开始工作
            var startResult = await _mainPage.StartWorkAsync();
            startResult.Should().BeTrue("开始工作应该成功");

            await TakeScreenshotAsync("WorkFlow_Start", "work_started");

            // 3. 检查状态
            var stopButtonVisible = await _mainPage.IsStopWorkButtonVisibleAsync();
            stopButtonVisible.Should().BeTrue("开始工作后，停止按钮应该可见");

            // 4. 模拟工作一段时间
            await Task.Delay(2000);
            await TakeScreenshotAsync("WorkFlow_Start", "working");

            // 5. 停止工作
            var stopResult = await _mainPage.StopWorkAsync();
            stopResult.Should().BeTrue("停止工作应该成功");

            await TakeScreenshotAsync("WorkFlow_Start", "work_stopped");

            // 6. 验证最终状态
            var startButtonVisible = await _mainPage.IsStartWorkButtonVisibleAsync();
            startButtonVisible.Should().BeTrue("停止工作后，开始按钮应该重新可见");
        }

        [Theory]
        [InlineData("软件开发", "移动应用项目")]
        [InlineData("会议讨论", "产品规划")]
        [InlineData("测试工作", "质量保证")]
        public async Task StartWork_WithDifferentInputs_ShouldWork(string description, string projectName)
        {
            // Arrange
            await StartAppAsync();
            _mainPage = new MainPageObject(_appHost, _logger);
            await _mainPage.WaitForPageLoadAsync();

            // Act
            var result = await _mainPage.StartWorkAsync(description, projectName);

            // Assert
            result.Should().BeTrue($"使用描述 '{description}' 和项目 '{projectName}' 开始工作应该成功");

            // 截取屏幕截图
            await TakeScreenshotAsync($"StartWork_WithInputs_{description.Replace(" ", "_")}");
        }

        public override void Dispose()
        {
            if (!_isDisposed)
            {
                _mainPage = null;
                base.Dispose();
            }
        }
    }
}
