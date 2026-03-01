using Eternity.Core.Flashing;

namespace Eternity.Plugins.Abstractions;

/// <summary>Plugin metadata.</summary>
public sealed record PluginManifest(string Id, string Name, string Vendor, string Version, string Description);

/// <summary>Extensibility interface for vendor workflows.</summary>
public interface IFlashingPlugin
{
    /// <summary>Gets plugin manifest.</summary>
    PluginManifest Manifest { get; }

    /// <summary>Builds vendor-specific flash plan.</summary>
    FlashPlan BuildPlan(IReadOnlyDictionary<string, string> partitionToImagePath);
}
