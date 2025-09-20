using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for TourRestrictedTour
/// </summary>
public class TourRestrictedTourConfiguration : IEntityTypeConfiguration<TourRestrictedTour>
{
    public void Configure(EntityTypeBuilder<TourRestrictedTour> builder)
    {
        builder.ToTable("TourRestrictedTours", "recreation");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(x => x.TourId)
            .IsRequired();

        builder.Property(x => x.RestrictedTourId)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Tour)
            .WithMany() // No navigation property on Tour side for this relationship
            .HasForeignKey(x => x.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RestrictedTour)
            .WithMany() // No navigation property on Tour side for this relationship
            .HasForeignKey(x => x.RestrictedTourId)
            .OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete cycles

        // Indexes
        builder.HasIndex(x => x.TourId)
            .HasDatabaseName("IX_TourRestrictedTours_TourId");

        builder.HasIndex(x => x.RestrictedTourId)
            .HasDatabaseName("IX_TourRestrictedTours_RestrictedTourId");

        // Unique constraint to prevent duplicate restrictions
        builder.HasIndex(x => new { x.TourId, x.RestrictedTourId })
            .IsUnique()
            .HasDatabaseName("IX_TourRestrictedTours_TourId_RestrictedTourId_Unique");

        // Check constraint to prevent self-referencing
    }
}