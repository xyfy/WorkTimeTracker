# WorkTimeTracker 项目

- [WorkTimeTracker 项目](#worktimetracker-项目)
  - [项目概述](#项目概述)
  - [项目结构](#项目结构)
  - [架构设计](#架构设计)
    - [1. WorkTimeTracker.Core（核心层）](#1-worktimetrackercore核心层)
    - [2. WorkTimeTracker.Data（数据层）](#2-worktimetrackerdata数据层)
    - [3. WorkTimeTracker.UI（表示层）](#3-worktimetrackerui表示层)
    - [4. WorkTimeTracker.Tests（测试层）](#4-worktimetrackertests测试层)
  - [核心功能](#核心功能)
    - [1. 工作时间管理](#1-工作时间管理)
    - [2. 提醒服务](#2-提醒服务)
    - [3. 数据存储](#3-数据存储)
  - [技术栈](#技术栈)
  - [开发环境设置](#开发环境设置)
  - [构建和运行](#构建和运行)
    - [开发模式运行](#开发模式运行)
    - [发布应用](#发布应用)
  - [依赖注入配置](#依赖注入配置)
  - [数据模型](#数据模型)
    - [WorkRecord](#workrecord)
  - [配置说明](#配置说明)
  - [支持的平台](#支持的平台)
  - [注意事项](#注意事项)
  - [开发文档](#开发文档)
  - [贡献](#贡献)
  - [许可证](#许可证)

这是一个基于 .NET MAUI 的多平台工作时间管理应用程序。

## 项目概述

WorkTimeTracker 是一个工作时间追踪应用，具有以下核心功能：

- 工作和休息时间的计时管理
- 语音提醒功能
- 工作记录的数据库存储
- 每日工作时间统计

## 项目结构

```text
.
├── WorkTimeTracker.UI/             # MAUI 前端项目
│   ├── App.xaml                   # 应用程序入口
│   ├── MainPage.xaml              # 主页面
│   ├── SchedulePage.xaml          # 计划页面
│   ├── MauiProgram.cs             # MAUI 程序配置
│   ├── Services/                  # UI 服务层
│   │   └── ReminderService.cs     # 提醒服务（UI层）
│   └── Platforms/                 # 平台特定代码
├── WorkTimeTracker.Core/          # 核心业务逻辑（.NET Standard 2.0）
│   ├── Models/                    # 业务模型
│   │   └── WorkRecord.cs          # 工作记录模型
│   ├── Interfaces/                # 接口定义
│   │   ├── IWorkTimeService.cs    # 工作时间服务接口
│   │   └── IWorkRecordRepository.cs # 工作记录仓库接口
│   └── Services/                  # 核心服务实现
│       └── WorkTimeService.cs     # 工作时间服务
├── WorkTimeTracker.Data/          # 数据访问层（.NET Standard 2.0）
│   ├── WorkRecordDatabase.cs      # SQLite 数据库访问
│   └── Repositories/              # 仓库实现
│       └── WorkRecordRepository.cs # 工作记录仓库
├── WorkTimeTracker.Tests/         # 单元测试项目
├── Docs/                          # 项目开发文档
│   ├── README.md                  # 文档说明
│   ├── PROJECT_COMPLETION_SUMMARY.md # 项目完成总结
│   ├── COMPILATION_FIXES.md       # 编译修复日志
│   └── PLATFORM_CONFIG_SUMMARY.md # 平台配置总结
└── PackagingGuide.md              # 打包指南
```

## 架构设计

项目采用分层架构设计，实现了前后端分离：

### 1. WorkTimeTracker.Core（核心层）

- **目标框架**: .NET Standard 2.0
- **职责**: 包含业务逻辑、模型定义和接口
- **特点**: 不依赖任何 UI 框架，纯业务逻辑
- **主要组件**:
  - `IWorkTimeService`: 工作时间管理服务接口
  - `WorkTimeService`: 核心业务逻辑实现
  - `WorkRecord`: 工作记录模型

### 2. WorkTimeTracker.Data（数据层）

- **目标框架**: .NET Standard 2.0
- **职责**: 数据持久化和仓库模式实现
- **特点**: 实现了仓库模式，便于单元测试
- **主要组件**:
  - `WorkRecordDatabase`: SQLite 数据库访问
  - `IWorkRecordRepository`: 数据仓库接口
  - `WorkRecordRepository`: 仓库模式实现

### 3. WorkTimeTracker.UI（表示层）

- **目标框架**: .NET 9.0 MAUI
- **职责**: 用户界面和平台特定功能
- **特点**: 仅处理 UI 逻辑和用户交互
- **主要组件**:
  - `MainPage`: 主界面
  - `ReminderService`: UI 层的提醒和语音服务

### 4. WorkTimeTracker.Tests（测试层）

- **目标框架**: .NET 8.0
- **职责**: 单元测试和集成测试
- **特点**: 可以独立测试核心业务逻辑

## 核心功能

### 1. 工作时间管理

- **开始/结束工作**: 通过 `MainPage` 的按钮控制工作状态
- **时间配置**: 可配置工作时长和休息时长（默认 50 分钟工作，10 分钟休息）
- **实时更新**: 每分钟更新一次当日工作时间显示

### 2. 提醒服务

`ReminderService` 类提供：

- 工作/休息周期管理
- 语音提醒功能（使用 Text-to-Speech）
- 数据库记录更新
- 可配置的工作和休息时长

### 3. 数据存储

使用 SQLite 数据库存储工作记录：

- `WorkRecord` 模型存储每日工作数据
- 记录工作秒数、休息秒数和总工作时间
- 支持异步数据访问

## 技术栈

- **.NET MAUI**: 跨平台应用框架 (Microsoft.Maui.Controls 9.0.80)
- **SQLite**: 本地数据库 (sqlite-net-pcl 1.9.172)
- **CommunityToolkit.Maui**: MAUI 社区工具包 (12.0.0)
- **Plugin.LocalNotification**: 本地通知插件 (12.0.1)
- **Microsoft.Extensions.Logging**: 日志记录 (9.0.6)

## 开发环境设置

1. 安装 .NET 9.0 SDK
2. 安装 Visual Studio 2022 或 Visual Studio Code
3. 安装 MAUI 工作负载：

   ```bash
   dotnet workload install maui
   ```

## 构建和运行

### 开发模式运行

```bash
dotnet run --project WorkTimeTracker.UI
```

### 发布应用

```bash
dotnet publish -c Release -f net9.0-maccatalyst -o ../publish WorkTimeTracker.UI
```

详细的打包指南请参考 [PackagingGuide.md](PackagingGuide.md)。

## 依赖注入配置

在 `MauiProgram.cs` 中配置了以下服务：

- `ReminderService`: 单例服务，提供工作提醒功能
- `WorkRecordDatabase`: 单例服务，管理 SQLite 数据库
- `MainPage`: 主页面服务

## 数据模型

### WorkRecord

存储每日工作记录的核心模型：

```csharp
public class WorkRecord
{
    public string Day { get; set; }          // 日期（主键）
    public int WorkSeconds { get; set; }     // 工作秒数
    public int RestSeconds { get; set; }     // 休息秒数
    public int TotalWorkSeconds { get; set; } // 总工作秒数
}
```

## 配置说明

应用使用 `Microsoft.Maui.Essentials.Preferences` 存储配置：

- `WorkDuration`: 工作时长（分钟）
- `RestDuration`: 休息时长（分钟）

## 支持的平台

- Android
- iOS
- macOS (Mac Catalyst)
- Windows

## 注意事项

1. 应用需要文本转语音权限用于语音提醒
2. 数据库文件存储在应用数据目录中
3. 配置信息持久化存储，应用重启后会恢复上次设置

## 开发文档

项目的所有开发和修复相关文档都存放在 `Docs/` 文件夹中，包括：

- **项目完成总结** - 完整的修复过程和结果汇总
- **编译修复日志** - 详细的编译错误修复记录
- **平台配置总结** - 多平台发布配置说明

这些文档记录了项目从存在多个编译错误到完全可构建和测试通过的完整修复过程，可作为后续维护和学习的重要参考。

## 贡献

欢迎提交 Issue 和 Pull Request 来改进这个项目。

## 许可证

本项目使用适当的开源许可证。具体信息请查看项目中的 LICENSE 文件。
