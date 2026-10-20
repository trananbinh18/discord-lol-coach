namespace DiscordLoLCoach.Application.Interfaces;

using DiscordLoLCoach.Application.Models;

/// <summary>
/// Singleton application settings service loaded from appsettings.
/// </summary>
public interface IAppSetting
{
    /// <summary>
    /// Gets the Discord configuration settings.
    /// </summary>
    DiscordSettings DiscordSettings { get; }

    /// <summary>
    /// Evaluates whether the given server ID is permitted to use the application.
    /// If AllowServerIds is null, empty, or contains only blank entries, all servers are allowed.
    /// </summary>
    bool IsServerAllowed(ulong? serverId);
}
