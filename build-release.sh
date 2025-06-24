#!/bin/bash

# WorkTimeTracker Multi-Platform Build Script
# 确保在项目根目录运行此脚本

set -e

PROJECT_PATH="WorkTimeTracker.UI/WorkTimeTracker.UI.csproj"
OUTPUT_DIR="publish"
APP_NAME="WorkTimeTracker"

echo "开始构建 WorkTimeTracker 多平台版本..."

# 清理输出目录
if [ -d "$OUTPUT_DIR" ]; then
    rm -rf "$OUTPUT_DIR"
fi
mkdir -p "$OUTPUT_DIR"

# 检查是否有 Android SDK
echo "检查构建环境..."
BUILD_ANDROID=false
BUILD_IOS=false
BUILD_WINDOWS=false

# 检查 Android SDK
if [ -n "$ANDROID_SDK_ROOT" ] || [ -n "$ANDROID_HOME" ] || [ -d "$HOME/Android/Sdk" ]; then
    BUILD_ANDROID=true
    echo "✓ Android SDK 已找到，将构建 Android 版本"
else
    echo "⚠ Android SDK 未找到，跳过 Android 构建"
fi

# 检查 iOS/macOS 支持
if [[ "$OSTYPE" == "darwin"* ]]; then
    BUILD_IOS=true
    echo "✓ macOS 系统，将构建 iOS 和 macOS 版本"
else
    echo "⚠ 非 macOS 系统，跳过 iOS 和 macOS 构建"
fi

# 检查 Windows 支持
if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]] || [[ "$OSTYPE" == "win32" ]]; then
    BUILD_WINDOWS=true
    echo "✓ Windows 系统，将构建 Windows 版本"
fi

# 构建 Android (APK)
if [ "$BUILD_ANDROID" = true ]; then
    echo "构建 Android 版本..."
    dotnet publish "$PROJECT_PATH" \
        -f net9.0-android \
        -c Release \
        -o "$OUTPUT_DIR/android" \
        -p:AndroidSigningKeyStore="" \
        -p:AndroidSigningKeyAlias="" \
        -p:AndroidSigningStorePass="" \
        -p:AndroidSigningKeyPass=""
    
    if [ $? -eq 0 ]; then
        echo "✓ Android 构建成功"
    else
        echo "✗ Android 构建失败"
    fi
fi

# 构建 iOS/macOS
if [ "$BUILD_IOS" = true ]; then
    echo "构建 macOS Catalyst 版本..."
    dotnet publish "$PROJECT_PATH" \
        -f net9.0-maccatalyst \
        -c Release \
        -o "$OUTPUT_DIR/maccatalyst" \
        -p:CreatePackage=true
    
    if [ $? -eq 0 ]; then
        echo "✓ macOS Catalyst 构建成功"
    else
        echo "✗ macOS Catalyst 构建失败"
    fi
    
    echo "构建 iOS 版本..."
    dotnet publish "$PROJECT_PATH" \
        -f net9.0-ios \
        -c Release \
        -o "$OUTPUT_DIR/ios" \
        -p:ArchiveOnBuild=true
    
    if [ $? -eq 0 ]; then
        echo "✓ iOS 构建成功"
    else
        echo "✗ iOS 构建失败"
    fi
fi

# 构建 Windows
if [ "$BUILD_WINDOWS" = true ]; then
    echo "构建 Windows 版本..."
    dotnet publish "$PROJECT_PATH" \
        -f net9.0-windows10.0.19041.0 \
        -c Release \
        -o "$OUTPUT_DIR/windows" \
        -p:PublishProfile=FolderProfile \
        -p:Platform=x64
    
    if [ $? -eq 0 ]; then
        echo "✓ Windows 构建成功"
    else
        echo "✗ Windows 构建失败"
    fi
fi

echo ""
echo "构建完成！输出文件位于 $OUTPUT_DIR 目录"
echo ""
echo "发布文件位置："
[ "$BUILD_ANDROID" = true ] && echo "- Android: $OUTPUT_DIR/android/"
[ "$BUILD_IOS" = true ] && echo "- iOS: $OUTPUT_DIR/ios/"
[ "$BUILD_IOS" = true ] && echo "- macOS: $OUTPUT_DIR/maccatalyst/"
[ "$BUILD_WINDOWS" = true ] && echo "- Windows: $OUTPUT_DIR/windows/"
echo ""
echo "注意：如需构建其他平台，请配置相应的 SDK 和工具链"
