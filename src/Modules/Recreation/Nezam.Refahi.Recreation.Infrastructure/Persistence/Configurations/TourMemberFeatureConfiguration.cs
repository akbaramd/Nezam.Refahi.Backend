using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for TourMemberFeature
/// </summary>
public class TourMemberFeatureConfiguration : IEntityTypeConfiguration<TourMemberFeature>
{
    public void Configure(EntityTypeBuilder<TourMemberFeature> builder)
    {
        builder.ToTable("TourMemberFeatures", "Recreation");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(x => x.TourId)
            .IsRequired();

        builder.Property(x => x.FeatureId)
            .IsRequired()
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(x => x.Tour)
            .WithMany(t => t.MemberFeatures)
            .HasForeignKey(x => x.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.TourId)
            .HasDatabaseName("IX_TourMemberFeatures_TourId");

        builder.HasIndex(x => x.FeatureId)
            .HasDatabaseName("IX_TourMemberFeatures_FeatureId");

        // Unique constraint to prevent duplicate feature requirements per tour
        builder.HasIndex(x => new { x.TourId, x.FeatureId })
            .IsUnique()
            .HasDatabaseName("IX_TourMemberFeatures_TourId_FeatureId_Unique");
    }
}