using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.StartupRegistrations;

public static class CustomCorsRegistrations
{
    private const string CorsPolicyName = "CorsPolicy";
    
    public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigin = configuration["AllowedOrigin"] ?? "*";
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, builder =>
            {
                builder
                    .WithOrigins(allowedOrigin)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        return services;
    }

    public static IApplicationBuilder UseCors(this IApplicationBuilder app)
    {
        app.UseCors(CorsPolicyName);
        return app;
    }
}