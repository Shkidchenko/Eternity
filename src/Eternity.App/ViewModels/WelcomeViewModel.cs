using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Eternity.App.Views;
using Eternity.Core.Welcome;

namespace Eternity.App.ViewModels;

/// <summary>Welcome view model enforcing one-time disclaimer acceptance.</summary>
public partial class WelcomeViewModel : ObservableObject
{
    private readonly WelcomeStateService _service;

    /// <summary>Initializes a new instance.</summary>
    public WelcomeViewModel(WelcomeStateService service)
    {
        _service = service;
    }

    /// <summary>Safety statement text.</summary>
    public string SafetyText => "软件仅用于合法设备维护；使用者自担风险；开发者不承担设备损坏责任。";

    /// <summary>Accepts disclaimer and navigates to main window.</summary>
    [RelayCommand]
    public void Start(Window window)
    {
        _service.AcceptWelcome();
        var main = new MainWindow { DataContext = new MainViewModel() };
        main.Show();
        window.Close();
    }
}
