using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Refund entity
/// </summary>
public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        // Table configuration
        builder.ToTable("Refunds", "finance");

        // Primary key
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(r => r.BillId)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.RequestedByNationalNumber)
            .IsRequired()
            .HasMaxLength(10)
            .IsFixedLength();

        builder.Property(r => r.RequestedAt)
            .IsRequired();

        builder.Property(r => r.ProcessedAt);

        builder.Property(r => r.CompletedAt);

        builder.Property(r => r.GatewayRefundId)
            .HasMaxLength(100);

        builder.Property(r => r.GatewayReference)
            .HasMaxLength(100);

        builder.Property(r => r.ProcessorNotes)
            .HasMaxLength(1000);

        builder.Property(r => r.RejectionReason)
            .HasMaxLength(500);

        // Money value object
        builder.OwnsOne(r => r.Amount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("AmountRials")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        // Relationships
        builder.HasOne(r => r.Bill)
            .WithMany(b => b.Refunds)
            .HasForeignKey(r => r.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(r => r.BillId);

        builder.HasIndex(r => r.Status);

        builder.HasIndex(r => r.RequestedByNationalNumber);

        builder.HasIndex(r => r.RequestedAt);

        builder.HasIndex(r => r.CompletedAt);

        builder.HasIndex(r => r.GatewayRefundId);
    }
}