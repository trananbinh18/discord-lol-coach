using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordLoLCoach.API.Handlers;
using DiscordLoLCoach.API.Interfaces;
using DiscordLoLCoach.API.Services;
using DiscordLoLCoach.Application.Interfaces;

namespace DiscordLoLCoach.API.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<DiscordConnectionService>();
        services.AddSingleton<ICommunicationConnectionService>(sp => sp.GetRequiredService<DiscordConnectionService>());
        services.AddSingleton<DiscordSocketClient>(sp => sp.GetRequiredService<DiscordConnectionService>().Client);
        services.AddSingleton<InteractionService>(sp => sp.GetRequiredService<DiscordConnectionService>().InteractionService);

        services.AddScoped<ICommunicationContextService, DiscordCommunicationContextService>();

        services.AddSingleton<IDiscordEventHandler, DiscordEventHandler>();
        services.AddHostedService<DiscordBotBackgroundService>();

        return services;
    }
}
