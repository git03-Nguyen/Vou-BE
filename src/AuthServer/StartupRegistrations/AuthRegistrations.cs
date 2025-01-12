using System.Text;
using AuthServer.Common;
using AuthServer.Data.Contexts;
using AuthServer.Data.Models;
using AuthServer.Options;
using AuthServer.Services.ProfileService;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Options;

namespace AuthServer.StartupRegistrations;

public static class AuthRegistrations
{
    public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();
        
        services.AddIdentityServer()
            .AddInMemoryClients(AuthConfig.Clients)
            .AddInMemoryIdentityResources(AuthConfig.IdentityResources)
            .AddInMemoryApiScopes(AuthConfig.ApiScopes)
            .AddInMemoryApiResources(AuthConfig.ApiResources)
            .AddDeveloperSigningCredential()
            .AddAspNetIdentity<User>()
            .AddProfileService<CustomProfileService>();
        services.AddTransient<IProfileService, CustomProfileService>();
        return services;
    }

    public static IServiceCollection ConfigureAuthenticate(this IServiceCollection services, IConfiguration configuration)
    {
        var authServerOptions = configuration.GetSection(AuthenticationOptions.OptionName).Get<AuthenticationOptions>();
        var authorityUrl = authServerOptions?.Authority;
        var authSecret = authServerOptions?.Secret;
        var key = Encoding.ASCII.GetBytes(authSecret!);
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
            {
                option.Authority = authorityUrl;
                option.RequireHttpsMetadata = false;
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Shared.Common.Constants.ADMIN, policy =>
            {
                policy.RequireAssertion(context => context.User.HasClaim("ROLE", Shared.Common.Constants.ADMIN));
            });
            
            options.AddPolicy(Shared.Common.Constants.COUNTERPART, policy =>
            {
                policy.RequireAssertion(context => context.User.HasClaim("ROLE", Shared.Common.Constants.COUNTERPART));
            });
            
            options.AddPolicy(Shared.Common.Constants.PLAYER, policy =>
            {
                policy.RequireAssertion(context => context.User.HasClaim("ROLE", Shared.Common.Constants.PLAYER));
            });
        });
            
        
        return services;
    }
}