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
            options.Tokens.EmailConfirmationTokenProvider = EmailConfirmationTokenProvider<User>.ProviderName;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<EmailConfirmationTokenProvider<User>>(EmailConfirmationTokenProvider<User>.ProviderName);
        
        services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(1));
        services.Configure<EmailConfirmationTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromDays(365));
       
        var identityBuilder = services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
                options.Caching.ClientStoreExpiration = TimeSpan.FromDays(10);
                options.Caching.ResourceStoreExpiration = TimeSpan.FromDays(10);
            })
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddDeveloperSigningCredential()
            .AddAspNetIdentity<User>()
            .AddProfileService<CustomProfileService>();
        services.AddTransient<IProfileService, CustomProfileService>();
        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
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
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        return services;
    }
}