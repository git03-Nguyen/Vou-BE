using GameService.Data.Contexts;

namespace GameService.StartupRegistrations;

public static class DatabaseRegistrations
{
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GameDbContext>();
        return services;
    }
}