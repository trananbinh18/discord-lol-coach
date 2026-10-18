namespace DiscordLoLCoach.Application.Interfaces;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Abstraction for maintaining and managing the persistent connection (e.g., WebSocket)
/// to communication platforms such as Discord.
/// Registered as a long-lived Singleton service that maintains the WebSocket connection.
/// </summary>
public interface ICommunicationConnectionService
{
    /// <summary>
    /// Gets a value indicating whether the connection to the communication service is currently active.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Starts and maintains the persistent connection.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes and cleans up the active connection.
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);
}
