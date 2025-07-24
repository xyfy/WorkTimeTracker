using Foundation;
using UserNotifications;

namespace WorkTimeTracker.UI;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIKit.UIApplication application, NSDictionary launchOptions)
    {
        // 请求通知权限
        RequestNotificationPermissions();
        
        return base.FinishedLaunching(application, launchOptions);
    }

    private async void RequestNotificationPermissions()
    {
        try
        {
            var center = UNUserNotificationCenter.Current;
            var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge;
            
            var (granted, error) = await center.RequestAuthorizationAsync(authOptions);
            
            if (granted)
            {
                System.Diagnostics.Debug.WriteLine("通知权限已获得");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"通知权限被拒绝: {error?.LocalizedDescription}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"请求通知权限失败: {ex.Message}");
        }
    }
}
