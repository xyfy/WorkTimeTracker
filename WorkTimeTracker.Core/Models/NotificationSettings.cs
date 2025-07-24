using System;

namespace WorkTimeTracker.Core.Models
{
    public class NotificationSettings
    {
        public bool VoiceReminderEnabled { get; set; } = true;
        public bool SystemNotificationEnabled { get; set; } = true;
        public bool ForegroundPopupEnabled { get; set; } = false;
        public bool DesktopNotificationEnabled { get; set; } = true;
        
        public string CustomVoiceMessage { get; set; } = string.Empty;
        public int ReminderFrequencyMinutes { get; set; } = 5;
        
        // 免打扰时段
        public bool DoNotDisturbEnabled { get; set; } = false;
        public TimeSpan DoNotDisturbStartTime { get; set; } = new TimeSpan(22, 0, 0); // 22:00
        public TimeSpan DoNotDisturbEndTime { get; set; } = new TimeSpan(8, 0, 0);   // 08:00
        
        // 声音设置
        public double VoiceVolume { get; set; } = 0.8;
        public string VoiceLanguage { get; set; } = "zh-CN";
        
        // 通知内容定制
        public string WorkStartMessage { get; set; } = "开始工作";
        public string WorkEndMessage { get; set; } = "工作结束";
        public string RestStartMessage { get; set; } = "开始休息";
        public string RestEndMessage { get; set; } = "休息结束";
    }
}
