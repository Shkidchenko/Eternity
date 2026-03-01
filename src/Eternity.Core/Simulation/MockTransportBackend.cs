using Eternity.Core.Abstractions;
using Eternity.Core.Errors;
using Eternity.Core.Models;

namespace Eternity.Core.Simulation;

/// <summary>Mock backend for CI/demo without physical devices.</summary>
public sealed class MockTransportBackend : ITransportBackend
{
    private readonly Dictionary<string, string> _responses = new();

    /// <summary>Initializes mock backend with default data.</summary>
    public MockTransportBackend()
    {
        Devices =
        [
            new DeviceInfo("MOCK123", "Pixel Mock", DeviceMode.Fastboot, "online"),
            new DeviceInfo("MOCK456", "OnePlus Mock", DeviceMode.Adb, "device")
        ];
    }

    /// <summary>Gets or sets simulated devices.</summary>
    public IReadOnlyList<DeviceInfo> Devices { get; set; }

    /// <summary>Registers deterministic command output.</summary>
    public void SetResponse(string key, string output) => _responses[key] = output;

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<DeviceInfo>>> ListDevicesAsync(CancellationToken cancellationToken)
        => Task.FromResult(Result<IReadOnlyList<DeviceInfo>>.Success(Devices));

    /// <inheritdoc />
    public Task<Result<string>> ExecuteCommandAsync(string tool, string arguments, string? deviceId, TimeSpan timeout, int retries, CancellationToken cancellationToken)
    {
        if (timeout <= TimeSpan.Zero)
        {
            return Task.FromResult(Result<string>.Fail(new OperationError(ErrorCode.ValidationFailed, "Timeout must be > 0", "transport", deviceId)));
        }

        if (arguments.Contains("fail", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(Result<string>.Fail(new OperationError(ErrorCode.ToolExecutionFailed, "Simulated failure", tool, deviceId)));
        }

        var key = $"{tool} {arguments}";
        var value = _responses.TryGetValue(key, out var outText) ? outText : "OK";
        return Task.FromResult(Result<string>.Success(value));
    }
}
