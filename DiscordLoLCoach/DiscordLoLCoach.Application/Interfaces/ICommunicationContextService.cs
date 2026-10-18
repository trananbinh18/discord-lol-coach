namespace DiscordLoLCoach.Application.Interfaces;

using System.Threading.Tasks;

/// <summary>
/// Scoped service abstraction tied to a specific incoming user message.
/// Stores interaction context (user message, channel, and acknowledgment) and provides methods to communicate back.
/// </summary>
public interface ICommunicationContextService
{
    /// <summary>
    /// Content or payload of the incoming user message.
    /// </summary>
    string? UserMessage { get; }

    /// <summary>
    /// Unique identifier of the incoming user message.
    /// </summary>
    ulong UserMessageId { get; }

    /// <summary>
    /// Identifier of the channel where the message was received.
    /// </summary>
    ulong ChannelId { get; }

    /// <summary>
    /// Content of the initial acknowledgment message (e.g., "thinking..." state).
    /// </summary>
    string? AckMessage { get; }

    /// <summary>
    /// Identifier of the initial acknowledgment message.
    /// </summary>
    ulong AckMessageId { get; }

    /// <summary>
    /// Sends an initial acknowledgment for the user message.
    /// </summary>
    Task SendAckAsync(string ackContent);

    /// <summary>
    /// Updates the acknowledged message or sends the final response to the channel.
    /// </summary>
    Task SendOrUpdateResponseAsync(string content);
}
