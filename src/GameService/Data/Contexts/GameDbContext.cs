using GameService.Data.Models;
using GameService.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace GameService.Data.Contexts;

public class GameDbContext : DbContext
{
    private readonly DatabaseOptions _databaseOptions;
    public GameDbContext(DbContextOptions<GameDbContext> options, IOptions<DatabaseOptions> databaseOptions) : base(options)
    {
        _databaseOptions = databaseOptions.Value;
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }
    
    public DbSet<Game> Games { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<VoucherInGameSession> VoucherInGameSessions { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
        builder.UseNpgsql(_databaseOptions.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(_databaseOptions.DefaultSchema);
        GameDbContextSeeds.Seed(builder);
        
        builder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            
        });
        
        builder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Game>()
                .WithMany()
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        builder.Entity<VoucherInGameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<GameSession>()
                .WithMany()
                .HasForeignKey(e => e.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        
    }
}