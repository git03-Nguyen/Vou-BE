using EventService.Data.Contexts;

namespace EventService.StartupRegistrations;

public static class DatabaseRegistrations
{
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EventDbContext>();
        return services;
    }
}