namespace DiscordLoLCoach.Application.Models;

using System.Collections.Generic;

/// <summary>
/// Configuration settings for the Discord Bot.
/// </summary>
public class DiscordSettings
{
    /// <summary>
    /// Bot token for Discord authentication.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// List of allowed server (guild) IDs.
    /// If null or empty, all servers are allowed.
    /// </summary>
    public List<string> AllowServerIds { get; set; } = new();
}
