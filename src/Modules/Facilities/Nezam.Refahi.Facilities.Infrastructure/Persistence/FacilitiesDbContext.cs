using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;
using Nezam.Refahi.Shared.Infrastructure;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence;

/// <summary>
/// DbContext for Facilities bounded context
/// </summary>
public class FacilitiesDbContext : DbContext
{
    public FacilitiesDbContext(DbContextOptions<FacilitiesDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Facility> Facilities { get; set; } = null!;
    public DbSet<FacilityCycle> FacilityCycles { get; set; } = null!;
    public DbSet<FacilityCycleDependency> FacilityCycleDependencies { get; set; } = null!;
    public DbSet<FacilityRequest> FacilityRequests { get; set; } = null!;
    public DbSet<FacilityFeature> FacilityFeatures { get; set; } = null!;
    public DbSet<FacilityCapability> FacilityCapabilityPolicies { get; set; } = null!;
    public DbSet<FacilityRejection> FacilityRejections { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new FacilityConfiguration());
        modelBuilder.ApplyConfiguration(new FacilityCycleConfiguration());
        modelBuilder.ApplyConfiguration(new FacilityCycleDependencyConfiguration());
        modelBuilder.ApplyConfiguration(new FacilityRequestConfiguration());
        modelBuilder.ApplyConfiguration(new FacilityFeatureConfiguration());
        modelBuilder.ApplyConfiguration(new FacilityCapabilityConfiguration());
        modelBuilder.ApplyConfiguration(new FacilityRejectionConfiguration());

        // Configure schema
        modelBuilder.HasDefaultSchema("facilities");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        // Enable sensitive data logging in development
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
