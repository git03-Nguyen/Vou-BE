using Microsoft.IdentityModel.Tokens;
using Shared.Options;

namespace ApiGateway.StartupRegistrations;

public static class AuthenticationRegistrations
{
    public static IServiceCollection ConfigureAuthenticate(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetSection(AuthenticationOptions.OptionName).Get<AuthenticationOptions>();
        var providerKey = authOptions?.ProviderKey;
        var authority = authOptions?.Authority;

        if (providerKey is not null)
        {
            services.AddAuthentication().AddJwtBearer(providerKey, options =>
            {
                options.Authority = authority;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });
        }

        return services;
    }
}