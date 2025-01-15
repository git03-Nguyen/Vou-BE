using PaymentService.Options;
using Shared.Options;

namespace PaymentService.StartupRegistrations;

public static class CustomOptionsRegistrations
{
    public static IServiceCollection ConfigureCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.OptionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.OptionName));
        services.Configure<HangfireOptions>(configuration.GetSection(HangfireOptions.OptionName));
        return services;
    }
}