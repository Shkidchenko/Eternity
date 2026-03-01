# Acceptance Criteria

1. CI 在 ubuntu-latest 构建、测试、发布：运行 `.github/workflows/release.yml`。
2. Windows/Linux 可运行主窗口：`dotnet run --project src/Eternity.App`。
3. 模拟模式可无设备演示：主界面点击刷新设备。
4. SHA256 通过显示校验信息，失败阻止刷机：`PackageParser` + 单元测试验证。
5. 首次欢迎页仅出现一次：`WelcomeStateService` 单元测试。
6. 插件系统可加载并执行 OnePlus 流程：`OnePlusPlugin` 单元测试与集成流程。
7. 文档与规范文件齐全：README / ARCHITECTURE / MODULES / ERROR_CODES / LOGGING / TEST_PLAN。
