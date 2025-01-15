using PaymentService.Data.Contexts;

namespace PaymentService.StartupRegistrations;

public static class DatabaseRegistrations
{
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EventDbContext>();
        return services;
    }
}