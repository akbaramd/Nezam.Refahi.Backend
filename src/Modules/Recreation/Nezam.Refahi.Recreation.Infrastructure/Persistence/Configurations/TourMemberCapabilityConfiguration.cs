using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for TourMemberCapability
/// </summary>
public class TourMemberCapabilityConfiguration : IEntityTypeConfiguration<TourMemberCapability>
{
    public void Configure(EntityTypeBuilder<TourMemberCapability> builder)
    {
        builder.ToTable("TourMemberCapabilities", "Recreation");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(x => x.TourId)
            .IsRequired();

        builder.Property(x => x.CapabilityId)
            .IsRequired()
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(x => x.Tour)
            .WithMany(t => t.MemberCapabilities)
            .HasForeignKey(x => x.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.TourId)
            .HasDatabaseName("IX_TourMemberCapabilities_TourId");

        builder.HasIndex(x => x.CapabilityId)
            .HasDatabaseName("IX_TourMemberCapabilities_CapabilityId");

        // Unique constraint to prevent duplicate capability requirements per tour
        builder.HasIndex(x => new { x.TourId, x.CapabilityId })
            .IsUnique()
            .HasDatabaseName("IX_TourMemberCapabilities_TourId_CapabilityId_Unique");
    }
}