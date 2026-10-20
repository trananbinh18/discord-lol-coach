namespace DiscordLoLCoach.API.Services;

using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordLoLCoach.Application.Interfaces;

/// <summary>
/// Scoped implementation of ICommunicationContextService for Discord interactions and messages.
/// </summary>
public class DiscordCommunicationContextService : ICommunicationContextService, IDisposable
{
    private SocketUserMessage? _userMessage;
    private SocketInteractionContext? _interactionContext;
    private IUserMessage? _ackMessage;
    private IDisposable? _typingState;

    public string? UserMessage { get; private set; }
    public ulong UserMessageId { get; private set; }
    public ulong ChannelId { get; private set; }
    public string? AckMessage { get; private set; }
    public ulong AckMessageId { get; private set; }

    /// <summary>
    /// Initializes context with an incoming Discord text message.
    /// </summary>
    public void InitializeForMessage(SocketUserMessage message)
    {
        _userMessage = message ?? throw new ArgumentNullException(nameof(message));
        UserMessage = message.Content;
        UserMessageId = message.Id;
        ChannelId = message.Channel.Id;
    }

    /// <summary>
    /// Initializes context with a Discord interaction (such as a slash command).
    /// </summary>
    public void InitializeForInteraction(SocketInteractionContext interactionContext)
    {
        _interactionContext = interactionContext ?? throw new ArgumentNullException(nameof(interactionContext));
        UserMessage = interactionContext.Interaction.Data?.ToString();
        UserMessageId = interactionContext.Interaction.Id;
        ChannelId = interactionContext.Channel.Id;
    }

    /// <inheritdoc />
    public async Task SendAckAsync(string ackContent)
    {
        AckMessage = ackContent;

        if (_userMessage != null)
        {
            _typingState = _userMessage.Channel.EnterTypingState();
            _ackMessage = await _userMessage.Channel.SendMessageAsync(ackContent);
            AckMessageId = _ackMessage.Id;
        }
        else if (_interactionContext != null)
        {
            if (!_interactionContext.Interaction.HasResponded)
            {
                await _interactionContext.Interaction.DeferAsync();
            }

            if (_interactionContext.Channel != null)
            {
                _typingState = _interactionContext.Channel.EnterTypingState();
            }
        }
    }

    /// <inheritdoc />
    public async Task SendOrUpdateResponseAsync(string content)
    {
        _typingState?.Dispose();
        _typingState = null;

        if (_ackMessage != null)
        {
            await _ackMessage.DeleteAsync();
        }

        if (_userMessage != null)
        {
            var sent = await _userMessage.ReplyAsync(content);
            AckMessageId = sent.Id;
        }
        else if (_interactionContext != null)
        {
            if (_interactionContext.Interaction.HasResponded)
            {
                await _interactionContext.Interaction.ModifyOriginalResponseAsync(msg => msg.Content = content);
            }
            else
            {
                await _interactionContext.Interaction.RespondAsync(content);
            }
        }
        else
        {
            throw new InvalidOperationException("Cannot send or update response: Discord communication context has not been initialized with a user message or interaction.");
        }
    }

    public void Dispose()
    {
        _typingState?.Dispose();
        _typingState = null;
        GC.SuppressFinalize(this);
    }
}
