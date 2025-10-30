using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class TourPricingConfiguration : IEntityTypeConfiguration<TourPricing>
{
    public void Configure(EntityTypeBuilder<TourPricing> builder)
    {
        builder.ToTable("TourPricing", "recreation");

        builder.HasKey(tp => tp.Id);
        builder.Property(tp => tp.Id)
            .ValueGeneratedNever();

        builder.Property(tp => tp.TourId)
            .IsRequired();

        builder.Property(tp => tp.ParticipantType)
            .IsRequired()
            .HasConversion<int>();

        builder.OwnsOne(tp => tp.BasePrice, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("PriceRials")
                .IsRequired();
            money.WithOwner();
        });

        builder.Property(tp => tp.DiscountPercentage)
            .HasColumnType("decimal(5,2)")
            .IsRequired(false);

        builder.Property(tp => tp.ValidFrom)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(tp => tp.ValidTo)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(tp => tp.MinQuantity)
            .IsRequired(false);

        builder.Property(tp => tp.MaxQuantity)
            .IsRequired(false);

        builder.Property(tp => tp.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(tp => tp.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(tp => tp.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(tp => tp.Tour)
            .WithMany(t => t.Pricing)
            .HasForeignKey(tp => tp.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure many-to-many relationships
        builder.HasMany(tp => tp.Capabilities)
            .WithOne(tpc => tpc.TourPricing)
            .HasForeignKey(tpc => tpc.TourPricingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(tp => tp.Features)
            .WithOne(tpf => tpf.TourPricing)
            .HasForeignKey(tpf => tpf.TourPricingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tp => tp.TourId);
        builder.HasIndex(tp => tp.ParticipantType);
        builder.HasIndex(tp => new { tp.TourId, tp.ParticipantType });
        builder.HasIndex(tp => new { tp.TourId, tp.ParticipantType, tp.IsDefault });
        builder.HasIndex(tp => tp.ValidFrom);
        builder.HasIndex(tp => tp.ValidTo);
        builder.HasIndex(tp => tp.IsActive);
    }
}