using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects;
using System;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            // Primary key
            builder.HasKey(pt => pt.Id);

            // Properties
            builder.Property(pt => pt.ReservationId)
                .IsRequired();

            builder.Property(pt => pt.CustomerId);

            builder.Property(pt => pt.TransactionReference)
                .HasMaxLength(100);

            builder.Property(pt => pt.Gateway)
                .HasMaxLength(50);

            builder.Property(pt => pt.IsRefund)
                .IsRequired();
                
            builder.Property(pt => pt.RefundedTransactionId);

            builder.Property(pt => pt.FailureReason)
                .HasMaxLength(500);


            // Value objects - Status
            builder.Property(pt => pt.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v))
                .IsRequired();

            // Value objects - Payment Method
            builder.Property(pt => pt.PaymentMethod)
                .HasConversion(
                    v => v.ToString(),
                    v => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), v))
                .IsRequired();

            // Value objects - Money
            builder.OwnsOne(pt => pt.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Amount")
                    .IsRequired();
                
                money.Property(m => m.Currency)
                    .HasColumnName("AmountCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Value objects - AuthorizedAmount
            builder.OwnsOne(pt => pt.AuthorizedAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("AuthorizedAmount");
                
                money.Property(m => m.Currency)
                    .HasColumnName("AuthorizedAmountCurrency")
                    .HasMaxLength(3);
            });

            // Value objects - CapturedAmount
            builder.OwnsOne(pt => pt.CapturedAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("CapturedAmount");
                
                money.Property(m => m.Currency)
                    .HasColumnName("CapturedAmountCurrency")
                    .HasMaxLength(3);
            });

            // Value objects - RefundFee
            builder.OwnsOne(pt => pt.RefundFee, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("RefundFee");
                
                money.Property(m => m.Currency)
                    .HasColumnName("RefundFeeCurrency")
                    .HasMaxLength(3);
            });

            // Value objects - OriginalAmount
            builder.OwnsOne(pt => pt.OriginalAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("OriginalAmount");
                
                money.Property(m => m.Currency)
                    .HasColumnName("OriginalAmountCurrency")
                    .HasMaxLength(3);
            });

            // Value objects - DisputeStatus
            builder.Property(pt => pt.DisputeStatus)
                .HasConversion(
                    v => v.HasValue ? v.ToString() : null,
                    v => string.IsNullOrEmpty(v) ? null : (PaymentDisputeStatus?)Enum.Parse(typeof(PaymentDisputeStatus), v));

            // Index to improve query performance
            builder.HasIndex(pt => pt.ReservationId);
            builder.HasIndex(pt => pt.TransactionReference);
            builder.HasIndex(pt => pt.Status);
            builder.HasIndex(pt => pt.CustomerId);
        }
    }
}
