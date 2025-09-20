using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for PaymentTransaction entity
/// </summary>
public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        // Table configuration
        builder.ToTable("PaymentTransactions", "finance");

        // Primary key
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(pt => pt.PaymentId)
            .IsRequired();

        builder.Property(pt => pt.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(pt => pt.GatewayTransactionId)
            .HasMaxLength(100);

        builder.Property(pt => pt.GatewayReference)
            .HasMaxLength(100);

        builder.Property(pt => pt.GatewayResponse)
            .HasMaxLength(2000);

        builder.Property(pt => pt.ErrorCode)
            .HasMaxLength(50);

        builder.Property(pt => pt.ErrorMessage)
            .HasMaxLength(500);

        builder.Property(pt => pt.CreatedAt)
            .IsRequired();

        // Money value object (nullable)
        builder.OwnsOne(pt => pt.ProcessedAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("ProcessedAmountRials")
                .HasColumnType("decimal(18,2)");
        });

        // Relationships
        builder.HasOne(pt => pt.Payment)
            .WithMany(p => p.Transactions)
            .HasForeignKey(pt => pt.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pt => pt.PaymentId);

        builder.HasIndex(pt => pt.GatewayTransactionId);

        builder.HasIndex(pt => pt.Status);

        builder.HasIndex(pt => pt.CreatedAt);
    }
}