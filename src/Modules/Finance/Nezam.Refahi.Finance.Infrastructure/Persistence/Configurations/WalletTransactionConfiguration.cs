using MCA.SharedKernel.Infrastructure.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for WalletTransaction entity
/// </summary>
public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        // Table configuration
        builder.ToTable("WalletTransactions", "finance");

        // Primary key
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(t => t.WalletId)
            .IsRequired();

        builder.Property(t => t.TransactionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.ReferenceId)
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.ExternalReference)
            .HasMaxLength(200);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // Money value objects
        builder.OwnsOne(t => t.Amount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("AmountRials")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        builder.OwnsOne(t => t.BalanceAfter, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("BalanceAfterRials")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        // Metadata as JSON
        builder.Property(t => t.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null!),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions)null!) ?? new Dictionary<string, string>())
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(t => t.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.WalletId);

        builder.HasIndex(t => t.TransactionType);

        builder.HasIndex(t => t.CreatedAt);

        builder.HasIndex(t => t.ReferenceId);

        builder.HasIndex(t => t.ExternalReference);

        // Composite indexes for performance
        builder.HasIndex(t => new { t.WalletId, t.CreatedAt });

        builder.HasIndex(t => new { t.WalletId, t.TransactionType });

        builder.HasIndex(t => new { t.TransactionType, t.CreatedAt });

        builder.ConfigureEntity();
    }
}
