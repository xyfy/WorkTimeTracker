using Bogus;
using System;
using System.Collections.Generic;

namespace WorkTimeTracker.UITests.Helpers
{
    /// <summary>
    /// UI 自动化测试数据生成器
    /// 注意：UI 测试不依赖应用内部模型，而是生成用于 UI 交互的测试数据
    /// </summary>
    public static class TestDataGenerator
    {
        private static readonly Faker _faker = new Faker("zh_CN");

        /// <summary>
        /// 生成工作描述测试数据
        /// </summary>
        public static string GenerateWorkDescription()
        {
            return _faker.Lorem.Sentence(3, 10);
        }

        /// <summary>
        /// 生成项目名称测试数据
        /// </summary>
        public static string GenerateProjectName()
        {
            return _faker.Company.CompanyName();
        }

        /// <summary>
        /// 生成测试时间范围
        /// </summary>
        public static (DateTime start, DateTime end) GenerateTimeRange()
        {
            var start = _faker.Date.Recent(7).Date.AddHours(_faker.Random.Int(8, 12));
            var end = start.AddHours(_faker.Random.Int(4, 8));
            return (start, end);
        }

        /// <summary>
        /// 生成通知设置测试数据
        /// </summary>
        public static class NotificationTestData
        {
            public static bool GenerateIsEnabled() => _faker.Random.Bool();
            
            public static bool GenerateWorkStartReminder() => _faker.Random.Bool();
            
            public static bool GenerateWorkEndReminder() => _faker.Random.Bool();
            
            public static bool GenerateBreakReminder() => _faker.Random.Bool();
            
            public static float GenerateVolume() => _faker.Random.Float(0.1f, 1.0f);
            
            public static string GenerateLanguage() => _faker.PickRandom("zh-CN", "en-US", "ja-JP");
            
            public static string GenerateDoNotDisturbStart() => _faker.Date.Soon().ToString("HH:mm");
            
            public static string GenerateDoNotDisturbEnd() => _faker.Date.Soon().ToString("HH:mm");
            
            public static bool GenerateIsDoNotDisturbEnabled() => _faker.Random.Bool();
            
            public static string GenerateCustomMessage() => _faker.Lorem.Sentence();
            
            public static int GenerateReminderFrequencyMinutes() => _faker.Random.Int(5, 60);
            
            public static List<string> GenerateNotificationTypes()
            {
                var types = new List<string> { "Sound", "Popup", "Vibration" };
                return _faker.Random.ListItems(types, _faker.Random.Int(1, 3)).ToList();
            }
        }

        /// <summary>
        /// 生成多个工作描述
        /// </summary>
        public static List<string> GenerateWorkDescriptions(int count = 5)
        {
            var descriptions = new List<string>();
            for (int i = 0; i < count; i++)
            {
                descriptions.Add(GenerateWorkDescription());
            }
            return descriptions;
        }

        /// <summary>
        /// 生成多个项目名称
        /// </summary>
        public static List<string> GenerateProjectNames(int count = 5)
        {
            var projectNames = new List<string>();
            for (int i = 0; i < count; i++)
            {
                projectNames.Add(GenerateProjectName());
            }
            return projectNames;
        }

        /// <summary>
        /// 生成随机数字字符串（用于频率等输入）
        /// </summary>
        public static string GenerateNumberString(int min = 1, int max = 100)
        {
            return _faker.Random.Int(min, max).ToString();
        }

        /// <summary>
        /// 生成随机时间字符串
        /// </summary>
        public static string GenerateTimeString()
        {
            return _faker.Date.Soon().ToString("HH:mm");
        }

        /// <summary>
        /// 生成测试用的长文本（用于边界测试）
        /// </summary>
        public static string GenerateLongText(int length = 1000)
        {
            return new string('X', length);
        }

        /// <summary>
        /// 生成无效输入数据（用于错误处理测试）
        /// </summary>
        public static class InvalidTestData
        {
            public static string EmptyString() => string.Empty;
            
            public static string NullString() => null!;
            
            public static string VeryLongString() => new string('A', 10000);
            
            public static string SpecialCharacters() => "!@#$%^&*()[]{}|\\:;\"'<>,.?/~`";
            
            public static string NegativeNumber() => "-1";
            
            public static string ZeroString() => "0";
            
            public static string InvalidTime() => "25:99";
        }
    }
}
