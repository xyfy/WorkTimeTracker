# WorkTimeTracker UI 自动化测试

这是 WorkTimeTracker 项目的 UI 自动化测试套件，使用跨平台测试框架来验证应用程序的用户界面功能。

## 📋 概述

本测试项目采用页面对象模式（Page Object Model），提供全面的 UI 自动化测试覆盖：

### 🎯 测试范围

- **主页面测试** (`MainPageUITests`)
  - 页面加载验证
  - 工作会话管理（开始/停止工作）
  - 表单输入验证
  - 页面导航功能

- **通知设置测试** (`NotificationSettingsUITests`)
  - 通知开关切换
  - 提醒类型设置
  - 音量和语言配置
  - 免打扰模式设置
  - 设置保存和重置

- **集成测试** (`AppIntegrationTests`)
  - 完整用户工作流程
  - 跨页面导航
  - 多个工作会话测试
  - 错误恢复验证
  - 性能基准测试

## 🏗️ 项目结构

```
WorkTimeTracker.UITests/
├── Base/
│   └── UITestBase.cs              # 测试基类
├── PageObjects/
│   ├── Base/
│   │   └── PageObjectBase.cs      # 页面对象基类
│   ├── MainPageObject.cs          # 主页面对象
│   └── NotificationSettingsPageObject.cs  # 通知设置页面对象
├── Tests/
│   ├── MainPageUITests.cs         # 主页面测试
│   ├── NotificationSettingsUITests.cs     # 通知设置测试
│   └── AppIntegrationTests.cs     # 集成测试
├── Helpers/
│   └── TestDataGenerator.cs       # 测试数据生成器
├── TestData/
│   └── test-config.json          # 测试配置
├── Screenshots/                   # 测试截图目录
├── Reports/                      # 测试报告目录
├── run-ui-tests.sh              # Linux/macOS 运行脚本
└── run-ui-tests.bat             # Windows 运行脚本
```

## 🛠️ 技术栈

- **测试框架**: xUnit
- **UI 自动化**: Appium WebDriver + Selenium
- **断言库**: FluentAssertions
- **测试数据**: Bogus
- **日志记录**: Serilog
- **截图和报告**: 自定义实现

## 🚀 快速开始

### 前提条件

1. **.NET 9.0 SDK** 或更高版本
2. **被测试的 WorkTimeTracker 应用程序** 已构建并可运行

### 安装和设置

1. 克隆项目并进入测试目录：
   ```bash
   cd WorkTimeTracker.UITests
   ```

2. 还原 NuGet 包：
   ```bash
   dotnet restore
   ```

3. 构建测试项目：
   ```bash
   dotnet build
   ```

## 🧪 运行测试

### 方式一：使用 .NET CLI

```bash
# 运行所有测试
dotnet test

# 运行特定测试类
dotnet test --filter "ClassName~MainPageUITests"

# 运行匹配模式的测试
dotnet test --filter "Name~MainPage*"

# 详细输出
dotnet test --verbosity detailed

# 生成测试报告
dotnet test --logger "trx;LogFileName=TestResults.xml"
```

### 方式二：使用便捷脚本

#### Linux/macOS:
```bash
# 赋予执行权限
chmod +x run-ui-tests.sh

# 运行所有测试
./run-ui-tests.sh

# 运行特定测试模式
./run-ui-tests.sh -t "MainPage*"

# 详细输出 + 报告 + 截图
./run-ui-tests.sh -v -r -s

# 显示帮助
./run-ui-tests.sh -h
```

#### Windows:
```cmd
REM 运行所有测试
run-ui-tests.bat

REM 运行特定测试类
run-ui-tests.bat -c "MainPageUITests"

REM 详细输出 + 报告 + 截图
run-ui-tests.bat -v -r -s
```

### 脚本选项说明

- `-t, --test <pattern>`: 运行匹配模式的测试
- `-c, --class <classname>`: 运行指定测试类
- `-v, --verbose`: 详细输出
- `-r, --report`: 生成测试报告
- `-s, --screenshots`: 启用截图
- `-h, --help`: 显示帮助信息

## 📊 测试报告和输出

### 测试报告
- **位置**: `Reports/` 目录
- **格式**: XML (TRX) 格式
- **命名**: `test_report_YYYYMMDD_HHMMSS.xml`

### 截图
- **位置**: `Screenshots/` 目录
- **格式**: PNG 图片
- **命名**: `TestName_StepName_YYYYMMDD_HHMMSS.png`

### 日志文件
- **位置**: `Screenshots/` 目录
- **格式**: 滚动日志文件
- **命名**: `ui-tests-YYYYMMDD.log`

