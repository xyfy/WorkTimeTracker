using Microsoft.Extensions.Hosting;
using Serilog;
using WorkTimeTracker.UITests.PageObjects.Base;

namespace WorkTimeTracker.UITests.PageObjects
{
    /// <summary>
    /// 主页面对象模型
    /// </summary>
    public class MainPageObject : PageObjectBase
    {
        // 元素选择器常量
        private const string START_WORK_BUTTON = "StartWorkButton";
        private const string STOP_WORK_BUTTON = "StopWorkButton";
        private const string VIEW_SCHEDULE_BUTTON = "ViewScheduleButton";
        private const string NOTIFICATION_SETTINGS_BUTTON = "NotificationSettingsButton";
        private const string CURRENT_STATUS_LABEL = "CurrentStatusLabel";
        private const string CURRENT_TIME_LABEL = "CurrentTimeLabel";
        private const string WORK_DESCRIPTION_ENTRY = "WorkDescriptionEntry";
        private const string PROJECT_NAME_ENTRY = "ProjectNameEntry";

        public MainPageObject(IHost? appHost, ILogger logger) : base(appHost, logger)
        {
        }

        /// <summary>
        /// 检查主页面是否已加载
        /// </summary>
        public override async Task<bool> IsPageLoadedAsync()
        {
            return await IsElementVisibleAsync(START_WORK_BUTTON) &&
                   await IsElementVisibleAsync(VIEW_SCHEDULE_BUTTON) &&
                   await IsElementVisibleAsync(NOTIFICATION_SETTINGS_BUTTON);
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        /// <param name="description">工作描述</param>
        /// <param name="projectName">项目名称</param>
        public async Task<bool> StartWorkAsync(string description = "", string projectName = "")
        {
            try
            {
                _logger.Information("开始执行开始工作操作");

                // 填写工作描述（如果提供）
                if (!string.IsNullOrEmpty(description))
                {
                    if (!await EnterTextAsync(WORK_DESCRIPTION_ENTRY, description))
                    {
                        return false;
                    }
                }

                // 填写项目名称（如果提供）
                if (!string.IsNullOrEmpty(projectName))
                {
                    if (!await EnterTextAsync(PROJECT_NAME_ENTRY, projectName))
                    {
                        return false;
                    }
                }

                // 点击开始工作按钮
                if (!await ClickElementAsync(START_WORK_BUTTON))
                {
                    return false;
                }

                // 等待状态更新
                await Task.Delay(1000);

                _logger.Information("开始工作操作完成");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "开始工作操作失败");
                return false;
            }
        }

        /// <summary>
        /// 停止工作
        /// </summary>
        public async Task<bool> StopWorkAsync()
        {
            try
            {
                _logger.Information("开始执行停止工作操作");

                // 点击停止工作按钮
                if (!await ClickElementAsync(STOP_WORK_BUTTON))
                {
                    return false;
                }

                // 等待状态更新
                await Task.Delay(1000);

                _logger.Information("停止工作操作完成");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "停止工作操作失败");
                return false;
            }
        }

        /// <summary>
        /// 导航到日程页面
        /// </summary>
        public async Task<bool> NavigateToSchedulePageAsync()
        {
            try
            {
                _logger.Information("导航到日程页面");

                if (!await ClickElementAsync(VIEW_SCHEDULE_BUTTON))
                {
                    return false;
                }

                await Task.Delay(500); // 等待页面切换
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "导航到日程页面失败");
                return false;
            }
        }

        /// <summary>
        /// 导航到通知设置页面
        /// </summary>
        public async Task<bool> NavigateToNotificationSettingsAsync()
        {
            try
            {
                _logger.Information("导航到通知设置页面");

                if (!await ClickElementAsync(NOTIFICATION_SETTINGS_BUTTON))
                {
                    return false;
                }

                await Task.Delay(500); // 等待页面切换
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "导航到通知设置页面失败");
                return false;
            }
        }

        /// <summary>
        /// 获取当前工作状态
        /// </summary>
        public async Task<string> GetCurrentWorkStatusAsync()
        {
            try
            {
                return await GetElementTextAsync(CURRENT_STATUS_LABEL);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "获取当前工作状态失败");
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取当前时间显示
        /// </summary>
        public async Task<string> GetCurrentTimeAsync()
        {
            try
            {
                return await GetElementTextAsync(CURRENT_TIME_LABEL);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "获取当前时间失败");
                return string.Empty;
            }
        }

        /// <summary>
        /// 检查开始工作按钮是否可见
        /// </summary>
        public async Task<bool> IsStartWorkButtonVisibleAsync()
        {
            return await IsElementVisibleAsync(START_WORK_BUTTON);
        }

        /// <summary>
        /// 检查停止工作按钮是否可见
        /// </summary>
        public async Task<bool> IsStopWorkButtonVisibleAsync()
        {
            return await IsElementVisibleAsync(STOP_WORK_BUTTON);
        }

        /// <summary>
        /// 设置工作描述
        /// </summary>
        /// <param name="description">工作描述</param>
        public async Task<bool> SetWorkDescriptionAsync(string description)
        {
            return await EnterTextAsync(WORK_DESCRIPTION_ENTRY, description);
        }

        /// <summary>
        /// 设置项目名称
        /// </summary>
        /// <param name="projectName">项目名称</param>
        public async Task<bool> SetProjectNameAsync(string projectName)
        {
            return await EnterTextAsync(PROJECT_NAME_ENTRY, projectName);
        }

        /// <summary>
        /// 获取工作描述
        /// </summary>
        public async Task<string> GetWorkDescriptionAsync()
        {
            return await GetElementTextAsync(WORK_DESCRIPTION_ENTRY);
        }

        /// <summary>
        /// 获取项目名称
        /// </summary>
        public async Task<string> GetProjectNameAsync()
        {
            return await GetElementTextAsync(PROJECT_NAME_ENTRY);
        }
    }
}
