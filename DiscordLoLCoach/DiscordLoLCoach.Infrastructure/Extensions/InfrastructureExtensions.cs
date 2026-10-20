using Microsoft.Extensions.DependencyInjection;

namespace DiscordLoLCoach.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Future infrastructure services (persistence, caching, external MCP clients) will be registered here.
        return services;
    }
}
