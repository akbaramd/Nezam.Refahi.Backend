using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence.Configurations;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence;

/// <summary>
/// DbContext for BasicDefinitions module
/// </summary>
public class BasicDefinitionsDbContext : DbContext
{
    public BasicDefinitionsDbContext(DbContextOptions<BasicDefinitionsDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Representative offices
    /// </summary>
    public DbSet<RepresentativeOffice> RepresentativeOffices { get; set; } = null!;

    /// <summary>
    /// Features catalog
    /// </summary>
    public DbSet<Features> Features { get; set; } = null!;

    /// <summary>
    /// Capabilities catalog
    /// </summary>
    public DbSet<Capability> Capabilities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new RepresentativeOfficeConfiguration());
        modelBuilder.ApplyConfiguration(new FeaturesConfiguration());
        modelBuilder.ApplyConfiguration(new CapabilityConfiguration());

        // Set schema
        modelBuilder.HasDefaultSchema("definitions");
    }
}
