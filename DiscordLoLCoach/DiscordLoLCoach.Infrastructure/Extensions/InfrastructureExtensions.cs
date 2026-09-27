using Microsoft.Extensions.DependencyInjection;

namespace DiscordLoLCoach.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // TODO: Register Infrastructure layer services (e.g., DbContext, Discord client, caching implementations)

        return services;
    }
}
