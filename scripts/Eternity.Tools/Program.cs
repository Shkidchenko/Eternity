using System.Security.Cryptography;

namespace Eternity.Tools;

/// <summary>CI helper utilities: version generation and SHA256 verification.</summary>
public static class Program
{
    /// <summary>Entrypoint.</summary>
    public static int Main(string[] args)
    {
        if (args.Length == 0) return 1;
        return args[0] switch
        {
            "version" => WriteVersion(args),
            "sha256" => VerifySha(args),
            _ => 1
        };
    }

    private static int WriteVersion(string[] args)
    {
        var suffix = args.Length > 1 ? args[1] : DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        Console.WriteLine($"v1.0.0-{suffix}");
        return 0;
    }

    private static int VerifySha(string[] args)
    {
        if (args.Length < 3 || !File.Exists(args[1])) return 1;
        using var stream = File.OpenRead(args[1]);
        using var sha = SHA256.Create();
        var value = Convert.ToHexString(sha.ComputeHash(stream)).ToLowerInvariant();
        var pass = value == args[2].ToLowerInvariant();
        Console.WriteLine(pass ? "sha256-ok" : $"sha256-failed:{value}");
        return pass ? 0 : 2;
    }
}
