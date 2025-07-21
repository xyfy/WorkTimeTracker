# GitHub Actions 工作流已禁用说明

本项目所有 `.github/workflows/` 下的 GitHub Actions 工作流已被禁用（文件已重命名为 `.disabled` 后缀），原因如下：

- 你当前为 GitHub 普通账号（非企业/团队付费账号），不希望消耗 Actions 免费额度或产生额外费用。
- Copilot Pro 订阅不影响 Actions 的计费，Actions 依然会消耗 GitHub 免费额度。
- 项目本地构建、测试、发布脚本（如 build-release.sh、build-release.bat）均可独立运行，无需依赖云端 CI/CD。

## 如需重新启用

将 `.github/workflows/` 目录下的 `.yml.disabled` 文件重命名回 `.yml` 即可。

---

> **提示**：如需本地自动化或持续集成，请使用本地脚本或第三方 CI 工具（如 Jenkins、GitLab CI 等）。
