using System.Net;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Shared.Extensions;
using Shared.StartupRegistrations;

namespace EventService.StartupRegistrations;

public static class ControllersRegistrations
{
    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        services.ConfigureDefaultValidationResponse();
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
    
    public static IApplicationBuilder UseControllers(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context => await context.Response.WriteAsync("OK!"));
        });
        return app;
    }
}