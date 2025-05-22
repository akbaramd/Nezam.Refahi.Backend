using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using System;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            // Primary key
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.HotelId)
                .IsRequired();

            builder.Property(r => r.PrimaryGuestId)
                .IsRequired();
                
            builder.Property(r => r.SpecialRequests)
                .HasMaxLength(1000);
                
            builder.Property(r => r.LastPaymentTransactionId);
            builder.Property(r => r.LastPaymentDate);
            builder.Property(r => r.PaidAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(r => r.LockExpirationTime)
                .IsRequired();

            // Value objects - DateRange (mapped as owned entity)
            builder.OwnsOne(r => r.StayPeriod, period =>
            {
                period.Property(p => p.CheckIn)
                    .HasColumnName("CheckInDate")
                    .IsRequired();
                    
                period.Property(p => p.CheckOut)
                    .HasColumnName("CheckOutDate")
                    .IsRequired();
                    
                // Note: NightCount is calculated, not stored
            });
            
            // Value objects - Status (enum)
            builder.Property(r => r.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (ReservationStatus)Enum.Parse(typeof(ReservationStatus), v))
                .HasMaxLength(50) // Protection against future enum name changes
                .IsRequired();
            
            // Value objects - Money
            builder.OwnsOne(r => r.TotalPrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("TotalPriceAmount")
                    .HasColumnType("decimal(18,2)") // Proper decimal handling
                    .IsRequired();
                
                money.Property(m => m.Currency)
                    .HasColumnName("TotalPriceCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Configure Guests collection - part of the Reservation aggregate
            builder.HasMany(r => r.Guests)
                .WithOne() // No navigation property on Guest pointing back to Reservation
                .HasForeignKey("ReservationId")
                .OnDelete(DeleteBehavior.Cascade); // Within aggregate boundary
                
            // Configure backing field for Guests collection
            builder.Metadata.FindNavigation(nameof(Reservation.Guests))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // Relationships with other aggregates
            builder.HasOne(r => r.Hotel)
                .WithMany(h => h.Reservations)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Restrict); // Cross-aggregate boundary
                
            builder.HasOne(r => r.PrimaryGuest) 
                .WithMany()
                .HasForeignKey(r => r.PrimaryGuestId)
                .OnDelete(DeleteBehavior.Restrict); // Cross-aggregate boundary

            // Create indexes for commonly queried fields
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => new { r.HotelId, r.Status });
            builder.HasIndex(r => r.PrimaryGuestId);
            builder.HasIndex(r => r.LastPaymentTransactionId)
                .HasFilter("[LastPaymentTransactionId] IS NOT NULL"); // Filtered index for nullable field
        }
    }
}
