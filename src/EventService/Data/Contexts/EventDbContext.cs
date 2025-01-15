using EventService.Data.Models;
using EventService.Data.Models.SyncModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace EventService.Data.Contexts;

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
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<VoucherToPlayer> VoucherToPlayers { get; set; }
    public DbSet<QuizSession> QuizSessions { get; set; }
    public DbSet<QuizSet> QuizSets { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    // Sync tables
    public DbSet<CounterPart> CounterParts { get; set; }
    public DbSet<Player> Players { get; set; }
    
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

        // CounterPart - Event: one - many
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<CounterPart>()
                .WithMany()
                .HasForeignKey(e => e.CounterPartId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Player - FavoriteEvent: one - many
        modelBuilder.Entity<FavoriteEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne<Event>()
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CounterPart - Voucher: one - many
        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.HasOne<CounterPart>()
                .WithMany()
                .HasForeignKey(v => v.CounterPartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Voucher - VoucherToPlayer: one - many
        modelBuilder.Entity<VoucherToPlayer>(entity =>
        {
            entity.HasKey(vp => vp.Id);
            entity.HasOne<Voucher>()
                .WithMany()
                .HasForeignKey(vp => vp.VoucherId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Player - VoucherToPlayer: one - many
            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(vp => vp.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<QuizSession>(entity =>
        {
            entity.HasKey(qs => qs.Id);
            
            // QuizSet - QuizSession: one - many
            entity.HasOne<QuizSet>()
                .WithMany()
                .HasForeignKey(qs => qs.QuizSetId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Voucher - QuizSession: one - many
            entity.HasOne<Voucher>()
                .WithMany()
                .HasForeignKey(qs => qs.VoucherId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Event - QuizSession: one - many
            entity.HasOne<Event>()
                .WithMany()
                .HasForeignKey(qs => qs.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // QuizSet - QuizSession
        modelBuilder.Entity<QuizSet>(entity =>
        {
            entity.HasKey(qs => qs.Id);
            
            // CounterPart - QuizSet: one - many
            entity.HasOne<CounterPart>()
                .WithMany()
                .HasForeignKey(qs => qs.CounterPartId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Player - Notification: one - many
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(n => n.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}