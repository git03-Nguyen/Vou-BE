using ApiGateway.Options;
using Microsoft.OpenApi.Models;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;

namespace ApiGateway.StartupRegistrations;

public static class OcelotRegistrations
{
    public static ConfigurationManager AddOcelotConfiguration(this ConfigurationManager configuration)
    {
        var currentEnv = configuration.GetSection("ASPNETCORE_ENVIRONMENT").Value;
        var ocelotOptions = configuration.GetSection(OcelotOptions.OptionName).Get<OcelotOptions>();
        var path = ocelotOptions?.Path ?? "Routes";

        if (string.Equals(currentEnv, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            var environments = new[] { "localhost", "docker" };
            foreach (var environment in environments)
            {
                configuration.AddOcelotWithSwaggerSupport(options =>
                {
                    options.Folder = Path.Combine(path, environment);
                    options.PrimaryOcelotConfigFileName = $"ocelot.{environment.ToLower()}.json";
                });
            }
        }
        
        configuration.AddJsonFile($"ocelot.{currentEnv?.ToLower()}.json", optional: true, reloadOnChange: true);
        return configuration;
    }
    
    public static IServiceCollection ConfigureOcelot(this IServiceCollection services, IConfigurationManager configuration)
    {
        services
            .AddOcelot()
            .AddCacheManager(x => x.WithDictionaryHandle());
        services.AddEndpointsApiExplorer();
        services.AddSwaggerForOcelot(configuration);
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API Gateway Document",
                Version = "v1",
                Description = " is the api detailed description for the project",
                Contact = new OpenApiContact
                {
                    Name = "Author",
                    Email = "nguyendinhanhvlqt@gmail.com"
                }
            });
        });
        
        return services;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (!environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerForOcelotUI();
        }

        return app;
    }
}