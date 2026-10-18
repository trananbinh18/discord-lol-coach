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
    /// Executes the primary business logic for the message encapsulated in the current communication context.
    /// </summary>
    public async Task ProcessMessageAsync()
    {
        // Coordinates conversation retrieval, anti-spam locks, MCP tool execution, and LLM coaching responses.
        await Task.CompletedTask;
    }
}
