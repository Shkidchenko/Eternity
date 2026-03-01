namespace Eternity.Core.Errors;

/// <summary>Stable and serializable application error codes.</summary>
public enum ErrorCode
{
    None = 0,
    ValidationFailed = 1000,
    Timeout = 1001,
    DeviceNotFound = 1002,
    ToolExecutionFailed = 1003,
    UnsupportedPackage = 2000,
    IntegrityCheckFailed = 2001,
    FlashStepFailed = 3000,
    PluginLoadFailed = 4000,
    UnauthorizedOperation = 4001
}

/// <summary>Serializable error descriptor.</summary>
public sealed record OperationError(ErrorCode Code, string Message, string? Module = null, string? DeviceId = null);

/// <summary>Wraps operation result with value or error.</summary>
public sealed record Result<T>(T? Value, OperationError? Error)
{
    /// <summary>Gets whether operation succeeded.</summary>
    public bool IsSuccess => Error is null;

    /// <summary>Creates success result.</summary>
    public static Result<T> Success(T value) => new(value, null);

    /// <summary>Creates failure result.</summary>
    public static Result<T> Fail(OperationError error) => new(default, error);
}
