using AuthServer.Options;

namespace AuthServer.Extensions;

public static class CustomOptionsRegistrations
{
    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthServerOption>(configuration.GetSection(AuthServerOption.OptionName));
        services.Configure<DbSettingsOption>(configuration.GetSection(DbSettingsOption.OptionName));
        return services;
    }
}