using ApiGateway.Options;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway.StartupRegistrations;

public static class OcelotRegistrations
{
    public static IServiceCollection ConfigureOcelot(this IServiceCollection services,IConfigurationManager configuration, IWebHostEnvironment environment)
    {
        var ocelotOptions = configuration.GetSection(OcelotOptions.OptionName).Get<OcelotOptions>();
        configuration.AddOcelot
        (
            ocelotOptions?.Folder,
            environment,
            MergeOcelotJson.ToFile,
            optional: false,
            reloadOnChange: true
        );
        configuration.AddOcelotWithSwaggerSupport(options => { options.Folder = ocelotOptions?.Folder; });
        services
            .AddOcelot()
            .AddCacheManager(x => x.WithDictionaryHandle());
        services.AddSwaggerForOcelot(configuration);
        return services;
    }

    public static IServiceCollection AddSwaggerService(this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        if (!environment.IsProduction())
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        return services;
    }

    public static IApplicationBuilder UseOcelotService(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (!environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerForOcelotUI(options => { options.PathToSwaggerGenerator = "/swagger/docs"; });
        }

        app.UseOcelot();

        return app;
    }
}