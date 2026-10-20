namespace DiscordLoLCoach.Application.Services;

using System.Linq;
using Microsoft.Extensions.Configuration;
using DiscordLoLCoach.Application.Interfaces;
using DiscordLoLCoach.Application.Models;

/// <summary>
/// Singleton implementation of IAppSetting that binds configurations from appsettings.
/// </summary>
public class AppSetting : IAppSetting
{
    public DiscordSettings DiscordSettings { get; } = new();

    public AppSetting(IConfiguration configuration)
    {
        configuration.GetSection("DiscordSettings").Bind(DiscordSettings);
    }

    /// <inheritdoc />
    public bool IsServerAllowed(ulong? serverId)
    {
        // If AllowServerIds is null, empty, or all elements are whitespace, allow all
        var configuredIds = DiscordSettings.AllowServerIds?
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();

        if (configuredIds == null || configuredIds.Count == 0)
        {
            return true;
        }

        if (!serverId.HasValue)
        {
            return false;
        }

        return configuredIds.Contains(serverId.Value.ToString());
    }
}
