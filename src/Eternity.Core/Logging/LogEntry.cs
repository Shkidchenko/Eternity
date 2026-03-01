namespace Eternity.Core.Logging;

/// <summary>Represents structured log message.</summary>
public sealed record LogEntry(DateTimeOffset Timestamp, string Level, string Module, string Message, string? DeviceId = null);
