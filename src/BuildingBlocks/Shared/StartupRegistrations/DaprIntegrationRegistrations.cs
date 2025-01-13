using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;
using Shared.Services.CachingServices.DistributedCache;

namespace Shared.StartupRegistrations;

public static class DaprIntegrationRegistrations
{
    public static IServiceCollection ConfigureDaprIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "localhost")
        {
            services.AddDaprClient();
        }
        else
        {
            var daprOptions = configuration.GetSection(DaprOptions.OptionName).Get<DaprOptions>();
            services.AddDaprClient(options =>
            {
                options.UseHttpEndpoint(daprOptions?.DaprHttpEndpoint ?? "http://localhost:3500");
            });
        }
        
        services.AddScoped<IDaprStateStoreService, DaprStateStoreService>();
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