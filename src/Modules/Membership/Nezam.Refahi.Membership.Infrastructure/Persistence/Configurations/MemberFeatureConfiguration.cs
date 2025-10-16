using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for MemberFeature entity
/// </summary>
public class MemberFeatureConfiguration : IEntityTypeConfiguration<MemberFeature>
{
    public void Configure(EntityTypeBuilder<MemberFeature> builder)
    {
        // Table name and schema
        builder.ToTable("MemberFeatures", "membership");

        // Primary key
        builder.HasKey(mf => mf.Id);

        // Properties
        builder.Property(mf => mf.Id)
            .ValueGeneratedOnAdd();

        builder.Property(mf => mf.MemberId)
            .IsRequired();

        builder.Property(mf => mf.FeatureKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mf => mf.FeatureTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(mf => mf.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(mf => mf.ValidFrom)
            .IsRequired(false);

        builder.Property(mf => mf.ValidTo)
            .IsRequired(false);

        builder.Property(mf => mf.AssignedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(mf => mf.AssignedAt)
            .IsRequired();

        builder.Property(mf => mf.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(mf => mf.MemberId)
            .HasDatabaseName("IX_MemberFeatures_MemberId");

        builder.HasIndex(mf => mf.FeatureKey)
            .HasDatabaseName("IX_MemberFeatures_FeatureKey");

        builder.HasIndex(mf => new { mf.MemberId, mf.FeatureKey })
            .HasDatabaseName("IX_MemberFeatures_MemberId_FeatureKey")
            .IsUnique();

        builder.HasIndex(mf => mf.IsActive)
            .HasDatabaseName("IX_MemberFeatures_IsActive");

        builder.HasIndex(mf => mf.ValidFrom)
            .HasDatabaseName("IX_MemberFeatures_ValidFrom");

        builder.HasIndex(mf => mf.ValidTo)
            .HasDatabaseName("IX_MemberFeatures_ValidTo");

        builder.HasIndex(mf => mf.AssignedBy)
            .HasDatabaseName("IX_MemberFeatures_AssignedBy");

        builder.HasIndex(mf => mf.AssignedAt)
            .HasDatabaseName("IX_MemberFeatures_AssignedAt");

        // Relationships
        builder.HasOne(mf => mf.Member)
            .WithMany(m => m.Features)
            .HasForeignKey(mf => mf.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        // Value conversions
        builder.Property(mf => mf.FeatureKey)
            .HasConversion(
                v => v,
                v => v);

        builder.Property(mf => mf.FeatureTitle)
            .HasConversion(
                v => v,
                v => v);

        // Query filters for soft delete (if needed)
        // builder.HasQueryFilter(mf => !mf.IsDeleted);
    }
}
