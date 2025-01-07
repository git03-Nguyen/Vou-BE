using AuthServer.Data.Models;
using AuthServer.Data.Seeds;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace AuthServer.Data.Contexts;

public class AuthDbContext : IdentityDbContext<User>
{
    private readonly DatabaseOptions _databaseOptions;
    public AuthDbContext(DbContextOptions<AuthDbContext> options, IOptions<DatabaseOptions> databaseOptions) : base(options)
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
        AuthDbContextSeeds.Seed(builder);
    }
}