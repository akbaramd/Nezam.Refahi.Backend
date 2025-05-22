using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using System;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            // Primary key
            builder.HasKey(h => h.Id);
            
            // Properties
            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.Description)
                .HasMaxLength(2000);
                
            builder.Property(h => h.Capacity)
                .IsRequired();
                
            builder.Property(h => h.IsAvailable)
                .IsRequired();

            // Value object: PricePerNight (Money type)
            builder.OwnsOne(h => h.PricePerNight, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("PricePerNight")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                
                money.Property(m => m.Currency)
                    .HasColumnName("PricePerNightCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Value object: Location (LocationReference type)
            builder.OwnsOne(h => h.Location, loc =>
            {
                loc.Property(l => l.CityId)
                    .HasColumnName("CityId")
                    .IsRequired();
                    
                loc.Property(l => l.ProvinceId)
                    .HasColumnName("ProvinceId")
                    .IsRequired();
                    
                loc.Property(l => l.CityName)
                    .HasColumnName("CityName")
                    .HasMaxLength(100)
                    .IsRequired();
                    
                loc.Property(l => l.ProvinceName)
                    .HasColumnName("ProvinceName")
                    .HasMaxLength(100)
                    .IsRequired();
                    
                loc.Property(l => l.Address)
                    .HasColumnName("Address")
                    .HasMaxLength(500)
                    .IsRequired();
            });

            // Configure backing fields for collections
            // HotelFeatures collection
            builder.HasMany(h => h.Features)
                .WithOne(f => f.Hotel)
                .HasForeignKey(f => f.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

  

            // HotelPhoto collection
            builder.HasMany(h => h.Photos)
                .WithOne(p => p.Hotel)
                .HasForeignKey(p => p.HotelId)
                .OnDelete(DeleteBehavior.Cascade);


            // Reservations relationship
            builder.HasMany(h => h.Reservations)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Metadata.FindNavigation(nameof(Hotel.Features))?
              .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Metadata.FindNavigation(nameof(Hotel.Photos))?
              .SetPropertyAccessMode(PropertyAccessMode.Field);
            
            // Create indexes for common query patterns
            builder.HasIndex(h => h.Name);
            builder.HasIndex(h => h.IsAvailable);
        }
    }
}