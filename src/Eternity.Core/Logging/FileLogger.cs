using System.Text.Json;

namespace Eternity.Core.Logging;

/// <summary>Persists logs to JSONL with simple rolling policy.</summary>
public sealed class FileLogger : ILogger
{
    private readonly string _directory;
    private readonly long _maxBytes;

    /// <summary>Initializes a new file logger.</summary>
    public FileLogger(string directory, long maxBytes = 2_000_000)
    {
        _directory = directory;
        _maxBytes = maxBytes;
        Directory.CreateDirectory(_directory);
    }

    /// <inheritdoc />
    public void Log(LogEntry entry)
    {
        var file = Path.Combine(_directory, "eternity.log");
        if (File.Exists(file) && new FileInfo(file).Length > _maxBytes)
        {
            File.Move(file, Path.Combine(_directory, $"eternity-{DateTime.UtcNow:yyyyMMddHHmmss}.log"), true);
        }

        File.AppendAllText(file, JsonSerializer.Serialize(entry) + Environment.NewLine);
    }
}
