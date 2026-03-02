using System.Security.Cryptography;
using Eternity.Core.Errors;
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;

namespace Eternity.Core.Packages;

/// <summary>Parses zip/tar flashing archives and computes integrity checks.</summary>
public sealed class PackageParser
{
    /// <summary>Parses flashing package from file.</summary>
    public async Task<Result<FlashPackage>> ParseAsync(string filePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return Result<FlashPackage>.Fail(new OperationError(ErrorCode.ValidationFailed, "Package file missing", "package"));
        }

        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        if (ext is not ".zip" and not ".tar")
        {
            return Result<FlashPackage>.Fail(new OperationError(ErrorCode.UnsupportedPackage, "Only zip/tar packages are supported", "package"));
        }

        var images = new List<ImageEntry>();
        var warnings = new List<string>();
        using IArchive archive = ext == ".zip"
            ? ZipArchive.Open(filePath)
            : TarArchive.Open(filePath);
        foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var name = entry.Key.Replace('\\', '/');
            if (!IsImage(name))
            {
                continue;
            }

            await using var stream = entry.OpenEntryStream();
            var sha = await ComputeShaAsync(stream, cancellationToken).ConfigureAwait(false);
            var partition = SuggestPartition(Path.GetFileName(name));
            images.Add(new ImageEntry(name, partition, sha, true));
            if (partition == "unknown")
            {
                warnings.Add($"Unrecognized image target: {name}");
            }
        }

        if (images.Count == 0)
        {
            warnings.Add("No flashable image entries detected.");
        }

        return Result<FlashPackage>.Success(new FlashPackage(filePath, images, warnings));
    }

    /// <summary>Suggests target partition for image file.</summary>
    public static string SuggestPartition(string fileName)
    {
        var normalized = fileName.ToLowerInvariant();
        if (normalized.Contains("boot")) return "boot";
        if (normalized.Contains("vbmeta")) return "vbmeta";
        if (normalized.Contains("system")) return "system";
        if (normalized.Contains("vendor")) return "vendor";
        if (normalized.Contains("recovery")) return "recovery";
        return "unknown";
    }

    private static bool IsImage(string name)
        => name.EndsWith(".img", StringComparison.OrdinalIgnoreCase)
            || name.EndsWith(".sparseimg", StringComparison.OrdinalIgnoreCase)
            || name.Contains("vbmeta", StringComparison.OrdinalIgnoreCase);

    private static async Task<string> ComputeShaAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var sha = SHA256.Create();
        var hash = await sha.ComputeHashAsync(stream, cancellationToken).ConfigureAwait(false);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
