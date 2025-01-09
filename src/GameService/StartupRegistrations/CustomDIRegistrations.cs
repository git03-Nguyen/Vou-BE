using GameService.Repositories;
using GameService.Repositories.Implements;
using GameService.Repositories.Interfaces;

namespace GameService.StartupRegistrations;

public static class CustomDIRegistrations
{
    public static IServiceCollection ConfigureDIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<GameRepository, GameRepository>();
        services.AddScoped<IGameSessionRepository, GameSessionRepository>();
        services.AddScoped<IVoucherInGameSessionRepository, VoucherInGameSessionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}