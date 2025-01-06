using System.Net;
using System.Text;
using Asp.Versioning;
using AuthServer.Data.Contexts;
using AuthServer.Data.Entities;
using AuthServer.Options;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Extensions;
using Shared.Validation;

namespace AuthServer.Extensions;

public static class CustomServicesRegistrations
{
    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                    var response = new ValidationErrorResponse
                    {
                        Status = HttpStatusCode.BadRequest.ToInt(),
                        Message = "One or more validation errors occurred",
                        Data = errors.Select(x => new ValidationError { Message = x }).ToList()
                    };
                    return response.ToObjectResult();
                };
            });
        return services;
    }
    
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettingsOptions = configuration.GetSection(DbSettingsOption.OptionName).Get<DbSettingsOption>();
        var connectionString = dbSettingsOptions?.ConnectionString;
        services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(connectionString, builder =>
        {
            builder.EnableRetryOnFailure();
        }));
        return services;
    }
    
    public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(x =>
        {
            x.DefaultApiVersion = new ApiVersion(1, 0);
            x.AssumeDefaultVersionWhenUnspecified = true;
            x.ReportApiVersions = true;
            x.UnsupportedApiVersionStatusCode = HttpStatusCode.NotFound.ToInt();
        });
        return services;
    }
    
    public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services)
    {
        var cacheSettingsOptions = _configuration.GetSection(CacheSettingsOptions.OptionName).Get<CacheSettingsOptions>();
        
        services
            .AddIdentity<User, IdentityRole>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = Constants.EmailConfirmation;
                options.Tokens.PasswordResetTokenProvider = Constants.ResetPassword;
            })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<EmailConfirmationTokenProvider<User>>(Constants.EmailConfirmation)
            .AddTokenProvider<ResetPasswordTokenProvider<User>>(Constants.ResetPassword); 
        
        services.Configure<DataProtectionTokenProviderOptions>(options => 
            options.TokenLifespan = TimeSpan.FromHours(1));
        services.Configure<EmailConfirmationTokenProviderOptions>(options =>
             options.TokenLifespan = TimeSpan.FromDays(365));
        services.Configure<ResetPasswordTokenProviderOptions>(opt =>
             opt.TokenLifespan = TimeSpan.FromHours(1));
       
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
            });

        var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var connectionMultiplexer = ConnectionMultiplexer.Connect(cacheSettingsOptions.ConnectionString);
            
            identityBuilder
            .AddOperationalStore(options =>
            { 
                options.RedisConnectionMultiplexer = connectionMultiplexer;
                options.KeyPrefix = IdentityUserCache.Operational;
                options.Db = cacheSettingsOptions.DbNumber;
            })
            .AddRedisCaching(options =>
            {
                options.RedisConnectionMultiplexer = connectionMultiplexer;
                options.KeyPrefix = IdentityUserCache.Configuration;
                options.Db = cacheSettingsOptions.DbNumber;
            })
            .AddClientStoreCache<ClientStore>()
            .AddResourceStoreCache<ResourceStore>()
            .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>()
            .AddAspNetIdentity<User>()
            .AddProfileService<ProfileService<>>();
        
        if (currentEnv is "localhost")
        {
           identityBuilder.AddDeveloperSigningCredential();
        }
        else
        { 
            var (signingKey, algorithm) = _configuration.GetAzureKeyVaultInfo();
            identityBuilder.AddSigningCredential(signingKey, algorithm);
            services.AddSingleton<CryptographyClientFactory>();
            services.AddTransient<ITokenCreationService, KeyVaultTokenCreationService>();
        }

        services.AddAuthorization();
        services.AddScoped<IUnitOfRepository, UnitOfRepository>();
        services.AddTransient<IProfileService, ProfileService>();
        services.AddTransient<IResourceOwnerPasswordValidator, CustomResourceOwnerPasswordValidator>();
        
        var authorityUrl = _configuration["AuthServer:AuthUrl"];
        var authSecret = _configuration["AuthServer:Secret"];
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
    }
    
}