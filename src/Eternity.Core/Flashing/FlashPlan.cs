namespace Eternity.Core.Flashing;

/// <summary>Transaction-like flash plan sequence.</summary>
public sealed record FlashPlan(IReadOnlyList<FlashStep> Steps, bool EnableRollback);
