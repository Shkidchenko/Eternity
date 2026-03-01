using Eternity.Core.Errors;
using Eternity.Core.Flashing;
using Eternity.Core.Logging;
using Eternity.Core.Packages;
using Eternity.Core.Plugins;
using Eternity.Core.Simulation;
using Eternity.Core.Welcome;
using Eternity.Plugin.OnePlus;

namespace Eternity.Tests.Unit;

/// <summary>Unit tests for core logic.</summary>
public sealed class CoreTests
{
    [Fact] public void SuggestPartition_Boot() => Assert.Equal("boot", PackageParser.SuggestPartition("boot.img"));
    [Fact] public void SuggestPartition_Vbmeta() => Assert.Equal("vbmeta", PackageParser.SuggestPartition("vbmeta.img"));
    [Fact] public void SuggestPartition_Unknown() => Assert.Equal("unknown", PackageParser.SuggestPartition("foo.bin"));
    [Fact] public void Welcome_Default_Show() => Assert.True(new WelcomeStateService(Path.GetTempFileName() + ".missing").ShouldShowWelcome());

    [Fact]
    public void Welcome_Accept_Hides()
    {
        var path = Path.GetTempFileName();
        var svc = new WelcomeStateService(path);
        svc.AcceptWelcome();
        Assert.False(svc.ShouldShowWelcome());
    }

    [Fact]
    public async Task MockBackend_Rejects_InvalidTimeout()
    {
        var backend = new MockTransportBackend();
        var result = await backend.ExecuteCommandAsync("adb", "devices", null, TimeSpan.Zero, 0, CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ValidationFailed, result.Error!.Code);
    }

    [Fact]
    public async Task FlashEngine_Stops_OnFailure()
    {
        var backend = new MockTransportBackend();
        var logger = new FileLogger(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        var engine = new FlashEngine(backend, logger);
        var plan = new FlashPlan([new FlashStep("boot", "fail.img")], true);
        var result = await engine.RunAsync("MOCK", plan, CancellationToken.None);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Plugin_Manifest_Valid()
    {
        var plugin = new OnePlusPlugin();
        Assert.Equal("OnePlus", plugin.Manifest.Vendor);
    }

    [Fact]
    public void Plugin_BuildPlan_OrdersSteps()
    {
        var plugin = new OnePlusPlugin();
        var plan = plugin.BuildPlan(new Dictionary<string, string>{{"system","s.img"},{"boot","b.img"}});
        Assert.Equal("boot", plan.Steps[0].Partition);
    }

    [Fact]
    public void PluginLoader_EmptyFolder_ReturnsEmpty()
    {
        var result = new PluginLoader().LoadPlugins(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public void Result_Fail_HasError()
    {
        var value = Result<string>.Fail(new OperationError(ErrorCode.Timeout, "timeout"));
        Assert.False(value.IsSuccess);
    }
}
