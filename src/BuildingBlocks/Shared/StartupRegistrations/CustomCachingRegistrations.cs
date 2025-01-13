using Microsoft.Extensions.DependencyInjection;
using Shared.Services.CachingServices.MemoryCache;

namespace Shared.StartupRegistrations;

public static class CustomCachingRegistrations
{
    public static IServiceCollection ConfigureMemoryCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IMemoryCacheService, MemoryCacheService>();
        return services;
    }
}