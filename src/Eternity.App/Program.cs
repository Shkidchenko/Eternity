using Avalonia;

namespace Eternity.App;

/// <summary>Application entrypoint.</summary>
internal static class Program
{
    /// <summary>Main method.</summary>
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    /// <summary>Builds app builder.</summary>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
}
