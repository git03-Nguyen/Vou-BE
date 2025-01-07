using AuthServer.Options;
using Shared.Options;

namespace AuthServer.StartupRegistrations;

public static class CustomOptionsRegistrations
{
    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.OptionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.OptionName));
        return services;
    }
}