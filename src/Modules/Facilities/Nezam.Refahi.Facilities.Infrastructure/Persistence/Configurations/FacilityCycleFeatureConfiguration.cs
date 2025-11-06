using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for FacilityCycleFeature entity
/// </summary>
public class FacilityCycleFeatureConfiguration : IEntityTypeConfiguration<FacilityCycleFeature>
{
    public void Configure(EntityTypeBuilder<FacilityCycleFeature> builder)
    {
        builder.ToTable("FacilityCycleFeatures");

        // Primary key
        builder.HasKey(f => f.Id);

        // Properties
        builder.Property(f => f.FacilityCycleId)
            .IsRequired();

        builder.Property(f => f.FeatureId)
            .IsRequired()
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(f => f.FacilityCycleId);
        builder.HasIndex(f => f.FeatureId);
        builder.HasIndex(f => new { f.FacilityCycleId, f.FeatureId }).IsUnique();

        // Relationships
        builder.HasOne(f => f.FacilityCycle)
            .WithMany(c => c.Features)
            .HasForeignKey(f => f.FacilityCycleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

