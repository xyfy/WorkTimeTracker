# 多平台发布配置文档

## 项目平台支持

WorkTimeTracker 现已配置为支持以下平台的发布：

- Android (API 21+)
- iOS (15.0+)
- macOS Catalyst (15.0+)
- Windows (10.0.19041.0+)

## 快速发布命令

### Android
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj -f net9.0-android -c Release
```

### iOS (需要 macOS)
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj -f net9.0-ios -c Release
```

### macOS Catalyst
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj -f net9.0-maccatalyst -c Release
```

### Windows
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj -f net9.0-windows10.0.19041.0 -c Release
```

## 使用构建脚本

项目根目录包含自动化构建脚本：

- `build-release.sh` (macOS/Linux)
- `build-release.bat` (Windows)

运行相应的脚本即可自动构建所有支持的平台版本。

## 重要注意事项

1. Android 发布需要签名配置
2. iOS 发布需要有效的开发者证书和配置文件
3. Windows 发布可以创建独立部署包
4. 所有平台都支持应用商店发布

详细发布步骤请参考 PackagingGuide.md 文件。
