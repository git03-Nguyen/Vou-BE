using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Services.HttpContextAccessor;

namespace Shared.StartupRegistrations;

public static class CustomHttpContextRegistrations
{
    public static IServiceCollection ConfigureCustomHttpContext(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.TryAddSingleton<ICustomHttpContextAccessor, CustomHttpContextAccessor>();
        return services;
    }
}