## 🔧 配置

### 测试配置文件 (`TestData/test-config.json`)

```json
{
  "TestSettings": {
    "DefaultTimeout": 10,
    "ScreenshotOnFailure": true,
    "ScreenshotOnSuccess": false,
    "LogLevel": "Information",
    "RetryCount": 2,
    "RetryDelay": 1000
  },
  "AppSettings": {
    "StartupTimeout": 30,
    "AppPath": "",
    "AppArguments": "",
    "CloseAppAfterTest": true
  },
  "PerformanceThresholds": {
    "MaxStartupTime": 30000,
    "MaxPageLoadTime": 10000,
    "MaxButtonResponseTime": 5000
  }
}
```

## 📝 编写测试

### 页面对象模式示例

```csharp
public class NewPageObject : PageObjectBase
{
    private const string ELEMENT_SELECTOR = "ElementSelector";
    
    public NewPageObject(IHost? appHost, ILogger logger) : base(appHost, logger)
    {
    }
    
    public override async Task<bool> IsPageLoadedAsync()
    {
        return await IsElementVisibleAsync(ELEMENT_SELECTOR);
    }
    
    public async Task<bool> ClickButtonAsync()
    {
        return await ClickElementAsync(ELEMENT_SELECTOR);
    }
}
```

### 测试类示例

```csharp
public class NewPageUITests : UITestBase
{
    [Fact]
    public async Task NewPage_ShouldLoadSuccessfully()
    {
        // Arrange
        await StartAppAsync();
        var newPage = new NewPageObject(_appHost, _logger);
        
        // Act
        var isLoaded = await newPage.WaitForPageLoadAsync();
        
        // Assert
        isLoaded.Should().BeTrue("新页面应该成功加载");
        
        // 截图
        await TakeScreenshotAsync("NewPage_LoadSuccessfully");
    }
}
```

## 🎨 最佳实践

### 1. 页面对象设计
- 每个页面创建对应的页面对象类
- 使用常量定义元素选择器
- 封装页面操作为方法
- 提供清晰的页面状态检查

### 2. 测试设计
- 使用描述性的测试方法名
- 遵循 Arrange-Act-Assert 模式
- 在关键步骤添加截图
- 使用流畅断言（FluentAssertions）

### 3. 数据管理
- 使用 TestDataGenerator 生成测试数据
- 避免硬编码测试值
- 包含边界值和无效数据测试

### 4. 错误处理
- 实现重试机制
- 提供清晰的错误消息
- 在失败时自动截图

## 🔍 故障排除

### 常见问题

1. **测试超时**
   - 检查应用程序是否正在运行
   - 增加超时设置
   - 验证元素选择器是否正确

2. **元素未找到**
   - 确认页面已完全加载
   - 检查元素选择器的准确性
   - 添加显式等待

3. **截图失败**
   - 确保有足够的磁盘空间
   - 检查 Screenshots 目录权限
   - 验证图形环境可用性

### 调试技巧

1. **启用详细日志**:
   ```bash
   dotnet test --verbosity detailed
   ```

2. **单独运行失败的测试**:
   ```bash
   dotnet test --filter "MethodName~SpecificTestMethod"
   ```

3. **查看测试输出**:
   ```bash
   ./run-ui-tests.sh -v -s
   ```

## 🚀 持续集成

### GitHub Actions 示例

```yaml
name: UI Tests

on: [push, pull_request]

jobs:
  ui-tests:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
        
    - name: Restore dependencies
      run: dotnet restore WorkTimeTracker.UITests
      
    - name: Build tests
      run: dotnet build WorkTimeTracker.UITests --no-restore
      
    - name: Run UI tests
      run: dotnet test WorkTimeTracker.UITests --no-build --verbosity normal
      
    - name: Upload test results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: test-results
        path: WorkTimeTracker.UITests/Reports/
```

## 📈 扩展和贡献

### 添加新测试

1. 在 `Tests/` 目录创建新的测试类
2. 继承 `UITestBase` 基类
3. 如需要，在 `PageObjects/` 目录创建对应的页面对象
4. 更新测试配置文件

### 贡献指南

1. Fork 项目
2. 创建功能分支
3. 编写测试和文档
4. 提交 Pull Request

## 📄 许可证

本项目遵循 MIT 许可证。详见 [LICENSE](../LICENSE) 文件。

---

**注意**: 这是一个基于模拟的 UI 自动化测试框架。在实际生产环境中，需要集成真实的 UI 自动化驱动程序（如 Appium）来与实际的 MAUI 应用程序进行交互。
