namespace DiscordLoLCoach.API.Interfaces;

/// <summary>
/// Abstraction for initializing and handling Discord inbound events (messages, interactions, ready state).
/// </summary>
public interface IDiscordEventHandler
{
    /// <summary>
    /// Hooks event listeners to the DiscordSocketClient and InteractionService.
    /// </summary>
    void Initialize();
}
