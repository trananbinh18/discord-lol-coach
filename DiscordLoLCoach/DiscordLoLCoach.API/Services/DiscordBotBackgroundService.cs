namespace DiscordLoLCoach.API.Services;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DiscordLoLCoach.API.Interfaces;
using DiscordLoLCoach.Application.Interfaces;

/// <summary>
/// Hosted background service in API that initializes event listeners and manages the Discord bot communication connection.
/// </summary>
public class DiscordBotBackgroundService : BackgroundService
{
    private readonly ICommunicationConnectionService _connectionService;
    private readonly IDiscordEventHandler _eventHandler;
    private readonly ILogger<DiscordBotBackgroundService> _logger;

    public DiscordBotBackgroundService(
        ICommunicationConnectionService connectionService,
        IDiscordEventHandler eventHandler,
        ILogger<DiscordBotBackgroundService> logger)
    {
        _connectionService = connectionService;
        _eventHandler = eventHandler;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing Discord event handlers...");
        _eventHandler.Initialize();
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Discord bot connection...");

        try
        {
            await _connectionService.StartAsync(stoppingToken);

            // Keep the background service active until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Discord bot background service execution canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in Discord bot background service.");
        }
        finally
        {
            _logger.LogInformation("Stopping Discord bot connection...");
            await _connectionService.StopAsync(CancellationToken.None);
        }
    }
}
