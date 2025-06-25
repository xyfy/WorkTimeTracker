# WorkTimeTracker GitHub Actions 配置

## 🔧 工作流说明

本项目包含以下 GitHub Actions 工作流：

### 1. CI/CD Pipeline (`ci-cd.yml`)
**触发条件**: 推送到 main/dev 分支、Pull Request、发布 Release
**功能**:
- 🔨 自动编译程序（所有平台）
- 🧪 自动测试程序（单元测试 + UI测试）
- 🌐 跨平台构建（Windows、macOS）
- 🚀 自动发布程序（Release时）
- 📦 创建发布包和NuGet包

### 2. PR Validation (`pr-validation.yml`)
**触发条件**: Pull Request 创建或更新
**功能**:
- ✅ 快速验证构建和测试
- 📋 代码分析和格式检查
- 📈 变更影响分析
- 💬 自动评论PR结果

### 3. Release Build (`release.yml`)
**触发条件**: 发布 Release 或手动触发
**功能**:
- 🪟 构建 Windows 应用程序
- 🍎 构建 macOS 应用程序
- 🐧 构建 Linux 库文件
- 📦 创建安装包
- 🚀 发布到 GitHub Releases

### 4. Maintenance (`maintenance.yml`)
**触发条件**: 每周一定时运行或手动触发
**功能**:
- 🔐 安全漏洞扫描
- 📦 依赖项更新检查
- 📊 代码质量报告
- 🧹 清理过期工件

### 5. Code Quality (`code-quality.yml`)
**触发条件**: 推送代码、PR或每日定时运行
**功能**:
- 📝 代码格式检查
- 🔍 静态分析
- 📊 代码复杂度分析
- 🔐 安全扫描
- 📈 测试覆盖率检查
- 🚪 质量门验证

## 🛠️ 配置要求

### 必需的 Secrets
- `GITHUB_TOKEN`: 自动提供，用于访问仓库

### 可选的 Secrets
- `NUGET_API_KEY`: 用于发布NuGet包到官方仓库

### 环境变量
- `DOTNET_VERSION`: .NET 版本 (默认: 9.0.x)
- `SOLUTION_FILE`: 解决方案文件 (默认: WorkTimeTracker.sln)

## 📋 分支策略

### main 分支
- 🔒 受保护分支
- ✅ 需要PR review
- 🧪 需要通过所有检查
- 🚀 自动触发发布流程

### dev/develop 分支
- 🔄 开发分支
- ✅ 运行完整CI流程
- 📊 生成质量报告

### feature/* 分支
- 🔍 基础验证检查
- 🧪 运行测试
- 📝 代码质量检查

## 🎯 质量门标准

代码合并到 main 分支需要满足以下条件：

### 🔨 构建要求
- ✅ 所有项目编译成功
- ✅ 无编译错误
- ⚠️ 警告数量 < 50

### 🧪 测试要求
- ✅ 所有单元测试通过
- ✅ 测试成功率 ≥ 80%
- ✅ UI测试基本功能验证

### 📝 代码质量
- ✅ 代码格式符合标准
- ✅ 静态分析无严重问题
- ✅ 无安全漏洞

### 🔐 安全要求
- ✅ 依赖项无已知漏洞
- ✅ 无硬编码密钥
- ✅ 安全扫描通过

## 🚀 发布流程

### 自动发布
1. 创建 Release tag (v1.0.0)
2. 自动触发构建流程
3. 生成多平台安装包
4. 发布到 GitHub Releases

### 手动发布
1. 运行 Release Build 工作流
2. 指定版本号
3. 选择是否创建Release

## 📊 监控和报告

### 工作流状态
- 📈 所有工作流状态在 Actions 页面可见
- 📊 质量报告在工作流摘要中显示
- 📧 失败时自动创建Issue

### 定期报告
- 📋 每周质量报告
- 🔐 安全扫描报告
- 📦 依赖项更新建议

## 🔧 本地开发建议

### 提交前检查
```bash
# 代码格式化
dotnet format

# 运行测试
dotnet test

# 构建检查
dotnet build --configuration Release
```

### 工作流测试
```bash
# 安装 act (本地运行 GitHub Actions)
# macOS: brew install act
# 测试 PR 验证工作流
act pull_request -W .github/workflows/pr-validation.yml
```

## 📚 参考资料

- [GitHub Actions 文档](https://docs.github.com/en/actions)
- [.NET GitHub Actions](https://github.com/actions/setup-dotnet)
- [MAUI 构建指南](https://docs.microsoft.com/en-us/dotnet/maui/)

## 🤝 贡献指南

1. Fork 仓库
2. 创建功能分支
3. 确保通过所有检查
4. 提交 Pull Request
5. 等待 Review 和合并
