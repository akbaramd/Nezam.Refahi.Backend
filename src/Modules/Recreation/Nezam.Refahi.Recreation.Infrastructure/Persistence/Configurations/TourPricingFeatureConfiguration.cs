using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class TourPricingFeatureConfiguration : IEntityTypeConfiguration<TourPricingFeature>
{
    public void Configure(EntityTypeBuilder<TourPricingFeature> builder)
    {
        builder.ToTable("TourPricingFeature", "recreation");

        builder.HasKey(tpf => tpf.Id);
        builder.Property(tpf => tpf.Id)
            .ValueGeneratedNever();

        builder.Property(tpf => tpf.TourPricingId)
            .IsRequired();

        builder.Property(tpf => tpf.FeatureId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(tpf => tpf.TourPricing)
            .WithMany(tp => tp.Features)
            .HasForeignKey(tpf => tpf.TourPricingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: one pricing cannot have the same feature twice
        builder.HasIndex(tpf => new { tpf.TourPricingId, tpf.FeatureId })
            .IsUnique();

        builder.HasIndex(tpf => tpf.FeatureId);
    }
}

