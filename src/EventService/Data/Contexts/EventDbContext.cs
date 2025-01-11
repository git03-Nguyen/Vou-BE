using EventService.Data.Models;
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
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<VoucherInEvent> VoucherInEvents { get; set; }
    public DbSet<VoucherToPlayer> VoucherToPlayers { get; set; }
    
    // Sync tables
    public DbSet<Event> CounterParts { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(_databaseOptions.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(_databaseOptions.DefaultSchema);

        // CounterPart - Event: one - many
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // CounterPart - Voucher: one - many
        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(v => v.Id);
        });
        
        modelBuilder.Entity<VoucherInEvent>(entity =>
        {
            entity.HasKey(ve => ve.Id);
            
            // Event - VoucherInEvent: one - many
            entity.HasOne<Event>()
                .WithMany()
                .HasForeignKey(ve => ve.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Voucher - VoucherInEvent: one - many
            entity.HasOne<Voucher>()
                .WithMany()
                .HasForeignKey(ve => ve.VoucherId)
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
        });
    }
}