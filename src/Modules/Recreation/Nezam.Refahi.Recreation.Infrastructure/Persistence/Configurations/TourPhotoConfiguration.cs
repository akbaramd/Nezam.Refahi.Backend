using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class TourPhotoConfiguration : IEntityTypeConfiguration<TourPhoto>
{
    public void Configure(EntityTypeBuilder<TourPhoto> builder)
    {
        builder.ToTable("TourPhotos", "recreation");

        builder.HasKey(tp => tp.Id);
        builder.Property(tp => tp.Id)
            .ValueGeneratedNever();

        builder.Property(tp => tp.TourId)
            .IsRequired();

        builder.Property(tp => tp.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(tp => tp.FileName)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(tp => tp.FilePath)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(tp => tp.Caption)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(tp => tp.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(tp => tp.Tour)
            .WithMany(t => t.Photos)
            .HasForeignKey(tp => tp.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tp => tp.TourId);
        builder.HasIndex(tp => tp.DisplayOrder);
    }
}