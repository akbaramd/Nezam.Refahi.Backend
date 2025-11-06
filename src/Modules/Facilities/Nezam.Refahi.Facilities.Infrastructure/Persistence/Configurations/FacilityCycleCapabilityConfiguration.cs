using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for FacilityCycleCapability entity
/// </summary>
public class FacilityCycleCapabilityConfiguration : IEntityTypeConfiguration<FacilityCycleCapability>
{
    public void Configure(EntityTypeBuilder<FacilityCycleCapability> builder)
    {
        builder.ToTable("FacilityCycleCapabilities");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.FacilityCycleId)
            .IsRequired();

        builder.Property(c => c.CapabilityId)
            .IsRequired()
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(c => c.FacilityCycleId);
        builder.HasIndex(c => c.CapabilityId);
        builder.HasIndex(c => new { c.FacilityCycleId, c.CapabilityId }).IsUnique();

        // Relationships
        builder.HasOne(c => c.FacilityCycle)
            .WithMany(cycle => cycle.Capabilities)
            .HasForeignKey(c => c.FacilityCycleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

