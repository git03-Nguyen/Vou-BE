namespace AuthServer.StartupRegistrations;

public static class CustomServicesRegistrations
{
    public static IServiceCollection ConfigureDIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        return services;
    }
}