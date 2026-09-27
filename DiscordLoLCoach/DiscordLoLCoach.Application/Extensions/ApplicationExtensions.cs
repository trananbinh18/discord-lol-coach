using Microsoft.Extensions.DependencyInjection;

namespace DiscordLoLCoach.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // TODO: Register Application layer services (e.g., MediatR handlers, Validators, etc.)
        
        return services;
    }
}
