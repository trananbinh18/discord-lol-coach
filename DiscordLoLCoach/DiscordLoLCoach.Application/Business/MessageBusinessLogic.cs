namespace DiscordLoLCoach.Application.Business;

using System.Threading.Tasks;
using DiscordLoLCoach.Application.Interfaces;

/// <summary>
/// Core business logic service for processing user messages and orchestrating coaching workflows.
/// </summary>
public class MessageBusinessLogic
{
    private readonly ICommunicationContextService _contextService;

    public MessageBusinessLogic(ICommunicationContextService contextService)
    {
        _contextService = contextService;
    }

    /// <summary>
    /// Processes normal text messages mentioning the bot.
    /// </summary>
    public async Task ProcessNormalMessageAsync()
    {
        // For now just reply "Received your message."
        await _contextService.SendOrUpdateResponseAsync("Received your message.");
    }

    /// <summary>
    /// Processes the /ping slash command.
    /// </summary>
    public async Task ProcessSlashPingAsync()
    {
        // For now just reply "Pong"
        await _contextService.SendOrUpdateResponseAsync("Pong");
    }

    /// <summary>
    /// Executes the primary business logic for the message encapsulated in the current communication context.
    /// </summary>
    public async Task ProcessMessageAsync()
    {
        // Coordinates conversation retrieval, anti-spam locks, MCP tool execution, and LLM coaching responses.
        await Task.CompletedTask;
    }
}
