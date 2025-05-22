using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations
{
    public class HotelPhotoConfiguration : IEntityTypeConfiguration<HotelPhoto>
    {
        public void Configure(EntityTypeBuilder<HotelPhoto> builder)
        {
            // Primary key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Caption)
                .HasMaxLength(200);

            builder.Property(p => p.AltText)
                .HasMaxLength(200);

            builder.Property(p => p.IsMainPhoto)
                .IsRequired();

            builder.Property(p => p.HotelId)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.Hotel)
                .WithMany(h => h.Photos)
                .HasForeignKey(p => p.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Constraint: Only one main photo per hotel (enforces domain rule)
            builder.HasIndex(p => new { p.HotelId, p.IsMainPhoto })
                .HasFilter("\"IsMainPhoto\" = true")
                .IsUnique();
                
            // Additional index for URL lookups (helps with duplicate detection)
            builder.HasIndex(p => p.Url);
        }
    }
}
