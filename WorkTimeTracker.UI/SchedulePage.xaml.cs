using System.Collections.ObjectModel;
using WorkTimeTracker.Data;
using WorkTimeTracker.Core.Models;

namespace WorkTimeTracker.UI;

public partial class SchedulePage : ContentPage
{
    // 使用 ObservableCollection 绑定时间表数据
    public ObservableCollection<WorkRecordViewModel> WorkRecords { get; set; } = new();

    private readonly WorkRecordDatabase _workRecordDb;

    public SchedulePage(WorkRecordDatabase workRecordDb)
    {
        InitializeComponent();
        _workRecordDb = workRecordDb;
        RecordsListView.ItemsSource = WorkRecords;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadWorkRecordsAsync();
    }

    // 异步加载数据的方法
    private async Task LoadWorkRecordsAsync()
    {
        // 假设 WorkRecordDatabase 提供获取今日及以往记录的方法
        // 如无此方法，请根据实际情况调整数据加载逻辑
        var records = await _workRecordDb.GetAllWorkRecordsAsync();
        WorkRecords.Clear();
        foreach (var record in records)
        {
            WorkRecords.Add(new WorkRecordViewModel
            {
                Key = record.Day, // 修改此处：使用 record.Day 而非 record.DateKey
                DisplayDetail = $"工作: {record.WorkSeconds/60} 分, 休息: {record.RestSeconds/60} 分"
            });
        }
    }
}

// 简单视图模型，便于绑定
public class WorkRecordViewModel
{
    public string Key { get; set; } = string.Empty;
    public string DisplayDetail { get; set; } = string.Empty;
}
