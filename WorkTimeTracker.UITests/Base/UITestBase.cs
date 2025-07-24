using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using Xunit;

namespace WorkTimeTracker.UITests.Base
{
    /// <summary>
    /// UI 自动化测试基类，提供通用的设置和清理逻辑
    /// </summary>
    public abstract class UITestBase : IDisposable
    {
        protected readonly ILogger _logger;
        protected IHost? _appHost;
        protected bool _isDisposed;

        protected UITestBase()
        {
            // 配置日志记录
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(GetScreenshotsFolder(), "ui-tests-.log"), 
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // 确保必要的文件夹存在
            EnsureDirectoriesExist();
        }

        /// <summary>
        /// 启动 MAUI 应用程序（模拟）
        /// </summary>
        protected virtual async Task StartAppAsync()
        {
            try
            {
                _logger.Information("正在准备 MAUI 应用程序测试环境...");
                
                // 注意：这是一个模拟的应用启动过程
                // 在实际的 MAUI UI 测试中，需要使用适当的测试框架来启动应用
                // 例如使用 Appium 或其他移动应用测试工具
                
                // 模拟应用启动延迟
                await Task.Delay(1000);
                
                _logger.Information("MAUI 应用程序测试环境准备完成");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "准备 MAUI 应用程序测试环境失败");
                throw;
            }
        }

        /// <summary>
        /// 停止应用程序
        /// </summary>
        protected virtual async Task StopAppAsync()
        {
            try
            {
                if (_appHost != null)
                {
                    _logger.Information("正在停止测试环境...");
                    await _appHost.StopAsync();
                    _appHost.Dispose();
                    _appHost = null;
                    _logger.Information("测试环境已停止");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "停止测试环境失败");
            }
        }

        /// <summary>
        /// 截取屏幕截图
        /// </summary>
        /// <param name="testName">测试名称</param>
        /// <param name="stepName">步骤名称</param>
        protected virtual async Task<string> TakeScreenshotAsync(string testName, string stepName = "")
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = string.IsNullOrEmpty(stepName) 
                    ? $"{testName}_{timestamp}.png"
                    : $"{testName}_{stepName}_{timestamp}.png";
                
                var filePath = Path.Combine(GetScreenshotsFolder(), fileName);
                
                // 这里应该实现实际的截图逻辑
                // 具体实现取决于所使用的测试框架
                
                _logger.Information($"屏幕截图已保存: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"截取屏幕截图失败: {testName}/{stepName}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 等待元素出现
        /// </summary>
        /// <param name="elementSelector">元素选择器</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        protected virtual async Task<bool> WaitForElementAsync(string elementSelector, int timeoutSeconds = 10)
        {
            var timeout = TimeSpan.FromSeconds(timeoutSeconds);
            var startTime = DateTime.Now;

            while (DateTime.Now - startTime < timeout)
            {
                try
                {
                    // 这里应该实现实际的元素查找逻辑
                    // 返回 true 如果找到元素
                    await Task.Delay(500); // 模拟等待
                }
                catch
                {
                    // 继续等待
                }

                await Task.Delay(100);
            }

            _logger.Warning($"等待元素超时: {elementSelector}");
            return false;
        }

        /// <summary>
        /// 获取截图文件夹路径
        /// </summary>
        private static string GetScreenshotsFolder()
        {
            var projectDir = GetProjectDirectory();
            return Path.Combine(projectDir, "Screenshots");
        }

        /// <summary>
        /// 获取测试数据文件夹路径
        /// </summary>
        protected static string GetTestDataFolder()
        {
            var projectDir = GetProjectDirectory();
            return Path.Combine(projectDir, "TestData");
        }

        /// <summary>
        /// 获取报告文件夹路径
        /// </summary>
        protected static string GetReportsFolder()
        {
            var projectDir = GetProjectDirectory();
            return Path.Combine(projectDir, "Reports");
        }

        /// <summary>
        /// 获取项目目录
        /// </summary>
        private static string GetProjectDirectory()
        {
            var currentDir = Directory.GetCurrentDirectory();
            
            // 查找项目根目录（包含 .csproj 文件的目录）
            var dir = new DirectoryInfo(currentDir);
            while (dir != null && !dir.GetFiles("*.csproj").Any())
            {
                dir = dir.Parent;
            }

            return dir?.FullName ?? currentDir;
        }

        /// <summary>
        /// 确保必要的文件夹存在
        /// </summary>
        private void EnsureDirectoriesExist()
        {
            var directories = new[]
            {
                GetScreenshotsFolder(),
                GetTestDataFolder(),
                GetReportsFolder()
            };

            foreach (var directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    _logger.Information($"创建目录: {directory}");
                }
            }
        }

        public virtual void Dispose()
        {
            if (!_isDisposed)
            {
                StopAppAsync().Wait();
                // Serilog Logger 实现了 IDisposable，但接口没有暴露
                if (_logger is IDisposable disposableLogger)
                {
                    disposableLogger.Dispose();
                }
                _isDisposed = true;
            }
        }
    }
}
