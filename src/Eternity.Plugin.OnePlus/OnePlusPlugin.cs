using Eternity.Core.Flashing;
using Eternity.Plugins.Abstractions;

namespace Eternity.Plugin.OnePlus;

/// <summary>Official OnePlus sample adapter with legal-safe partition mapping.</summary>
public sealed class OnePlusPlugin : IFlashingPlugin
{
    /// <inheritdoc />
    public PluginManifest Manifest => new(
        "oneplus.official.sample",
        "OnePlus Adapter (Safe)",
        "OnePlus",
        "1.0.0",
        "Provides vendor-oriented partition ordering without any lock bypass logic.");

    /// <inheritdoc />
    public FlashPlan BuildPlan(IReadOnlyDictionary<string, string> partitionToImagePath)
    {
        var ordered = new[] { "vbmeta", "boot", "vendor", "system" };
        var steps = ordered
            .Where(partitionToImagePath.ContainsKey)
            .Select(p => new FlashStep(p, partitionToImagePath[p]))
            .ToList();
        return new FlashPlan(steps, EnableRollback: true);
    }
}
