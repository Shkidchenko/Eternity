using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Eternity.App.ViewModels;
using Eternity.App.Views;
using Eternity.Core.Welcome;

namespace Eternity.App;

/// <summary>Main app type.</summary>
public partial class App : Application
{
    /// <inheritdoc />
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var statePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eternity", "welcome.json");
            var service = new WelcomeStateService(statePath);
            desktop.MainWindow = service.ShouldShowWelcome()
                ? new WelcomeWindow { DataContext = new WelcomeViewModel(service) }
                : new MainWindow { DataContext = new MainViewModel() };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
