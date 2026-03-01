namespace Eternity.Core.Packages;

/// <summary>Describes image discovered in flashing package.</summary>
public sealed record ImageEntry(string Path, string SuggestedPartition, string Sha256, bool Verified);

/// <summary>Parsed package model.</summary>
public sealed record FlashPackage(string FilePath, IReadOnlyList<ImageEntry> Images, IReadOnlyList<string> Warnings);
