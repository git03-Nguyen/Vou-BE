using GameService.Repositories;
using GameService.Repositories.Implements;
using GameService.Repositories.Interfaces;

namespace GameService.StartupRegistrations;

public static class CustomDIRegistrations
{
    public static IServiceCollection ConfigureDIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IPlayerQuizSessionRepository, PlayerQuizSessionRepository>();
        services.AddScoped<IPlayerShakeSessionRepository, PlayerShakeSessionRepository>();
        services.AddScoped<IQuizSessionRepository, QuizSessionRepository>();
        services.AddScoped<IQuizSetRepository, QuizSetRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}