#!/bin/bash

# WorkTimeTracker Release Build Script
# 构建 macOS 版本的工作计时器应用

set -e

PROJECT_PATH="WorkTimeTracker.UI/WorkTimeTracker.UI.csproj"
OUTPUT_DIR="publish"
APP_NAME="工作计时器"

echo "🚀 开始构建工作计时器发布版本..."

# 清理输出目录
if [ -d "$OUTPUT_DIR" ]; then
    echo "🧹 清理之前的构建文件..."
    rm -rf "$OUTPUT_DIR"
fi
mkdir -p "$OUTPUT_DIR"

# 检查构建环境
echo "🔍 检查构建环境..."
if [[ "$OSTYPE" != "darwin"* ]]; then
    echo "❌ 此脚本仅支持在 macOS 系统上运行"
    exit 1
fi

echo "✅ macOS 系统检测通过"

# 清理之前的构建
echo "🧽 清理项目..."
dotnet clean "$PROJECT_PATH" -c Release --verbosity quiet

# 构建 macOS 版本
echo "🍎 构建 macOS 版本..."
dotnet publish "$PROJECT_PATH" \
    -f net9.0-maccatalyst \
    -c Release \
    --verbosity minimal

if [ $? -eq 0 ]; then
    echo "✅ macOS 版本构建成功！"
    
    # 复制应用到发布目录
    echo "📦 打包发布版本..."
    SOURCE_PATH="WorkTimeTracker.UI/bin/Release/net9.0-maccatalyst/${APP_NAME}.app"
    
    if [ -d "$SOURCE_PATH" ]; then
        cp -R "$SOURCE_PATH" "$OUTPUT_DIR/"
        
        # 验证签名
        echo "🔐 验证应用签名..."
        codesign -dv "$OUTPUT_DIR/${APP_NAME}.app" > "$OUTPUT_DIR/codesign-info.txt" 2>&1
        
        # 获取应用信息
        APP_SIZE=$(du -sh "$OUTPUT_DIR/${APP_NAME}.app" | cut -f1)
        
        echo ""
        echo "🎉 发布构建完成！"
        echo "📊 应用信息:"
        echo "   名称: $APP_NAME"
        echo "   大小: $APP_SIZE"
        echo "   位置: $OUTPUT_DIR/${APP_NAME}.app"
        echo ""
        echo "📝 签名信息已保存到: $OUTPUT_DIR/codesign-info.txt"
        echo ""
        echo "🚀 安装说明:"
        echo "1. 将 ${APP_NAME}.app 拖入"应用程序"文件夹"
        echo "2. 首次运行时右键点击选择"打开""
        echo "3. 享受使用工作计时器！"
        echo ""
        echo "✨ 发布成功完成！"
    else
        echo "❌ 构建的应用文件未找到: $SOURCE_PATH"
        exit 1
    fi
else
    echo "❌ macOS 版本构建失败！"
    exit 1
fi
