@echo off
REM WorkTimeTracker Multi-Platform Build Script for Windows
REM 确保在项目根目录运行此脚本

setlocal

set PROJECT_PATH=WorkTimeTracker.UI\WorkTimeTracker.UI.csproj
set OUTPUT_DIR=publish
set APP_NAME=WorkTimeTracker

echo 开始构建 WorkTimeTracker 多平台版本...

REM 清理输出目录
if exist "%OUTPUT_DIR%" rmdir /s /q "%OUTPUT_DIR%"
mkdir "%OUTPUT_DIR%"

REM 构建 Android (APK)
echo 构建 Android 版本...
dotnet publish "%PROJECT_PATH%" ^
    -f net9.0-android ^
    -c Release ^
    -o "%OUTPUT_DIR%\android" ^
    -p:AndroidSigningKeyStore= ^
    -p:AndroidSigningKeyAlias= ^
    -p:AndroidSigningStorePass= ^
    -p:AndroidSigningKeyPass=

REM 构建 Windows
echo 构建 Windows 版本...
dotnet publish "%PROJECT_PATH%" ^
    -f net9.0-windows10.0.19041.0 ^
    -c Release ^
    -o "%OUTPUT_DIR%\windows" ^
    -p:PublishProfile=FolderProfile ^
    -p:Platform=x64 ^
    -p:RuntimeIdentifier=win-x64 ^
    -p:PublishSingleFile=true ^
    -p:SelfContained=true

echo 构建完成！输出文件位于 %OUTPUT_DIR% 目录
echo.
echo 发布文件位置：
echo - Android: %OUTPUT_DIR%\android\
echo - Windows: %OUTPUT_DIR%\windows\
echo.
echo 注意：iOS 和 macOS 版本需要在 macOS 系统上构建

pause
