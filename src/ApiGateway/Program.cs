using ApiGateway.StartupRegistrations;
using Ocelot.Middleware;
using Shared.StartupRegistrations;

namespace ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseLogging();
        builder.Configuration
            .AddOcelotConfiguration()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        // Add services to the container.
        builder.Services
            .ConfigureAuthentication(builder.Configuration)
            .ConfigureOcelot(builder.Configuration)
            .ConfigureCors(builder.Configuration);
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger(app.Environment);
        app.UseCustomCors();
        app.UseAuthentication();
        app.UseWebSockets();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseOcelot().Wait();
        app.Run();
    }
}