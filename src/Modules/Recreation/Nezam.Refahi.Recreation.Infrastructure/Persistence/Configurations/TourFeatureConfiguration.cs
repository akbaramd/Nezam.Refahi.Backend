using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class TourFeatureConfiguration : IEntityTypeConfiguration<TourFeature>
{
    public void Configure(EntityTypeBuilder<TourFeature> builder)
    {
        builder.ToTable("TourFeatures", "recreation");

        builder.HasKey(tf => tf.Id);
        builder.Property(tf => tf.Id)
            .ValueGeneratedNever();

        builder.Property(tf => tf.TourId)
            .IsRequired();

        builder.Property(tf => tf.FeatureId)
            .IsRequired();

        builder.Property(tf => tf.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(tf => tf.IsHighlighted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(tf => tf.AssignedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasOne(tf => tf.Tour)
            .WithMany(t => t.TourFeatures)
            .HasForeignKey(tf => tf.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tf => tf.Feature)
            .WithMany(f => f.TourFeatures)
            .HasForeignKey(tf => tf.FeatureId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tf => tf.TourId);
        builder.HasIndex(tf => tf.FeatureId);
        builder.HasIndex(tf => tf.IsHighlighted);
        builder.HasIndex(tf => tf.AssignedAt);
        builder.HasIndex(tf => new { tf.TourId, tf.FeatureId }).IsUnique();
    }
}