using ApiGateway.StartupRegistrations;
using Shared.StartupRegistrations;

namespace ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseLogging();
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        // Add services to the container.
        builder.Services
            .ConfigureAuthentication(builder.Configuration)
            .ConfigureOcelot(builder.Configuration, builder.Environment)
            .ConfigureSwagger(builder.Environment)
            .ConfigureCors(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseCustomCors();
        app.UseAuthentication();
        app.UseWebSockets();
        app.UseOcelot(app.Environment);
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.Run();
    }
}