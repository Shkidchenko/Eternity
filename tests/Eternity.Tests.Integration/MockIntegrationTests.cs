using Eternity.Core.Flashing;
using Eternity.Core.Logging;
using Eternity.Core.Simulation;

namespace Eternity.Tests.Integration;

/// <summary>Integration tests with mock transport.</summary>
public sealed class MockIntegrationTests
{
    [Fact]
    public async Task EndToEnd_MockFlash_Succeeds()
    {
        var backend = new MockTransportBackend();
        backend.SetResponse("fastboot flash vbmeta \"vbmeta.img\"", "OK");
        backend.SetResponse("fastboot flash boot \"boot.img\"", "OK");
        var logger = new FileLogger(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        var engine = new FlashEngine(backend, logger);
        var result = await engine.RunAsync("MOCK123", new FlashPlan([new FlashStep("vbmeta", "vbmeta.img"), new FlashStep("boot", "boot.img")], true), CancellationToken.None);
        Assert.True(result.IsSuccess);
    }
}
