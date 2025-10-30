using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class TourPricingCapabilityConfiguration : IEntityTypeConfiguration<TourPricingCapability>
{
    public void Configure(EntityTypeBuilder<TourPricingCapability> builder)
    {
        builder.ToTable("TourPricingCapability", "recreation");

        builder.HasKey(tpc => tpc.Id);
        builder.Property(tpc => tpc.Id)
            .ValueGeneratedNever();

        builder.Property(tpc => tpc.TourPricingId)
            .IsRequired();

        builder.Property(tpc => tpc.CapabilityId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(tpc => tpc.TourPricing)
            .WithMany(tp => tp.Capabilities)
            .HasForeignKey(tpc => tpc.TourPricingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: one pricing cannot have the same capability twice
        builder.HasIndex(tpc => new { tpc.TourPricingId, tpc.CapabilityId })
            .IsUnique();

        builder.HasIndex(tpc => tpc.CapabilityId);
    }
}

