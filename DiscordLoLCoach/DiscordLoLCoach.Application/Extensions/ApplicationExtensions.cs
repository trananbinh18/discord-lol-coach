using Microsoft.Extensions.DependencyInjection;

namespace DiscordLoLCoach.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<Interfaces.IAppSetting, Services.AppSetting>();
        services.AddScoped<Business.MessageBusinessLogic>();

        return services;
    }
}

