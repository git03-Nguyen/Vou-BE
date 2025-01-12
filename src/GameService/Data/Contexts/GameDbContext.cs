using GameService.Data.Models;
using GameService.Data.Models.SyncModels;
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
    
    public DbSet<PlayerQuizSession> PlayerQuizSessions { get; set; }
    public DbSet<PlayerShakeSession> PlayerShakeSessions { get; set; }
    
    // Sync tables
    public DbSet<Player> Players { get; set; }
    public DbSet<QuizSession> QuizSessions { get; set; }
    public DbSet<QuizSet> QuizSets { get; set; }
    
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
        
        // Player
        builder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
            
        // QuizSession - QuizSet: one - one
        builder.Entity<QuizSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<QuizSet>()
                .WithOne()
                .HasForeignKey<QuizSession>(e => e.QuizSetId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // QuizSet
        builder.Entity<QuizSet>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        
        builder.Entity<PlayerQuizSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // PlayerQuizSession - Player: one - many
            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // PlayerQuizSession - QuizSession: one - many
            entity.HasOne<QuizSession>()
                .WithMany()
                .HasForeignKey(e => e.QuizSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        builder.Entity<PlayerShakeSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // PlayerShakeSession - Player: one - many
            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
    }
}