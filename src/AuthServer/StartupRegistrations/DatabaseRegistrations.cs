using AuthServer.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.StartupRegistrations;

public static class DatabaseRegistrations
{
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>();
        
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        try
        {
            while (!dbContext.Database.CanConnect())
            {
                Console.WriteLine("Waiting for database connection...");
                Thread.Sleep(1000);
            }

            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Count != 0)
            {
                dbContext.Database.Migrate();
                Console.WriteLine($"Migrations applied successfully: {string.Join(", ", pendingMigrations)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying migrations: {ex.Message}");
        }
        
        return services;
    }
}