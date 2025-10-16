using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for TourAgency entity
/// </summary>
public class TourAgencyConfiguration : IEntityTypeConfiguration<TourAgency>
{
    public void Configure(EntityTypeBuilder<TourAgency> builder)
    {
        // Table configuration
        builder.ToTable("TourAgencies", "recreation");

        // Primary key
        builder.HasKey(ta => ta.Id);
        builder.Property(ta => ta.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(ta => ta.TourId)
            .IsRequired();

        builder.Property(ta => ta.AgencyId)
            .IsRequired();

        builder.Property(ta => ta.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ta => ta.AgencyCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ta => ta.AgencyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ta => ta.ValidFrom);

        builder.Property(ta => ta.ValidTo);

        builder.Property(ta => ta.AssignedBy)
            .HasMaxLength(100);

        builder.Property(ta => ta.AssignedAt)
            .IsRequired();

        builder.Property(ta => ta.Notes)
            .HasMaxLength(1000);

        builder.Property(ta => ta.AccessLevel)
            .HasMaxLength(50);

        builder.Property(ta => ta.MaxReservations);

        builder.Property(ta => ta.MaxParticipants);

        // Relationships
        builder.HasOne(ta => ta.Tour)
            .WithMany(t => t.TourAgencies)
            .HasForeignKey(ta => ta.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Note: Agency relationship will be handled through BasicDefinitions context
        // We only store the AgencyId as a foreign key reference

        // Indexes
        builder.HasIndex(ta => ta.TourId);
        builder.HasIndex(ta => ta.AgencyId);
        builder.HasIndex(ta => ta.AgencyCode);
        builder.HasIndex(ta => new { ta.TourId, ta.AgencyId })
            .IsUnique(); // Ensure one agency assignment per tour
        builder.HasIndex(ta => ta.IsActive);
        builder.HasIndex(ta => ta.ValidFrom);
        builder.HasIndex(ta => ta.ValidTo);
    }
}
