using Microsoft.Extensions.DependencyInjection;

namespace DiscordLoLCoach.API.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // TODO: Register API layer specific services (e.g., Swagger, custom middlewares, webhooks)
        
        return services;
    }
}
