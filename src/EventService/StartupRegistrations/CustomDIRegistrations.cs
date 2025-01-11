using EventService.Repositories;
using EventService.Repositories.Implements;
using EventService.Repositories.Interfaces;

namespace EventService.StartupRegistrations;

public static class CustomDIRegistrations
{
    public static IServiceCollection ConfigureDIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();
        services.AddScoped<IVoucherInEventRepository, VoucherInEventRepository>();
        services.AddScoped<IVoucherToPlayerRepository, VoucherToPlayerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}