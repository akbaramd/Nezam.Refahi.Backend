using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations
{
    public class HotelFeatureConfiguration : IEntityTypeConfiguration<HotelFeature>
    {
        public void Configure(EntityTypeBuilder<HotelFeature> builder)
        {
            // Primary key
            builder.HasKey(f => f.Id);

            // Properties
            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.Value)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.Description)
                .HasMaxLength(500);

            builder.Property(f => f.HotelId)
                .IsRequired();

            // Relationships
            builder.HasOne(f => f.Hotel)
                .WithMany(h => h.Features)
                .HasForeignKey(f => f.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Create index for common query patterns - features are often queried by name
            builder.HasIndex(f => new { f.HotelId, f.Name })
                .IsUnique(); // Enforce uniqueness within a hotel
        }
    }
}
