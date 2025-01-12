using Microsoft.Extensions.DependencyInjection;

namespace Shared.StartupRegistrations;

public static class DaprClientRegistrations
{
    public static IServiceCollection ConfigureDaprIntegration(this IServiceCollection services)
    {
        services.AddDaprClient();
        return services;
    }
}