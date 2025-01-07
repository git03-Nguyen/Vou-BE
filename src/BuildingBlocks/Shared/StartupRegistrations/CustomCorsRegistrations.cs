using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.StartupRegistrations;

public static class CustomCorsRegistrations
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigin = configuration["AllowedOrigin"] ?? "http://localhost:3000";
        services.AddCors(options =>
        {
            options.AddPolicy(Constants.CommonConstants.CorsPolicyName, builder =>
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
}