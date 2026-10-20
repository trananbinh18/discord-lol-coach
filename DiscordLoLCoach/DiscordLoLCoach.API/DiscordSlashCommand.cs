namespace DiscordLoLCoach.API;

using System.Threading.Tasks;
using Discord.Interactions;
using DiscordLoLCoach.Application.Business;
using DiscordLoLCoach.Application.Interfaces;
using DiscordLoLCoach.API.Services;

/// <summary>
/// Slash command interaction module for Discord.
/// </summary>
public class DiscordSlashCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly MessageBusinessLogic _businessLogic;
    private readonly ICommunicationContextService _contextService;

    public DiscordSlashCommand(
        MessageBusinessLogic businessLogic,
        ICommunicationContextService contextService)
    {
        _businessLogic = businessLogic;
        _contextService = contextService;
    }

    public override void BeforeExecute(ICommandInfo command)
    {
        EnsureContextInitialized();
    }

    [SlashCommand("ping", "Replies with a pong!")]
    public async Task PingAsync()
    {
        await _businessLogic.ProcessSlashPingAsync();
    }

    private void EnsureContextInitialized()
    {
        if (_contextService is DiscordCommunicationContextService discordContext && Context != null)
        {
            discordContext.InitializeForInteraction(Context);
        }
    }
}
