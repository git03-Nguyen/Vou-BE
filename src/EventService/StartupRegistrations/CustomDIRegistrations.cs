using EventService.Repositories;
using EventService.Repositories.Implements;
using EventService.Repositories.Interfaces;
using EventService.Services.PubSubService;

namespace EventService.StartupRegistrations;

public static class CustomDIRegistrations
{
    public static IServiceCollection ConfigureDIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<ICounterPartRepository, CounterPartRepository>();
        services.AddScoped<IQuizSessionRepository, QuizSessionRepository>();
        services.AddScoped<IQuizSetRepository, QuizSetRepository>();
        services.AddScoped<IVoucherToPlayerRepository, VoucherToPlayerRepository>();
        services.AddScoped<IFavoriteEventRepository, FavoriteEventRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IEventPublishService, EventPublishService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}