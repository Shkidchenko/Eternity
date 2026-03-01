using System.Diagnostics;
using Eternity.Core.Abstractions;
using Eternity.Core.Errors;
using Eternity.Core.Models;

namespace Eternity.Core.Services;

/// <summary>Runs adb/fastboot CLI commands with retries and timeout control.</summary>
public sealed class ToolTransportBackend : ITransportBackend
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<DeviceInfo>>> ListDevicesAsync(CancellationToken cancellationToken)
    {
        var adb = await ExecuteCommandAsync("adb", "devices", null, TimeSpan.FromSeconds(10), 1, cancellationToken).ConfigureAwait(false);
        var fastboot = await ExecuteCommandAsync("fastboot", "devices", null, TimeSpan.FromSeconds(10), 1, cancellationToken).ConfigureAwait(false);
        if (!adb.IsSuccess && !fastboot.IsSuccess)
        {
            return Result<IReadOnlyList<DeviceInfo>>.Fail(adb.Error!);
        }

        var list = new List<DeviceInfo>();
        if (adb.IsSuccess)
        {
            list.AddRange(ParseDeviceList(adb.Value!, DeviceMode.Adb));
        }

        if (fastboot.IsSuccess)
        {
            list.AddRange(ParseDeviceList(fastboot.Value!, DeviceMode.Fastboot));
        }

        return Result<IReadOnlyList<DeviceInfo>>.Success(list);
    }

    /// <inheritdoc />
    public async Task<Result<string>> ExecuteCommandAsync(string tool, string arguments, string? deviceId, TimeSpan timeout, int retries, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tool))
        {
            return Result<string>.Fail(new OperationError(ErrorCode.ValidationFailed, "Tool is required", "transport", deviceId));
        }

        for (var attempt = 0; attempt <= retries; attempt++)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);
            Process? process = null;
            try
            {
                var scopedArguments = BuildScopedArguments(tool, arguments, deviceId);
                var psi = new ProcessStartInfo(tool, scopedArguments)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process = Process.Start(psi);
                if (process is null)
                {
                    return Result<string>.Fail(new OperationError(ErrorCode.ToolExecutionFailed, $"Cannot start {tool}", "transport", deviceId));
                }

                await process.WaitForExitAsync(cts.Token).ConfigureAwait(false);
                var output = await process.StandardOutput.ReadToEndAsync(cts.Token).ConfigureAwait(false);
                var error = await process.StandardError.ReadToEndAsync(cts.Token).ConfigureAwait(false);

                if (process.ExitCode == 0)
                {
                    return Result<string>.Success(output.Trim());
                }

                if (attempt == retries)
                {
                    return Result<string>.Fail(new OperationError(ErrorCode.ToolExecutionFailed, error, "transport", deviceId));
                }
            }
            catch (OperationCanceledException)
            {
                TryTerminateProcess(process);

                if (attempt == retries)
                {
                    return Result<string>.Fail(new OperationError(ErrorCode.Timeout, $"{tool} timed out", "transport", deviceId));
                }
            }
            catch (Exception ex)
            {
                TryTerminateProcess(process);

                if (attempt == retries)
                {
                    return Result<string>.Fail(new OperationError(ErrorCode.ToolExecutionFailed, ex.Message, "transport", deviceId));
                }
            }
            finally
            {
                process?.Dispose();
            }
        }

        return Result<string>.Fail(new OperationError(ErrorCode.ToolExecutionFailed, "Unknown failure", "transport", deviceId));
    }

    private static string BuildScopedArguments(string tool, string arguments, string? deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId) || !IsDeviceScopedTool(tool))
        {
            return arguments;
        }

        var escapedDeviceId = deviceId.Replace("\"", "\\\"");
        return $"-s \"{escapedDeviceId}\" {arguments}".Trim();
    }

    private static bool IsDeviceScopedTool(string tool)
        => string.Equals(tool, "adb", StringComparison.OrdinalIgnoreCase)
           || string.Equals(tool, "fastboot", StringComparison.OrdinalIgnoreCase);

    private static void TryTerminateProcess(Process? process)
    {
        if (process is null || process.HasExited)
        {
            return;
        }

        try
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
        }
        catch
        {
            // Swallow process termination failures and allow retry/error handling to continue.
        }
    }

    private static IEnumerable<DeviceInfo> ParseDeviceList(string data, DeviceMode mode)
    {
        foreach (var line in data.Split('\n', StringSplitOptions.RemoveEmptyEntries).Skip(1))
        {
            var parts = line.Split('\t', ' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                yield return new DeviceInfo(parts[0], parts[0], mode, parts.Length > 1 ? parts[1] : "unknown");
            }
        }
    }
}
