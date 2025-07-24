using Foundation;
using UserNotifications;

namespace WorkTimeTracker.UI.Platforms.MacCatalyst
{
    public static class MacNotificationHelper
    {
        public static async Task<bool> ShowNotificationAsync(string title, string subtitle, string body)
        {
            try
            {
                var center = UNUserNotificationCenter.Current;
                
                // 检查权限
                var settings = await center.GetNotificationSettingsAsync();
                if (settings.AuthorizationStatus != UNAuthorizationStatus.Authorized)
                {
                    System.Diagnostics.Debug.WriteLine("通知权限未授权");
                    return false;
                }

                // 创建通知内容
                var content = new UNMutableNotificationContent
                {
                    Title = title,
                    Subtitle = subtitle,
                    Body = body,
                    Sound = UNNotificationSound.Default,
                    Badge = 1
                };

                // 立即触发的通知
                var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.1, false);
                
                // 创建请求
                var requestId = Guid.NewGuid().ToString();
                var request = UNNotificationRequest.FromIdentifier(requestId, content, trigger);

                // 显示通知
                await center.AddNotificationRequestAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"macOS 通知已发送: {title} - {body}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"macOS 通知发送失败: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> RequestPermissionAsync()
        {
            try
            {
                var center = UNUserNotificationCenter.Current;
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge;
                
                var (granted, error) = await center.RequestAuthorizationAsync(authOptions);
                
                if (granted)
                {
                    System.Diagnostics.Debug.WriteLine("macOS 通知权限已获得");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"macOS 通知权限被拒绝: {error?.LocalizedDescription}");
                }
                
                return granted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"请求 macOS 通知权限失败: {ex.Message}");
                return false;
            }
        }
    }
}
