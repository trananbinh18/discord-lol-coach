namespace DiscordLoLCoach.API.Handlers;

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DiscordLoLCoach.API.Interfaces;
using DiscordLoLCoach.Application.Business;
using DiscordLoLCoach.Application.Interfaces;
using DiscordLoLCoach.API.Services;

/// <summary>
/// Presentation-layer event handler that routes Discord gateway events to Application business logic.
/// </summary>
public class DiscordEventHandler : IDiscordEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly IAppSetting _appSetting;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DiscordEventHandler> _logger;

    public DiscordEventHandler(
        DiscordSocketClient client,
        InteractionService interactionService,
        IAppSetting appSetting,
        IServiceScopeFactory scopeFactory,
        ILogger<DiscordEventHandler> logger)
    {
        _client = client;
        _interactionService = interactionService;
        _appSetting = appSetting;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public void Initialize()
    {
        _client.Ready += OnReadyAsync;
        _client.MessageReceived += OnMessageReceivedAsync;
        _client.InteractionCreated += OnInteractionCreatedAsync;
        _interactionService.SlashCommandExecuted += OnSlashCommandExecutedAsync;
    }

    private async Task OnReadyAsync()
    {
        _logger.LogInformation("Discord client connected as {User}. Registering slash commands from API layer...", _client.CurrentUser?.Username);

        try
        {
            var apiAssembly = Assembly.GetExecutingAssembly();
            using var scope = _scopeFactory.CreateScope();
            await _interactionService.AddModulesAsync(apiAssembly, scope.ServiceProvider);
            await _interactionService.RegisterCommandsGloballyAsync();
            _logger.LogInformation("Discord slash commands registered successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register Discord slash commands.");
        }
    }

    private Task OnMessageReceivedAsync(SocketMessage rawMessage)
    {
        if (rawMessage is not SocketUserMessage message || message.Author.IsBot)
        {
            return Task.CompletedTask;
        }

        // Check if the current bot is mentioned
        if (_client.CurrentUser == null || !message.MentionedUsers.Any(u => u.Id == _client.CurrentUser.Id))
        {
            return Task.CompletedTask;
        }

        // Validate allowed servers
        ulong? guildId = (message.Channel as SocketGuildChannel)?.Guild.Id;
        if (!_appSetting.IsServerAllowed(guildId))
        {
            _logger.LogWarning("Message received from unauthorized server: {GuildId}", guildId);
            return Task.CompletedTask;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var contextService = scope.ServiceProvider.GetRequiredService<ICommunicationContextService>() as DiscordCommunicationContextService;
                var businessLogic = scope.ServiceProvider.GetRequiredService<MessageBusinessLogic>();

                contextService?.InitializeForMessage(message);

                if (contextService != null)
                {
                    await contextService.SendAckAsync("Thinking...");
                }

                await businessLogic.ProcessNormalMessageAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing normal message {MessageId}", message.Id);
            }
        });

        return Task.CompletedTask;
    }

    private Task OnInteractionCreatedAsync(SocketInteraction interaction)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                if (interaction.GuildId.HasValue && !_appSetting.IsServerAllowed(interaction.GuildId.Value))
                {
                    await interaction.RespondAsync("This server is not authorized to use this bot.", ephemeral: true);
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var context = new SocketInteractionContext(_client, interaction);
                var contextService = scope.ServiceProvider.GetRequiredService<ICommunicationContextService>() as DiscordCommunicationContextService;
                contextService?.InitializeForInteraction(context);

                if (contextService != null)
                {
                    await contextService.SendAckAsync("Processing...");
                }

                var result = await _interactionService.ExecuteCommandAsync(context, scope.ServiceProvider);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Slash command execution returned: {Error} - {Reason}", result.Error, result.ErrorReason);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling interaction {InteractionId}", interaction.Id);

                if (interaction.HasResponded)
                {
                    await interaction.ModifyOriginalResponseAsync(msg => msg.Content = "An error occurred while executing the command.");
                }
                else
                {
                    await interaction.RespondAsync("An error occurred while executing the command.", ephemeral: true);
                }
            }
        });

        return Task.CompletedTask;
    }

    private Task OnSlashCommandExecutedAsync(SlashCommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
        {
            _logger.LogError("Slash command '{CommandName}' failed: {Error} - {Reason}", commandInfo.Name, result.Error, result.ErrorReason);
        }
        else
        {
            _logger.LogInformation("Slash command '{CommandName}' executed successfully.", commandInfo.Name);
        }

        return Task.CompletedTask;
    }
}
