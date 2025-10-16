using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for FacilityCapabilityPolicy entity
/// </summary>
public class FacilityCapabilityPolicyConfiguration : IEntityTypeConfiguration<FacilityCapabilityPolicy>
{
    public void Configure(EntityTypeBuilder<FacilityCapabilityPolicy> builder)
    {
        builder.ToTable("FacilityCapabilityPolicies");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.FacilityId)
            .IsRequired();

        builder.Property(p => p.CapabilityId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PolicyType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.ModifierValue)
            .HasPrecision(18, 4);

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(p => p.FacilityId);
        builder.HasIndex(p => p.CapabilityId);
        builder.HasIndex(p => p.PolicyType);
        builder.HasIndex(p => new { p.FacilityId, p.CapabilityId }).IsUnique();

        // Relationships
        builder.HasOne<Facility>()
            .WithMany(f => f.CapabilityPolicies)
            .HasForeignKey(p => p.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
