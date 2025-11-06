using MCA.SharedKernel.Infrastructure.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("Tours", "recreation");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Difficulty)
          .IsRequired()
          .HasConversion<string>()
          .HasDefaultValue(TourDifficulty.Easy);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(t => t.TourStart)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.TourEnd)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(TourStatus.Draft);



        builder.Property(t => t.MinAge)
            .IsRequired(false);

        builder.Property(t => t.MaxAge)
            .IsRequired(false);

        builder.Property(t => t.IsActive)
          
            .IsRequired()
            .HasDefaultValue(true);

        // Note: FeatureIds and RestrictedTourIds are handled through navigation properties
        // FeatureIds through TourFeature junction entity
        // RestrictedTourIds should be implemented as a separate junction entity if needed

        builder.ConfigureFullAuditableEntity();

        // Participants are now managed through reservations

        builder.HasMany(t => t.Pricing)
            .WithOne(p => p.Tour)
            .HasForeignKey(p => p.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Photos)
            .WithOne(p => p.Tour)
            .HasForeignKey(p => p.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure TourFeature relationship
        builder.HasMany(t => t.TourFeatures)
            .WithOne(tf => tf.Tour)
            .HasForeignKey(tf => tf.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure TourMemberCapability relationship
        builder.HasMany(t => t.MemberCapabilities)
            .WithOne(mc => mc.Tour)
            .HasForeignKey(mc => mc.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure TourMemberFeature relationship
        builder.HasMany(t => t.MemberFeatures)
            .WithOne(mf => mf.Tour)
            .HasForeignKey(mf => mf.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure TourRestrictedTour relationship
        builder.HasMany(t => t.TourRestrictedTours)
            .WithOne(trt => trt.Tour)
            .HasForeignKey(trt => trt.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure TourReservation relationship
        builder.HasMany(t => t.Reservations)
            .WithOne(r => r.Tour)
            .HasForeignKey(r => r.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure TourCapacity relationship
        builder.HasMany(t => t.Capacities)
            .WithOne(tc => tc.Tour)
            .HasForeignKey(tc => tc.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.TourStart);
        builder.HasIndex(t => t.IsActive);
        builder.HasIndex(t => new { t.TourStart, t.TourEnd });
    }
}