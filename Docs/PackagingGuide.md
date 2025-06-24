# WorkTimeTracker 多平台发布指南

## 概述

WorkTimeTracker 支持以下平台的发布：
- **Android** (API 21+)
- **iOS** (iOS 15.0+) 
- **macOS** (macOS Catalyst 15.0+)
- **Windows** (Windows 10 版本 1809+)

## 前置要求

### 通用要求
- .NET 9.0 SDK
- MAUI 工作负载：`dotnet workload install maui`

### Android 发布要求
- Android SDK (API 21+)
- Java 17 JDK
- Android 签名密钥（用于正式发布）

### iOS/macOS 发布要求
- macOS 系统
- Xcode 15+
- Apple 开发者账户（用于发布到 App Store）
- 有效的配置文件和证书

### Windows 发布要求
- Windows 10/11 系统（用于本机编译）
- Visual Studio 2022（可选，用于更好的调试体验）

## 快速发布

### 使用构建脚本

#### 在 macOS/Linux 上：
```bash
chmod +x build-release.sh
./build-release.sh
```

#### 在 Windows 上：
```batch
build-release.bat
```

## 手动平台发布

### 1. Android 发布

#### 开发版本 (Debug APK)
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj \
    -f net9.0-android \
    -c Debug \
    -o publish/android-debug
```

#### 发布版本 (Release APK)
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj \
    -f net9.0-android \
    -c Release \
    -o publish/android-release \
    -p:AndroidSigningKeyStore=your-keystore.keystore \
    -p:AndroidSigningKeyAlias=your-key-alias \
    -p:AndroidSigningStorePass=your-store-password \
    -p:AndroidSigningKeyPass=your-key-password
```

#### 创建签名密钥
```bash
keytool -genkeypair -v -keystore worktimetracker.keystore \
    -alias worktimetracker -keyalg RSA -keysize 2048 -validity 10000 \
    -storepass your-password -keypass your-password
```

### 2. iOS 发布

#### 开发版本
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj \
    -f net9.0-ios \
    -c Debug \
    -o publish/ios-debug
```

#### 发布版本 (App Store)
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj \
    -f net9.0-ios \
    -c Release \
    -o publish/ios-release \
    -p:ArchiveOnBuild=true \
    -p:CodesignProvision="Your Provisioning Profile" \
    -p:CodesignKey="iPhone Distribution: Your Name"
```

### 3. macOS 发布 (Mac Catalyst)

#### 开发版本
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj \
    -f net9.0-maccatalyst \
    -c Debug \
    -o publish/maccatalyst-debug
```

#### 发布版本
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj \
    -f net9.0-maccatalyst \
    -c Release \
    -o publish/maccatalyst-release \
    -p:CreatePackage=true \
    -p:RuntimeIdentifiers=maccatalyst-x64,maccatalyst-arm64
```

### 4. Windows 发布

#### 开发版本
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj \
    -f net9.0-windows10.0.19041.0 \
    -c Debug \
    -o publish/windows-debug
```

#### 发布版本 (独立部署)
```bash
dotnet publish WorkTimeTracker.UI/WorkTimeTracker.UI.csproj \
    -f net9.0-windows10.0.19041.0 \
    -c Release \
    -o publish/windows-release \
    -p:RuntimeIdentifier=win-x64 \
    -p:PublishSingleFile=true \
    -p:SelfContained=true
```

## 应用商店发布

### Google Play Store (Android)
1. 使用发布签名创建 AAB 文件：
```bash
dotnet publish -c Release -f net9.0-android \
    -p:AndroidPackageFormat=aab \
    -p:AndroidSigningKeyStore=release.keystore
```
2. 上传到 Google Play Console
3. 填写应用信息和截图
4. 提交审核

### Apple App Store (iOS)
1. 确保有有效的开发者账户和证书
2. 使用 Xcode 或命令行上传到 App Store Connect
3. 在 App Store Connect 中配置应用信息
4. 提交审核

### Microsoft Store (Windows)
1. 注册 Microsoft 开发者账户
2. 创建 MSIX 包：
```bash
dotnet publish -c Release -f net9.0-windows10.0.19041.0 \
    -p:WindowsPackageType=MSIX
```
3. 上传到 Microsoft Partner Center

## 测试建议

### Android
- 在不同 Android 版本上测试 (API 21-34)
- 测试不同屏幕尺寸和密度
- 验证权限请求正常工作

### iOS
- 在 iPhone 和 iPad 上测试
- 测试不同 iOS 版本 (15.0+)
- 验证 TTS 和通知功能

### Windows
- 测试不同 Windows 版本 (1809+)
- 验证在高 DPI 屏幕上的显示
- 测试触摸和鼠标输入

## 故障排除

### 常见问题

1. **Android 签名错误**
   - 确保密钥库路径正确
   - 验证密码和别名匹配

2. **iOS 配置文件问题**
   - 检查开发者账户状态
   - 确保设备已注册

3. **Windows 依赖问题**
   - 使用 SelfContained=true 包含所有依赖
   - 检查目标 Windows 版本兼容性

### 性能优化

- 启用 AOT 编译以提高启动速度
- 使用 R8/ProGuard 压缩 Android APK
- 优化图片资源大小

## 版本管理

在 `WorkTimeTracker.UI.csproj` 中更新版本号：
```xml
<ApplicationDisplayVersion>1.0.1</ApplicationDisplayVersion>
<ApplicationVersion>1</ApplicationVersion>
```

## 注意事项

1. **安全性**
   - 不要在代码中硬编码签名密码
   - 使用环境变量或安全存储
   - 定期更新依赖包

2. **合规性**
   - 遵守各平台的应用商店政策
   - 添加必要的隐私政策
   - 请求最小必要权限

3. **维护**
   - 定期更新 MAUI 和依赖包
   - 监控崩溃报告和用户反馈
   - 保持不同平台版本同步isual Studio 打包
- 打开项目。
- 切换到 Release 模式。
- 在菜单中选择“生成” -> “发布”，按照向导生成适用于目标平台的安装包。

## 2. 使用命令行打包
在项目根目录下执行以下命令（以 Mac Catalyst 为例）：
```bash
dotnet publish -c Release -f net9.0-maccatalyst -o ../publish
```
生成的应用将位于 `../publish` 文件夹内。

## 3. 安装包制作
如需要将发布产物制作成 DMG 或 PKG 安装包，可以使用相应的第三方工具（例如 create-dmg、Packages）。

## 4. 注意事项
- 确保发布前解决所有警告和错误。
- 检查应用的依赖项是否完整。
