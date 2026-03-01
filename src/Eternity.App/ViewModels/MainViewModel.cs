using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Eternity.Core.Flashing;
using Eternity.Core.Logging;
using Eternity.Core.Packages;
using Eternity.Core.Simulation;

namespace Eternity.App.ViewModels;

/// <summary>Main shell view model.</summary>
public partial class MainViewModel : ObservableObject
{
    private readonly MockTransportBackend _backend = new();
    private readonly FileLogger _logger = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Eternity", "logs"));
    private readonly PackageParser _parser = new();

    /// <summary>Initializes main view model.</summary>
    public MainViewModel()
    {
        Logs.Add("模拟模式已启用（无真实设备）。");
        Author = "作者：阿蕾克希娅（酷安同名）";
        AuthorLink = "https://github.com/Shkidchenko/";
    }

    /// <summary>Connected devices.</summary>
    public ObservableCollection<string> Devices { get; } = new();

    /// <summary>Package images.</summary>
    public ObservableCollection<string> Images { get; } = new();

    /// <summary>Runtime logs.</summary>
    public ObservableCollection<string> Logs { get; } = new();

    /// <summary>Gets author text.</summary>
    public string Author { get; }

    /// <summary>Gets author link.</summary>
    public string AuthorLink { get; }

    /// <summary>Gets or sets package path.</summary>
    [ObservableProperty]
    private string packagePath = string.Empty;

    /// <summary>Gets or sets dark mode.</summary>
    [ObservableProperty]
    private bool isDarkTheme;

    /// <summary>Loads mock devices.</summary>
    [RelayCommand]
    public async Task RefreshDevicesAsync()
    {
        Devices.Clear();
        var result = await _backend.ListDevicesAsync(CancellationToken.None);
        foreach (var device in result.Value ?? Array.Empty<Eternity.Core.Models.DeviceInfo>())
        {
            Devices.Add($"{device.Serial} [{device.Mode}] {device.State}");
        }
    }

    /// <summary>Parses package path.</summary>
    [RelayCommand]
    public async Task ParsePackageAsync()
    {
        Images.Clear();
        var result = await _parser.ParseAsync(PackagePath, CancellationToken.None);
        if (!result.IsSuccess)
        {
            Logs.Add($"解析失败: {result.Error!.Message}");
            return;
        }

        foreach (var image in result.Value!.Images)
        {
            Images.Add($"{image.Path} -> {image.SuggestedPartition} SHA256:{image.Sha256[..8]}...");
        }

        foreach (var warning in result.Value.Warnings)
        {
            Logs.Add($"警告: {warning}");
        }
    }

    /// <summary>Runs a one-click flash simulation with mandatory confirmation points handled by UI.</summary>
    [RelayCommand]
    public async Task StartFlashAsync()
    {
        var engine = new FlashEngine(_backend, _logger);
        var plan = new FlashPlan(
        [
            new FlashStep("vbmeta", "vbmeta.img"),
            new FlashStep("boot", "boot.img")
        ], true);

        var result = await engine.RunAsync("MOCK123", plan, CancellationToken.None);
        Logs.Add(result.IsSuccess ? "刷机流程完成" : $"刷机中止: {result.Error!.Message}");
    }
}
