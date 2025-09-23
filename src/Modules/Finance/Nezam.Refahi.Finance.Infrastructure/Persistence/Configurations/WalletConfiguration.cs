using MCA.SharedKernel.Infrastructure.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Wallet entity
/// </summary>
public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        // Table configuration
        builder.ToTable("Wallets", "finance");

        // Primary key
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(w => w.NationalNumber)
            .IsRequired()
            .HasMaxLength(10)
            .IsFixedLength();

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(w => w.WalletName)
            .HasMaxLength(100);

        builder.Property(w => w.Description)
            .HasMaxLength(500);

        builder.Property(w => w.LastTransactionAt);

        // Money value object
        builder.OwnsOne(w => w.Balance, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("BalanceRials")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        // Metadata as JSON
        builder.Property(w => w.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null!),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions)null!) ?? new Dictionary<string, string>())
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasMany(w => w.Transactions)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(w => w.NationalNumber)
            .IsUnique();

        builder.HasIndex(w => w.Status);

        builder.HasIndex(w => w.CreatedAt);

        builder.HasIndex(w => w.LastTransactionAt);

        // Composite index for performance
        builder.HasIndex(w => new { w.NationalNumber, w.Status });

        builder.ConfigureFullAuditableEntity();
    }
}
