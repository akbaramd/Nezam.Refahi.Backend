using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence;

/// <summary>
/// Database context for the Membership bounded context
/// All entity configurations are automatically applied from the Configurations folder
/// </summary>
public class MembershipDbContext(DbContextOptions<MembershipDbContext> options) : DbContext(options)
{
    // Membership bounded context
    public DbSet<Member> Members { get; set; } = default!;
    public DbSet<Capability> Capabilities { get; set; } = default!;
    public DbSet<MemberCapability> MemberCapabilities { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;
    public DbSet<MemberRole> MemberRoles { get; set; } = default!;
    public DbSet<Features> Features { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        // This automatically discovers and applies all IEntityTypeConfiguration<T> implementations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}