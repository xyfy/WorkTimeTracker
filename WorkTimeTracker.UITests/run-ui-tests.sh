#!/bin/bash

# WorkTimeTracker UI 自动化测试运行脚本
# 使用方法: ./run-ui-tests.sh [选项]

set -e

# 配置变量
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UI_TESTS_PROJECT="$PROJECT_ROOT/WorkTimeTracker.UITests"
REPORTS_DIR="$UI_TESTS_PROJECT/Reports"
SCREENSHOTS_DIR="$UI_TESTS_PROJECT/Screenshots"

# 创建必要的目录
mkdir -p "$REPORTS_DIR"
mkdir -p "$SCREENSHOTS_DIR"

# 颜色输出
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 打印带颜色的消息
print_message() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# 显示帮助信息
show_help() {
    echo "WorkTimeTracker UI 自动化测试运行脚本"
    echo ""
    echo "使用方法: $0 [选项]"
    echo ""
    echo "选项:"
    echo "  -t, --test <pattern>     运行匹配模式的测试 (例如: MainPage*)"
    echo "  -c, --class <classname>  运行指定测试类"
    echo "  -v, --verbose            详细输出"
    echo "  -r, --report             生成测试报告"
    echo "  -s, --screenshots        启用截图"
    echo "  -h, --help               显示帮助信息"
    echo ""
    echo "示例:"
    echo "  $0                                    # 运行所有测试"
    echo "  $0 -t MainPage*                      # 运行主页面相关测试"
    echo "  $0 -c MainPageUITests                # 运行主页面测试类"
    echo "  $0 -v -r -s                          # 详细输出+报告+截图"
}

# 默认参数
VERBOSE=""
TEST_PATTERN=""
TEST_CLASS=""
GENERATE_REPORT=false
ENABLE_SCREENSHOTS=false

# 解析命令行参数
while [[ $# -gt 0 ]]; do
    case $1 in
        -t|--test)
            TEST_PATTERN="$2"
            shift 2
            ;;
        -c|--class)
            TEST_CLASS="$2"
            shift 2
            ;;
        -v|--verbose)
            VERBOSE="--verbosity detailed"
            shift
            ;;
        -r|--report)
            GENERATE_REPORT=true
            shift
            ;;
        -s|--screenshots)
            ENABLE_SCREENSHOTS=true
            shift
            ;;
        -h|--help)
            show_help
            exit 0
            ;;
        *)
            print_message $RED "未知选项: $1"
            show_help
            exit 1
            ;;
    esac
done

# 构建测试命令
TEST_COMMAND="dotnet test \"$UI_TESTS_PROJECT\""

if [[ -n "$TEST_PATTERN" ]]; then
    TEST_COMMAND="$TEST_COMMAND --filter \"Name~$TEST_PATTERN\""
elif [[ -n "$TEST_CLASS" ]]; then
    TEST_COMMAND="$TEST_COMMAND --filter \"ClassName~$TEST_CLASS\""
fi

if [[ -n "$VERBOSE" ]]; then
    TEST_COMMAND="$TEST_COMMAND $VERBOSE"
fi

if [[ "$GENERATE_REPORT" == true ]]; then
    TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
    REPORT_FILE="$REPORTS_DIR/test_report_$TIMESTAMP.xml"
    TEST_COMMAND="$TEST_COMMAND --logger \"trx;LogFileName=$REPORT_FILE\""
fi

# 设置环境变量
if [[ "$ENABLE_SCREENSHOTS" == true ]]; then
    export UI_TEST_SCREENSHOTS_ENABLED=true
fi

# 打印配置信息
print_message $BLUE "============================================"
print_message $BLUE "WorkTimeTracker UI 自动化测试"
print_message $BLUE "============================================"
print_message $YELLOW "项目路径: $PROJECT_ROOT"
print_message $YELLOW "测试项目: $UI_TESTS_PROJECT"
print_message $YELLOW "报告目录: $REPORTS_DIR"
print_message $YELLOW "截图目录: $SCREENSHOTS_DIR"

if [[ -n "$TEST_PATTERN" ]]; then
    print_message $YELLOW "测试模式: $TEST_PATTERN"
fi

if [[ -n "$TEST_CLASS" ]]; then
    print_message $YELLOW "测试类: $TEST_CLASS"
fi

if [[ "$GENERATE_REPORT" == true ]]; then
    print_message $YELLOW "生成报告: 是"
fi

if [[ "$ENABLE_SCREENSHOTS" == true ]]; then
    print_message $YELLOW "启用截图: 是"
fi

print_message $BLUE "============================================"

# 检查先决条件
print_message $BLUE "检查先决条件..."

# 检查 .NET SDK
if ! command -v dotnet &> /dev/null; then
    print_message $RED "错误: 未找到 dotnet CLI。请安装 .NET SDK。"
    exit 1
fi

print_message $GREEN "✓ .NET SDK 可用"

# 检查项目文件
if [[ ! -f "$UI_TESTS_PROJECT/WorkTimeTracker.UITests.csproj" ]]; then
    print_message $RED "错误: 未找到 UI 测试项目文件。"
    exit 1
fi

print_message $GREEN "✓ UI 测试项目存在"

# 还原 NuGet 包
print_message $BLUE "还原 NuGet 包..."
cd "$UI_TESTS_PROJECT"
dotnet restore
if [[ $? -eq 0 ]]; then
    print_message $GREEN "✓ NuGet 包还原成功"
else
    print_message $RED "✗ NuGet 包还原失败"
    exit 1
fi

# 构建测试项目
print_message $BLUE "构建测试项目..."
dotnet build --no-restore
if [[ $? -eq 0 ]]; then
    print_message $GREEN "✓ 测试项目构建成功"
else
    print_message $RED "✗ 测试项目构建失败"
    exit 1
fi

# 运行测试
print_message $BLUE "运行 UI 自动化测试..."
print_message $YELLOW "执行命令: $TEST_COMMAND"
print_message $BLUE "============================================"

# 记录开始时间
START_TIME=$(date +%s)

# 执行测试命令
eval $TEST_COMMAND
TEST_EXIT_CODE=$?

# 计算执行时间
END_TIME=$(date +%s)
DURATION=$((END_TIME - START_TIME))

print_message $BLUE "============================================"

# 输出结果
if [[ $TEST_EXIT_CODE -eq 0 ]]; then
    print_message $GREEN "✓ 所有测试通过！"
    print_message $GREEN "执行时间: ${DURATION} 秒"
else
    print_message $RED "✗ 部分测试失败"
    print_message $RED "执行时间: ${DURATION} 秒"
fi

# 输出报告和截图位置
if [[ "$GENERATE_REPORT" == true ]]; then
    print_message $YELLOW "测试报告保存在: $REPORTS_DIR"
fi

if [[ "$ENABLE_SCREENSHOTS" == true ]]; then
    print_message $YELLOW "测试截图保存在: $SCREENSHOTS_DIR"
fi

# 显示日志文件位置
LOG_FILES=$(find "$SCREENSHOTS_DIR" -name "*.log" -type f 2>/dev/null || true)
if [[ -n "$LOG_FILES" ]]; then
    print_message $YELLOW "测试日志:"
    echo "$LOG_FILES" | while read -r log_file; do
        print_message $YELLOW "  - $log_file"
    done
fi

print_message $BLUE "============================================"

exit $TEST_EXIT_CODE
