using Microsoft.Extensions.DependencyInjection;
using Shared.Services.CachingServices;
using Shared.Services.CachingServices.DistributedCache;
using Shared.Services.CachingServices.MemoryCache;

namespace Shared.StartupRegistrations;

public static class CustomCachingRegistrations
{
    public static IServiceCollection ConfigureMemoryCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IMemoryCacheService, MemoryCacheService>();
        services.AddScoped<ICacheService, MemoryCacheService>();
        return services;
    }
    
    public static IServiceCollection ConfigureDaprStateStore(this IServiceCollection services)
    {
        services.AddScoped<IDaprStateStoreService, DaprStateStoreService>();
        services.AddScoped<ICacheService, DaprStateStoreService>();
        return services;
    }
}