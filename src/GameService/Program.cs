using Shared.StartupRegistrations;

namespace GameService;

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
            .ConfigureControllers()
            .ConfigureCustomHttpContext()
            .ConfigureDIServices(builder.Configuration)
            .ConfigureMediatRService(typeof(Program).Assembly)
            .ConfigureFluentValidation(typeof(Program).Assembly)
            .ConfigureSwagger(builder.Environment)
            .ConfigureExceptionHandler();

        // Configure the HTTP request pipeline.
        var app = builder.Build();
        app.UseSwaggerService(app.Environment)
            .UseHttpsRedirection()
            .UseRouting()
            .UseExceptionHandler()
            .UseAuthentication(builder.Configuration)
            .UseControllers();

        app.Run();
    }
}