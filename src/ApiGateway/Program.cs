using ApiGateway.StartupRegistrations;
using Shared.Constants;
using Shared.StartupRegistrations;

namespace ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .ConfigureAuthentication(builder.Configuration)
            .ConfigureOcelot(builder.Configuration, builder.Environment)
            .ConfigureSwagger(builder.Environment)
            .ConfigureCors(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseCors(CommonConstants.CorsPolicyName);
        app.UseAuthentication();
        app.UseWebSockets();
        app.UseOcelot(app.Environment);
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.Run();
    }
}