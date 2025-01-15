using PaymentService.Repositories;
using PaymentService.Repositories.Implements;
using PaymentService.Repositories.Interfaces;
using PaymentService.Services.EventPublishService;

namespace PaymentService.StartupRegistrations;

public static class CustomDIRegistrations
{
    public static IServiceCollection ConfigureDIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IQuizSessionRepository, QuizSessionRepository>();
        services.AddScoped<IFavoriteEventRepository, FavoriteEventRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IEventPublishService, EventPublishService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}