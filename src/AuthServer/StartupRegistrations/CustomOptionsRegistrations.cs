using AuthServer.Options;
using Shared.Options;

namespace AuthServer.StartupRegistrations;

public static class CustomOptionsRegistrations
{
    public static IServiceCollection ConfigureCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.OptionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.OptionName));
        services.Configure<EmailSettingsOptions>(configuration.GetSection(EmailSettingsOptions.OptionName));
        services.Configure<DaprOptions>(configuration.GetSection(DaprOptions.OptionName));
        return services;
    }
}