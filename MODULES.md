# Modules and Public APIs
- `ITransportBackend`: 设备枚举与命令执行（异步、超时、重试）
- `PackageParser.ParseAsync`: 解析 zip/tar，输出镜像清单与 SHA256
- `FlashEngine.RunAsync`: 顺序刷机事务执行
- `PluginLoader.LoadPlugins`: 动态加载插件
- `WelcomeStateService`: 首次欢迎页状态持久化
