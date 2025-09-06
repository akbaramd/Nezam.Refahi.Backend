using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence;

/// <summary>
/// Main database context for the application
/// All entity configurations are automatically applied from the Configurations folder
/// </summary>
public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{

    // Identity bounded context
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<UserToken> UserTokens { get; set; } = default!;
    public DbSet<OtpChallenge> OtpChallenges { get; set; } = default!;
    public DbSet<RefreshSession> RefreshSessions { get; set; } = default!;
    public DbSet<UserPreference> UserPreferences { get; set; } = default!;
    
    // Role and Claims
    public DbSet<Role> Roles { get; set; } = default!;
    public DbSet<RoleClaim> RoleClaims { get; set; } = default!;
    public DbSet<UserRole> UserRoles { get; set; } = default!;
    public DbSet<UserClaim> UserClaims { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        // This automatically discovers and applies all IEntityTypeConfiguration<T> implementations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
