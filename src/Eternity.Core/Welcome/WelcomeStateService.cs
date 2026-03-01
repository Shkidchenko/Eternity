using System.Text.Json;

namespace Eternity.Core.Welcome;

/// <summary>Manages first-launch welcome state persistence.</summary>
public sealed class WelcomeStateService
{
    private readonly string _path;

    /// <summary>Initializes service with explicit path.</summary>
    public WelcomeStateService(string path)
    {
        _path = path;
    }

    /// <summary>Gets whether welcome screen should be shown.</summary>
    public bool ShouldShowWelcome()
    {
        if (!File.Exists(_path)) return true;
        var state = JsonSerializer.Deserialize<WelcomeState>(File.ReadAllText(_path));
        return state?.Accepted != true;
    }

    /// <summary>Marks welcome accepted.</summary>
    public void AcceptWelcome()
    {
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(_path, JsonSerializer.Serialize(new WelcomeState(true)));
    }

    private sealed record WelcomeState(bool Accepted);
}
