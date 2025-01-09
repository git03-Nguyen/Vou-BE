using GameService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace GameService.Data.Contexts;

public class GameDbContext : DbContext
{
    private readonly DatabaseOptions _databaseOptions;
    public DbSet<Game> Events { get; set; }
    public DbSet<GameSession> Vouchers { get; set; }
    public GameDbContext(DbContextOptions<GameDbContext> options, IOptions<DatabaseOptions> databaseOptions) : base(options)
    {
        _databaseOptions = databaseOptions.Value;
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
        builder.UseNpgsql(_databaseOptions.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(_databaseOptions.DefaultSchema);

        builder.Entity<Game>()
            .HasKey(x => x.Id);
        
        builder.Entity<Voucher>()
            .HasKey(x => x.Id);
    }
}