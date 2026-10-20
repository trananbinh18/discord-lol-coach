namespace DiscordLoLCoach.API.Services;

using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using DiscordLoLCoach.Application.Interfaces;

/// <summary>
/// Singleton service managing the persistent WebSocket connection to the Discord Gateway.
/// </summary>
public class DiscordConnectionService : ICommunicationConnectionService, IAsyncDisposable
{
    private readonly IAppSetting _appSetting;
    private readonly ILogger<DiscordConnectionService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private bool _isConnected;

    public DiscordConnectionService(
        IAppSetting appSetting,
        ILogger<DiscordConnectionService> logger)
    {
        _appSetting = appSetting;
        _logger = logger;

        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.DirectMessages |
                             GatewayIntents.MessageContent,
            LogLevel = LogSeverity.Info
        };

        _client = new DiscordSocketClient(config);
        _interactionService = new InteractionService(_client.Rest, new InteractionServiceConfig
        {
            LogLevel = LogSeverity.Info,
            DefaultRunMode = RunMode.Sync,
            AutoServiceScopes = false
        });

        _client.Log += OnLogAsync;
        _interactionService.Log += OnLogAsync;

        _client.Connected += () =>
        {
            _isConnected = true;
            return Task.CompletedTask;
        };

        _client.Disconnected += _ =>
        {
            _isConnected = false;
            return Task.CompletedTask;
        };
    }

    public bool IsConnected => _isConnected;

    public DiscordSocketClient Client => _client;

    public InteractionService InteractionService => _interactionService;

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var token = _appSetting.DiscordSettings.Token;
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Discord token is empty. DiscordConnectionService will not establish a connection.");
            return;
        }

        _logger.LogInformation("Connecting to Discord Gateway...");
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Disconnecting from Discord Gateway...");
        await _client.StopAsync();
        await _client.LogoutAsync();
        _isConnected = false;
    }

    private Task OnLogAsync(LogMessage logMessage)
    {
        var level = logMessage.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.Information
        };

        _logger.Log(level, logMessage.Exception, "[Discord] {Message}", logMessage.Message);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _client.Dispose();
        _interactionService.Dispose();
        GC.SuppressFinalize(this);
    }
}
