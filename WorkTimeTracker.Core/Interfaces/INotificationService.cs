using System.Threading.Tasks;
using WorkTimeTracker.Core.Models;

namespace WorkTimeTracker.Core.Interfaces
{
    public interface INotificationService
    {
        NotificationSettings Settings { get; }
        
        Task LoadSettingsAsync();
        Task SaveSettingsAsync();
        
        Task ShowWorkStartNotificationAsync();
        Task ShowWorkEndNotificationAsync();
        Task ShowRestStartNotificationAsync();
        Task ShowRestEndNotificationAsync();
        Task ShowCustomNotificationAsync(string message);
        
        bool IsInDoNotDisturbPeriod();
    }
}
