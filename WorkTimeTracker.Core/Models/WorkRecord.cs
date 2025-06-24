using SQLite;

namespace WorkTimeTracker.Core.Models
{
    public class WorkRecord
    {
        [PrimaryKey]
        public string Day { get; set; } = string.Empty;
        public int WorkSeconds { get; set; } = 0;
        public int RestSeconds { get; set; } = 0;
        public int TotalWorkSeconds { get; set; } = 0;

        public WorkRecord() { }

        public WorkRecord(string day, int workSeconds, int restSeconds)
        {
            Day = day;
            WorkSeconds = workSeconds;
            RestSeconds = restSeconds;
            TotalWorkSeconds = workSeconds + restSeconds;
        }
    }
}
