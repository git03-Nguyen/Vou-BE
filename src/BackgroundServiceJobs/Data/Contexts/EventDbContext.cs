using BackgroundServiceJobs.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace BackgroundServiceJobs.Data.Contexts;

public class EventDbContext : DbContext
{
    private readonly DatabaseOptions _databaseOptions;
    public EventDbContext(DbContextOptions<EventDbContext> options, IOptions<DatabaseOptions> databaseOptions) : base(options)
    {
        _databaseOptions = databaseOptions.Value;
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }
    
    public DbSet<Event> Events { get; set; }
    public DbSet<FavoriteEvent> FavoriteEvents { get; set; }
    public DbSet<QuizSession> QuizSessions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(_databaseOptions.ConnectionString);
        optionsBuilder.EnableSensitiveDataLogging(false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(_databaseOptions.DefaultSchema);
    }
}