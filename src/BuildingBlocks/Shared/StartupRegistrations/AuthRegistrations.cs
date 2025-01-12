using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Common;
using Shared.Options;

namespace Shared.StartupRegistrations;

public static class AuthRegistrations
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authServerOptions = configuration.GetSection(AuthenticationOptions.OptionName).Get<AuthenticationOptions>();
        var secretBytes = Encoding.ASCII.GetBytes(authServerOptions?.Secret ?? String.Empty);
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
        {
            option.Authority = authServerOptions?.Authority;
            option.RequireHttpsMetadata = false;
            option.SaveToken = true;
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = false,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretBytes)
            };
        });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Constants.ADMIN, policy =>
            {
                policy.RequireAssertion(context => context.User.HasClaim("ROLE", Constants.ADMIN));
            });
            
            options.AddPolicy(Constants.COUNTERPART, policy =>
            {
                policy.RequireAssertion(context => context.User.HasClaim("ROLE", Constants.COUNTERPART));
            });
            
            options.AddPolicy(Constants.PLAYER, policy =>
            {
                policy.RequireAssertion(context => context.User.HasClaim("ROLE", Constants.PLAYER));
            });
        });
        
        return services;
    }
    
    public static IApplicationBuilder UseAuthentication(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}