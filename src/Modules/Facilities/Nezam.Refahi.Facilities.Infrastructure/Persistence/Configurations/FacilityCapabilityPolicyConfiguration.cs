using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for FacilityCapabilityPolicy entity
/// </summary>
public class FacilityCapabilityConfiguration : IEntityTypeConfiguration<FacilityCapability>
{
    public void Configure(EntityTypeBuilder<FacilityCapability> builder)
    {
        builder.ToTable("FacilityCapability");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.FacilityId)
            .IsRequired();

        builder.Property(p => p.CapabilityId)
            .IsRequired()
            .HasMaxLength(100);


        // Indexes
        builder.HasIndex(p => p.FacilityId);
        builder.HasIndex(p => p.CapabilityId);
        builder.HasIndex(p => new { p.FacilityId, p.CapabilityId }).IsUnique();

        // Relationships
        builder.HasOne<Facility>()
            .WithMany(f => f.CapabilityPolicies)
            .HasForeignKey(p => p.FacilityId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
