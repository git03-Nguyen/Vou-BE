using BackgroundServiceJobs.StartupRegistrations;
using Shared.StartupRegistrations;

namespace BackgroundServiceJobs;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.ConfigureKestrel(serverOptions => 
        { {
            serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
            serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
        } });
        builder.Host.UseLogging();
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Add services to the container.
        builder.Services
            .ConfigureCustomOptions(builder.Configuration)
            .ConfigureDbContext(builder.Configuration)
            .ConfigureAuthentication(builder.Configuration)
            .ConfigureApiVersioning()
            .ConfigureDaprIntegration(builder.Configuration)
            .ConfigureBackgroundJobs(builder.Configuration)
            .ConfigureControllers()
            .ConfigureCustomHttpContext()
            .ConfigureDIServices(builder.Configuration)
            .ConfigureMediatRService(typeof(Program).Assembly)
            .ConfigureFluentValidation(typeof(Program).Assembly)
            .ConfigureSwagger(builder.Configuration)
            .ConfigureExceptionHandler();

        // Configure the HTTP request pipeline.
        var app = builder.Build();
        app.UseSwaggerService(app.Environment)
            .UseHttpsRedirection()
            .UseRouting()
            .UseExceptionHandler()
            .UseAuthentication(builder.Configuration)
            .UseDaprIntegration()
            .UseControllers()
            .UseBackgroundJobs();

        app.Run();
    }
}