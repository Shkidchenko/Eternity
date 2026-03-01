# Eternity

Eternity 是一个基于 **.NET 8 + Avalonia** 的跨平台（Windows/Linux）可视化安卓刷机工具，采用 Fluent Design 风格，并提供 ADB/Fastboot 封装、刷机包解析、一键刷机、插件扩展与模拟模式。

## Quick Start
1. `dotnet restore Eternity.sln`
2. `dotnet build Eternity.sln -c Release`
3. `dotnet test Eternity.sln -c Release`
4. `dotnet run --project src/Eternity.App/Eternity.App.csproj`

## Features
- ADB/Fastboot 异步封装（重试、超时、错误码、日志）
- ZIP/TAR 刷机包解析与 SHA256 校验
- 事务化顺序刷机引擎（失败即中止，可选回滚）
- 插件系统与 OnePlus 示例插件（合法流程示例）
- 首次启动欢迎页（一次性安全声明确认）
- 模拟模式支持无设备演示与 CI 测试

## Disclaimer / 免责声明
- 软件仅用于合法设备维护。
- 使用者需自行承担风险。
- 开发者不承担设备损坏、数据丢失或法律责任。

## CI / Release
GitHub Actions 工作流位于 `.github/workflows/release.yml`，会在 ubuntu-latest 上执行 restore/build/test/publish，并在 tag 或手动触发时上传 release artifacts。

## Author
- 作者：阿蕾克希娅（酷安同名）
- GitHub: https://github.com/Shkidchenko/

## English Summary
Eternity is a cross-platform Android flashing desktop tool built with .NET 8 and Avalonia Fluent UI. It includes async ADB/Fastboot wrappers, package parsing (zip/tar), SHA256 integrity checks, transactional flashing with stop-on-failure behavior, plugin loading with a legal-safe OnePlus adapter example, first-launch mandatory disclaimer flow, and GitHub Actions release automation for Windows/Linux portable artifacts.

## Third-party Dependencies and Licenses
- Avalonia (MIT)
- CommunityToolkit.Mvvm (MIT)
- SharpCompress (MIT)
- xUnit (Apache-2.0)
