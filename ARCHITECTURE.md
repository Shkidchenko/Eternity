# Architecture
- UI: `Eternity.App` (Avalonia Fluent)
- Domain/Core: `Eternity.Core`
- Plugin Contract: `Eternity.Plugins.Abstractions`
- Vendor Plugin: `Eternity.Plugin.OnePlus`
- Tests: unit + integration with mock backend

## Data Flow
UI -> ViewModel -> Core Service (Transport/Parser/Engine) -> Logger/Result/ErrorCode -> UI.

## State Machine
Idle -> PackageParsed -> Confirmed -> Flashing -> Success/Failed(Abort).
