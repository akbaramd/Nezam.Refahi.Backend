using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class ReservationPriceSnapshotConfiguration : IEntityTypeConfiguration<ReservationPriceSnapshot>
{
    public void Configure(EntityTypeBuilder<ReservationPriceSnapshot> builder)
    {
        builder.ToTable("ReservationPriceSnapshots", "recreation", t =>
        {
            t.HasCheckConstraint("CK_ReservationPriceSnapshots_BasePricePositive", "[BasePriceRials] >= 0");
            t.HasCheckConstraint("CK_ReservationPriceSnapshots_FinalPricePositive", "[FinalPriceRials] >= 0");
            t.HasCheckConstraint("CK_ReservationPriceSnapshots_DiscountValid", "[DiscountAmountRials] IS NULL OR [DiscountAmountRials] >= 0");
        });

        builder.HasKey(ps => ps.Id);
        builder.Property(ps => ps.Id)
            .ValueGeneratedNever();

        builder.Property(ps => ps.ReservationId)
            .IsRequired();

        builder.Property(ps => ps.ParticipantType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ps => ps.TourPricingId)
            .IsRequired(false);

        // Configure Money value objects
        builder.OwnsOne(ps => ps.BasePrice, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("BasePriceRials")
                .HasColumnType("bigint")
                .IsRequired();
            // No need for WithOwner() - EF Core automatically sets ownership
        });

        builder.OwnsOne(ps => ps.FinalPrice, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("FinalPriceRials")
                .HasColumnType("bigint")
                .IsRequired();
            // No need for WithOwner() - EF Core automatically sets ownership
        });

        builder.OwnsOne(ps => ps.DiscountAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("DiscountAmountRials")
                .HasColumnType("bigint");
            // No need for WithOwner() - EF Core automatically sets ownership
        });

        builder.Property(ps => ps.DiscountCode)
            .HasMaxLength(50);

        builder.Property(ps => ps.DiscountDescription)
            .HasMaxLength(200);

        builder.Property(ps => ps.PricingRules)
            .HasColumnType("nvarchar(max)"); // JSON

        builder.Property(ps => ps.RequiredCapabilityIds)
            .HasColumnType("nvarchar(max)") // JSON array
            .IsRequired(false);

        builder.Property(ps => ps.RequiredFeatureIds)
            .HasColumnType("nvarchar(max)") // JSON array
            .IsRequired(false);

        builder.Property(ps => ps.WasDefaultPricing)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ps => ps.AppliedDiscountPercentage)
            .HasColumnType("decimal(5,2)")
            .IsRequired(false);

        builder.Property(ps => ps.WasEarlyBird)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ps => ps.WasLastMinute)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ps => ps.SnapshotDate)
            .IsRequired()
            .HasColumnType("datetime2");

        // Multi-tenancy support
        builder.Property(ps => ps.TenantId)
            .IsRequired(false)
            .HasMaxLength(50);

        // Configure relationship with TourReservation
        // IMPORTANT: Navigation property must be explicitly configured to ensure proper tracking
        builder.HasOne(ps => ps.Reservation)
            .WithMany(r => r.PriceSnapshots)
            .HasForeignKey(ps => ps.ReservationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Unique constraint: One snapshot per reservation per participant type
        builder.HasIndex(ps => new { ps.TenantId, ps.ReservationId, ps.ParticipantType })
            .HasDatabaseName("UX_ReservationPriceSnapshots_TenantReservationParticipantType")
            .IsUnique();

        // Performance indexes
        builder.HasIndex(ps => new { ps.TenantId, ps.ReservationId })
            .HasDatabaseName("IX_ReservationPriceSnapshots_TenantReservation");

        builder.HasIndex(ps => ps.SnapshotDate)
            .HasDatabaseName("IX_ReservationPriceSnapshots_SnapshotDate");

        builder.HasIndex(ps => ps.DiscountCode)
            .HasDatabaseName("IX_ReservationPriceSnapshots_DiscountCode")
            .HasFilter("[DiscountCode] IS NOT NULL");

        builder.HasIndex(ps => ps.TourPricingId)
            .HasDatabaseName("IX_ReservationPriceSnapshots_TourPricingId")
            .HasFilter("[TourPricingId] IS NOT NULL");
    }
}
