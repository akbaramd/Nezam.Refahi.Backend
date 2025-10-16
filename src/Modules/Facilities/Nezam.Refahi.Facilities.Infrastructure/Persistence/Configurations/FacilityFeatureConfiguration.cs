using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for FacilityFeature entity
/// </summary>
public class FacilityFeatureConfiguration : IEntityTypeConfiguration<FacilityFeature>
{
    public void Configure(EntityTypeBuilder<FacilityFeature> builder)
    {
        builder.ToTable("FacilityFeatures");

        // Primary key
        builder.HasKey(f => f.Id);

        // Properties
        builder.Property(f => f.FacilityId)
            .IsRequired();

        builder.Property(f => f.FeatureId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.RequirementType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(f => f.Notes)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(f => f.FacilityId);
        builder.HasIndex(f => f.FeatureId);
        builder.HasIndex(f => f.RequirementType);
        builder.HasIndex(f => new { f.FacilityId, f.FeatureId }).IsUnique();

        // Relationships
        builder.HasOne<Facility>()
            .WithMany(f => f.Features)
            .HasForeignKey(f => f.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
