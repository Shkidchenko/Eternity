namespace Eternity.Core.Logging;

/// <summary>Writes structured logs.</summary>
public interface ILogger
{
    /// <summary>Writes a log entry.</summary>
    void Log(LogEntry entry);
}
