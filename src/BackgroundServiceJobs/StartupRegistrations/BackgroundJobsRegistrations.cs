using Hangfire;
using Hangfire.PostgreSql;
using PaymentService.BackgroundJobs.EventJobs;
using PaymentService.Options;

namespace PaymentService.StartupRegistrations;

public static class BackgroundJobsRegistrations
{
    public static IServiceCollection ConfigureBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireOptions = configuration.GetSection(HangfireOptions.OptionName).Get<HangfireOptions>();
        services.AddHangfire(config =>
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                {
                    options.UseNpgsqlConnection(configuration.GetConnectionString(hangfireOptions.ConnectionString));
                }));
        services.AddHangfireServer();
        return services;
    }
    
    public static IApplicationBuilder UseBackgroundJobs(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard();
        RecurringJob.AddOrUpdate<EventStatusJob>(nameof(EventStatusJob),x => x.UpdateEventStatuses(), Cron.Minutely);
        return app;
    }
}