using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shared.StartupRegistrations;

public static class DatabaseMigrationRegistrations
{
    private const int MaxMigrationAttempts = 30;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

    public static IServiceProvider ApplyPendingMigrations<TContext>(this IServiceProvider services)
        where TContext : DbContext
    {
        for (var attempt = 1; attempt <= MaxMigrationAttempts; attempt++)
        {
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                if (!dbContext.Database.CanConnect())
                {
                    if (attempt == MaxMigrationAttempts)
                    {
                        throw new InvalidOperationException($"Unable to connect to database for {typeof(TContext).Name} after {MaxMigrationAttempts} attempts.");
                    }

                    logger.LogWarning(
                        "Database for {DbContext} is not reachable yet. Retrying in {RetryDelaySeconds} seconds (attempt {Attempt}/{MaxAttempts}).",
                        typeof(TContext).Name,
                        RetryDelay.TotalSeconds,
                        attempt,
                        MaxMigrationAttempts);

                    Thread.Sleep(RetryDelay);
                    continue;
                }

                var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
                if (pendingMigrations.Count == 0)
                {
                    logger.LogInformation("No pending migrations found for {DbContext}.", typeof(TContext).Name);
                    return services;
                }

                dbContext.Database.Migrate();
                logger.LogInformation(
                    "Applied {MigrationCount} pending migrations for {DbContext}: {MigrationNames}",
                    pendingMigrations.Count,
                    typeof(TContext).Name,
                    string.Join(", ", pendingMigrations));

                return services;
            }
            catch (Exception ex) when (attempt < MaxMigrationAttempts)
            {
                logger.LogWarning(
                    ex,
                    "Failed to apply migrations for {DbContext}. Retrying in {RetryDelaySeconds} seconds (attempt {Attempt}/{MaxAttempts}).",
                    typeof(TContext).Name,
                    RetryDelay.TotalSeconds,
                    attempt,
                    MaxMigrationAttempts);

                Thread.Sleep(RetryDelay);
            }
        }

        throw new InvalidOperationException($"Failed to apply pending migrations for {typeof(TContext).Name} after {MaxMigrationAttempts} attempts.");
    }
}
