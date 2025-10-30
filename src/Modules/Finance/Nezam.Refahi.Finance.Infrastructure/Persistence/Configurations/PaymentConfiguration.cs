using MCA.SharedKernel.Infrastructure.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Payment entity
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Table configuration
        builder.ToTable("Payments", "finance");

        // Primary key
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(p => p.BillId)
            .IsRequired();

  

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.Method)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.Gateway)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.GatewayTransactionId)
            .HasMaxLength(100);

        builder.Property(p => p.GatewayReference)
            .HasMaxLength(100);


        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.ExpiryDate);

        builder.Property(p => p.CompletedAt);

        builder.Property(p => p.FailureReason)
            .HasMaxLength(1000);

        // Discount properties
        builder.Property(p => p.AppliedDiscountCode)
            .HasMaxLength(50);

        builder.Property(p => p.AppliedDiscountCodeId);

        builder.Property(p => p.IsFreePayment)
            .HasDefaultValue(false);

        builder.ConfigureSoftDeletableEntity();
        // Money value object
        builder.OwnsOne(p => p.Amount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("AmountRials")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        // Applied discount amount as owned entity
        builder.OwnsOne(p => p.AppliedDiscountAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("AppliedDiscountAmountRials")
                .HasColumnType("decimal(18,2)");
        });

        // Relationships
        builder.HasOne(p => p.Bill)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Transactions)
            .WithOne(pt => pt.Payment)
            .HasForeignKey(pt => pt.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.BillId);

        builder.HasIndex(p => p.Status);

        builder.HasIndex(p => p.GatewayTransactionId);
        builder.HasIndex(p => p.GatewayReference);

        builder.HasIndex(p => p.CreatedAt);

        builder.HasIndex(p => p.CompletedAt);
    }
}