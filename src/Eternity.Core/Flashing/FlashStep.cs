namespace Eternity.Core.Flashing;

/// <summary>Defines a single flashing action.</summary>
public sealed record FlashStep(string Partition, string ImagePath, bool Optional = false);

/// <summary>Result status of a flash step.</summary>
public enum FlashStepStatus { Pending, Success, Failed, Skipped }

/// <summary>Result of one step execution.</summary>
public sealed record FlashStepResult(FlashStep Step, FlashStepStatus Status, string Output, TimeSpan Duration);
