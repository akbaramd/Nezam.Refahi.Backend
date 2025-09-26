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
        builder.Property(w => w.ExternalUserId)
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

        // Balance is now calculated dynamically from snapshots + transactions
        // No need to configure it as a database column

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
        builder.HasIndex(w => w.ExternalUserId)
            .IsUnique();

        builder.HasIndex(w => w.Status);

        builder.HasIndex(w => w.CreatedAt);

        builder.HasIndex(w => w.LastTransactionAt);

        // Composite index for performance
        builder.HasIndex(w => new { w.ExternalUserId, w.Status });

        builder.ConfigureFullAuditableEntity();
    }
}
