@echo off
setlocal enabledelayedexpansion

REM WorkTimeTracker UI 自动化测试运行脚本 (Windows)
REM 使用方法: run-ui-tests.bat [选项]

REM 配置变量
set "SCRIPT_DIR=%~dp0"
set "PROJECT_ROOT=%SCRIPT_DIR%.."
set "UI_TESTS_PROJECT=%PROJECT_ROOT%\WorkTimeTracker.UITests"
set "REPORTS_DIR=%UI_TESTS_PROJECT%\Reports"
set "SCREENSHOTS_DIR=%UI_TESTS_PROJECT%\Screenshots"

REM 创建必要的目录
if not exist "%REPORTS_DIR%" mkdir "%REPORTS_DIR%"
if not exist "%SCREENSHOTS_DIR%" mkdir "%SCREENSHOTS_DIR%"

REM 默认参数
set "VERBOSE="
set "TEST_PATTERN="
set "TEST_CLASS="
set "GENERATE_REPORT=false"
set "ENABLE_SCREENSHOTS=false"

REM 解析命令行参数
:parse_args
if "%~1"=="" goto :args_done
if "%~1"=="-t" (
    set "TEST_PATTERN=%~2"
    shift
    shift
    goto :parse_args
)
if "%~1"=="--test" (
    set "TEST_PATTERN=%~2"
    shift
    shift
    goto :parse_args
)
if "%~1"=="-c" (
    set "TEST_CLASS=%~2"
    shift
    shift
    goto :parse_args
)
if "%~1"=="--class" (
    set "TEST_CLASS=%~2"
    shift
    shift
    goto :parse_args
)
if "%~1"=="-v" (
    set "VERBOSE=--verbosity detailed"
    shift
    goto :parse_args
)
if "%~1"=="--verbose" (
    set "VERBOSE=--verbosity detailed"
    shift
    goto :parse_args
)
if "%~1"=="-r" (
    set "GENERATE_REPORT=true"
    shift
    goto :parse_args
)
if "%~1"=="--report" (
    set "GENERATE_REPORT=true"
    shift
    goto :parse_args
)
if "%~1"=="-s" (
    set "ENABLE_SCREENSHOTS=true"
    shift
    goto :parse_args
)
if "%~1"=="--screenshots" (
    set "ENABLE_SCREENSHOTS=true"
    shift
    goto :parse_args
)
if "%~1"=="-h" goto :show_help
if "%~1"=="--help" goto :show_help
echo 未知选项: %~1
goto :show_help

:show_help
echo WorkTimeTracker UI 自动化测试运行脚本
echo.
echo 使用方法: %~nx0 [选项]
echo.
echo 选项:
echo   -t, --test ^<pattern^>     运行匹配模式的测试 (例如: MainPage*)
echo   -c, --class ^<classname^>  运行指定测试类
echo   -v, --verbose            详细输出
echo   -r, --report             生成测试报告
echo   -s, --screenshots        启用截图
echo   -h, --help               显示帮助信息
echo.
echo 示例:
echo   %~nx0                                    # 运行所有测试
echo   %~nx0 -t MainPage*                      # 运行主页面相关测试
echo   %~nx0 -c MainPageUITests                # 运行主页面测试类
echo   %~nx0 -v -r -s                          # 详细输出+报告+截图
exit /b 0

:args_done

REM 构建测试命令
set "TEST_COMMAND=dotnet test "%UI_TESTS_PROJECT%""

if not "%TEST_PATTERN%"=="" (
    set "TEST_COMMAND=!TEST_COMMAND! --filter "Name~%TEST_PATTERN%""
) else if not "%TEST_CLASS%"=="" (
    set "TEST_COMMAND=!TEST_COMMAND! --filter "ClassName~%TEST_CLASS%""
)

if not "%VERBOSE%"=="" (
    set "TEST_COMMAND=!TEST_COMMAND! %VERBOSE%"
)

if "%GENERATE_REPORT%"=="true" (
    for /f "tokens=1-4 delims=/ " %%a in ('date /t') do set "DATE_PART=%%c%%a%%b"
    for /f "tokens=1-2 delims=: " %%a in ('time /t') do set "TIME_PART=%%a%%b"
    set "TIMESTAMP=!DATE_PART!_!TIME_PART!"
    set "REPORT_FILE=%REPORTS_DIR%\test_report_!TIMESTAMP!.xml"
    set "TEST_COMMAND=!TEST_COMMAND! --logger "trx;LogFileName=!REPORT_FILE!""
)

REM 设置环境变量
if "%ENABLE_SCREENSHOTS%"=="true" (
    set "UI_TEST_SCREENSHOTS_ENABLED=true"
)

REM 打印配置信息
echo ============================================
echo WorkTimeTracker UI 自动化测试
echo ============================================
echo 项目路径: %PROJECT_ROOT%
echo 测试项目: %UI_TESTS_PROJECT%
echo 报告目录: %REPORTS_DIR%
echo 截图目录: %SCREENSHOTS_DIR%

if not "%TEST_PATTERN%"=="" echo 测试模式: %TEST_PATTERN%
if not "%TEST_CLASS%"=="" echo 测试类: %TEST_CLASS%
if "%GENERATE_REPORT%"=="true" echo 生成报告: 是
if "%ENABLE_SCREENSHOTS%"=="true" echo 启用截图: 是

echo ============================================

REM 检查先决条件
echo 检查先决条件...

REM 检查 .NET SDK
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo 错误: 未找到 dotnet CLI。请安装 .NET SDK。
    exit /b 1
)
echo ✓ .NET SDK 可用

REM 检查项目文件
if not exist "%UI_TESTS_PROJECT%\WorkTimeTracker.UITests.csproj" (
    echo 错误: 未找到 UI 测试项目文件。
    exit /b 1
)
echo ✓ UI 测试项目存在

REM 还原 NuGet 包
echo 还原 NuGet 包...
cd /d "%UI_TESTS_PROJECT%"
dotnet restore
if errorlevel 1 (
    echo ✗ NuGet 包还原失败
    exit /b 1
)
echo ✓ NuGet 包还原成功

REM 构建测试项目
echo 构建测试项目...
dotnet build --no-restore
if errorlevel 1 (
    echo ✗ 测试项目构建失败
    exit /b 1
)
echo ✓ 测试项目构建成功

REM 运行测试
echo 运行 UI 自动化测试...
echo 执行命令: !TEST_COMMAND!
echo ============================================

REM 记录开始时间
set "START_TIME=%TIME%"

REM 执行测试命令
call !TEST_COMMAND!
set "TEST_EXIT_CODE=%ERRORLEVEL%"

REM 记录结束时间
set "END_TIME=%TIME%"

echo ============================================

REM 输出结果
if %TEST_EXIT_CODE%==0 (
    echo ✓ 所有测试通过！
) else (
    echo ✗ 部分测试失败
)

REM 输出报告和截图位置
if "%GENERATE_REPORT%"=="true" echo 测试报告保存在: %REPORTS_DIR%
if "%ENABLE_SCREENSHOTS%"=="true" echo 测试截图保存在: %SCREENSHOTS_DIR%

REM 显示日志文件位置
if exist "%SCREENSHOTS_DIR%\*.log" (
    echo 测试日志:
    for %%f in ("%SCREENSHOTS_DIR%\*.log") do echo   - %%f
)

echo ============================================

exit /b %TEST_EXIT_CODE%
