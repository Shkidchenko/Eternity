using Eternity.Core.Errors;
using Eternity.Core.Models;

namespace Eternity.Core.Abstractions;

/// <summary>Provides ADB/Fastboot transport operations.</summary>
public interface ITransportBackend
{
    /// <summary>Enumerates connected devices.</summary>
    Task<Result<IReadOnlyList<DeviceInfo>>> ListDevicesAsync(CancellationToken cancellationToken);

    /// <summary>Executes command with timeout and retry policy.</summary>
    Task<Result<string>> ExecuteCommandAsync(string tool, string arguments, string? deviceId, TimeSpan timeout, int retries, CancellationToken cancellationToken);
}
