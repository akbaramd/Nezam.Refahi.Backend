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
            .ValueGeneratedNever(); // Client generates ID in constructor

        builder.Property(mf => mf.MemberId)
            .IsRequired();

        builder.Property(mf => mf.FeatureKey)
            .IsRequired()
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(mf => mf.MemberId)
            .HasDatabaseName("IX_MemberFeatures_MemberId");

        builder.HasIndex(mf => mf.FeatureKey)
            .HasDatabaseName("IX_MemberFeatures_FeatureKey");

        builder.HasIndex(mf => new { mf.MemberId, mf.FeatureKey })
            .IsUnique()
            .HasDatabaseName("IX_MemberFeatures_MemberId_FeatureKey");

        // Relationships
        builder.HasOne(mf => mf.Member)
            .WithMany(m => m.Features)
            .HasForeignKey(mf => mf.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
