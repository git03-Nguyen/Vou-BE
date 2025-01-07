using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Middlewares;

namespace Shared.StartupRegistrations;

public static class ExceptionHandlerRegistrations
{
    public static IServiceCollection ConfigureExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<ExceptionHandlerMiddleware>();
        services.AddProblemDetails();
        return services;
    }
    
    public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        app.UseExceptionHandler();
        return app;
    }
}