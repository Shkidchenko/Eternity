using Eternity.Core.Abstractions;
using Eternity.Core.Errors;
using Eternity.Core.Logging;

namespace Eternity.Core.Flashing;

/// <summary>Executes transactional flash plan and halts on first failure.</summary>
public sealed class FlashEngine
{
    private readonly ITransportBackend _backend;
    private readonly ILogger _logger;

    /// <summary>Initializes engine.</summary>
    public FlashEngine(ITransportBackend backend, ILogger logger)
    {
        _backend = backend;
        _logger = logger;
    }

    /// <summary>Runs all flash steps sequentially.</summary>
    public async Task<Result<IReadOnlyList<FlashStepResult>>> RunAsync(string deviceId, FlashPlan plan, CancellationToken cancellationToken)
    {
        var results = new List<FlashStepResult>();
        foreach (var step in plan.Steps)
        {
            var start = DateTimeOffset.UtcNow;
            _logger.Log(new LogEntry(start, "INFO", "flash", $"Flashing {step.Partition} from {step.ImagePath}", deviceId));
            var cmd = $"flash {step.Partition} \"{step.ImagePath}\"";
            var resp = await _backend.ExecuteCommandAsync("fastboot", cmd, deviceId, TimeSpan.FromMinutes(2), 1, cancellationToken).ConfigureAwait(false);
            var elapsed = DateTimeOffset.UtcNow - start;
            if (!resp.IsSuccess)
            {
                results.Add(new FlashStepResult(step, FlashStepStatus.Failed, resp.Error!.Message, elapsed));
                _logger.Log(new LogEntry(DateTimeOffset.UtcNow, "ERROR", "flash", resp.Error.Message, deviceId));
                if (plan.EnableRollback)
                {
                    await _backend.ExecuteCommandAsync("fastboot", "reboot bootloader", deviceId, TimeSpan.FromSeconds(30), 0, cancellationToken).ConfigureAwait(false);
                }

                return Result<IReadOnlyList<FlashStepResult>>.Fail(new OperationError(ErrorCode.FlashStepFailed, $"Step failed: {step.Partition}", "flash", deviceId));
            }

            results.Add(new FlashStepResult(step, FlashStepStatus.Success, resp.Value!, elapsed));
        }

        return Result<IReadOnlyList<FlashStepResult>>.Success(results);
    }
}
