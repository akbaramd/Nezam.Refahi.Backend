using MCA.SharedKernel.Infrastructure.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class TourReservationConfiguration : IEntityTypeConfiguration<TourReservation>
{
    public void Configure(EntityTypeBuilder<TourReservation> builder)
    {
        builder.ToTable("TourReservations", "recreation", t =>
        {
            t.HasCheckConstraint("CK_TourReservations_ExpiryAfterReservation", "[ExpiryDate] IS NULL OR [ExpiryDate] > [ReservationDate]");
            t.HasCheckConstraint("CK_TourReservations_ConfirmationAfterReservation", "[ConfirmationDate] IS NULL OR [ConfirmationDate] >= [ReservationDate]");
            t.HasCheckConstraint("CK_TourReservations_CancellationAfterReservation", "[CancellationDate] IS NULL OR [CancellationDate] >= [ReservationDate]");
        });

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.TourId)
            .IsRequired();

        builder.Property(r => r.TrackingCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ReservationStatus.Draft);

        builder.Property(r => r.ReservationDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(r => r.ExpiryDate)
            .HasColumnType("datetime2");

        builder.Property(r => r.ConfirmationDate)
            .HasColumnType("datetime2");

        builder.Property(r => r.CancellationDate)
            .HasColumnType("datetime2");

        // Configure Money value objects
        builder.OwnsOne(r => r.TotalAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("TotalAmountRials")
                .HasColumnType("bigint");
        });

        builder.OwnsOne(r => r.PaidAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("PaidAmountRials")
                .HasColumnType("bigint");
        });

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.Property(r => r.CancellationReason)
            .HasMaxLength(500);

        builder.Property(r => r.BillId)
            .IsRequired(false);

        builder.Property(r => r.CapacityId)
            .IsRequired(false);

        builder.Property(r => r.MemberId)
            .IsRequired(false);

        builder.Property(r => r.ExternalUserId)
            .IsRequired();

        // Multi-tenancy support
        builder.Property(r => r.TenantId)
            .IsRequired(false)
            .HasMaxLength(50);

        // Audit fields
        builder.ConfigureFullAuditableEntity();

        // Configure relationship with Tour
        builder.HasOne(r => r.Tour)
            .WithMany(t => t.Reservations)
            .HasForeignKey(r => r.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with TourCapacity
        builder.HasOne(r => r.Capacity)
            .WithMany()
            .HasForeignKey(r => r.CapacityId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure relationship with Participants
        builder.HasMany(r => r.Participants)
            .WithOne(p => p.Reservation)
            .HasForeignKey(p => p.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with PriceSnapshots
        builder.HasMany(r => r.PriceSnapshots)
            .WithOne(ps => ps.Reservation)
            .HasForeignKey(ps => ps.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraints with tenant support
        builder.HasIndex(r => new { r.TenantId, r.TrackingCode })
            .HasDatabaseName("UX_TourReservations_TenantTrackingCode")
            .IsUnique();

        // Unique constraint: One main participant per tour per tenant (for active reservations)
        builder.HasIndex(r => new { r.TenantId, r.TourId, r.MemberId })
            .HasDatabaseName("UX_TourReservations_TenantTourMember")
            .IsUnique()
            .HasFilter("[Status] IN (1, 2, 3)"); // Held, Paying, Confirmed

        // Performance indexes
        builder.HasIndex(r => new { r.TenantId, r.TourId, r.Status })
            .HasDatabaseName("IX_TourReservations_TenantTourStatus");

        builder.HasIndex(r => new { r.TenantId, r.Status, r.ExpiryDate })
            .HasDatabaseName("IX_TourReservations_TenantStatusExpiry");

        builder.HasIndex(r => new { r.TenantId, r.CapacityId, r.Status })
            .HasDatabaseName("IX_TourReservations_TenantCapacityStatus");

        builder.HasIndex(r => new { r.TenantId, r.MemberId, r.ReservationDate })
            .HasDatabaseName("IX_TourReservations_TenantMemberDate");

        builder.HasIndex(r => new { r.TenantId, r.ExternalUserId, r.ReservationDate })
            .HasDatabaseName("IX_TourReservations_TenantExternalUserDate");

        builder.HasIndex(r => r.ReservationDate)
            .HasDatabaseName("IX_TourReservations_ReservationDate");

        builder.HasIndex(r => r.ExpiryDate)
            .HasDatabaseName("IX_TourReservations_ExpiryDate")
            .HasFilter("[ExpiryDate] IS NOT NULL");
    }
}