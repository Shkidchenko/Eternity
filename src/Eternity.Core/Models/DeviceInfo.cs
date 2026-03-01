namespace Eternity.Core.Models;

/// <summary>Represents an Android device visible to transport backends.</summary>
public sealed record DeviceInfo(string Serial, string DisplayName, DeviceMode Mode, string State);

/// <summary>Known device modes.</summary>
public enum DeviceMode { Unknown, Adb, Fastboot, Recovery }
