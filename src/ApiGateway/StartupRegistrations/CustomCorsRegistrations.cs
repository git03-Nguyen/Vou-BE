namespace ApiGateway.StartupRegistrations;

public static class CustomCorsRegistrations
{
    private const string CorsPolicy = "CorsPolicy";
    private const string DefaultAllowedOrigin = "http://localhost:3000";

    public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigin = configuration["AllowedOrigin"] ?? DefaultAllowedOrigin;
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, builder =>
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
    
    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
    {
        app.UseCors(CorsPolicy);
        return app;
    }
}