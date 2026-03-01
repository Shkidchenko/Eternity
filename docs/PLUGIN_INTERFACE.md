# Plugin Interface
实现 `IFlashingPlugin`：
- `Manifest` 提供插件标识与描述。
- `BuildPlan` 根据映射生成 `FlashPlan`。

示例：`src/Eternity.Plugin.OnePlus/OnePlusPlugin.cs`。

安全要求：禁止实现任何未授权解锁、锁绕过、漏洞利用逻辑。
