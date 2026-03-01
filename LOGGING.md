# Logging
- Format: JSONL (`LogEntry`)
- Fields: timestamp, level, module, message, device id
- Location: `%LOCALAPPDATA%/Eternity/logs` (or platform equivalent)
- Rotation: max 2MB then rename with timestamp
