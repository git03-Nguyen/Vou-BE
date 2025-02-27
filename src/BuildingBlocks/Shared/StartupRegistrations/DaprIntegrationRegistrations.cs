using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;
using Shared.Services.CachingServices.DistributedCache;
using Shared.Services.ServiceInvocation;

namespace Shared.StartupRegistrations;

public static class DaprIntegrationRegistrations
{
    public static IServiceCollection ConfigureDaprIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDaprClient();
        services.AddScoped<IDaprStateStoreService, DaprStateStoreService>();
        services.AddScoped<IServiceInvocationService, ServiceInvocationService>();
        return services;
    }
    
    public static IApplicationBuilder UseDaprIntegration(this IApplicationBuilder app)
    {
        app.UseCloudEvents();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapSubscribeHandler();
        });
        return app;
    }
}