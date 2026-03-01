# User Manual (Draft)
## 安装
下载 Release 中的 Windows/Linux 便携包，解压后运行 `Eternity.App`。

## 典型工作流
1. 启动后首次阅读并确认欢迎声明。
2. 刷新设备列表。
3. 导入 zip/tar 刷机包并检查 SHA256 与分区映射建议。
4. 双确认后启动刷机。
5. 导出日志用于审计与故障排查。

## Troubleshooting
- 未识别设备：检查 adb/fastboot 与驱动。
- 校验失败：重新下载官方包。
- 插件加载失败：检查插件 DLL 与 manifest。
