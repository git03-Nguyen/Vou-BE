namespace PaymentService.StartupRegistrations;

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