using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for WalletSnapshot
/// </summary>
public class WalletSnapshotConfiguration : IEntityTypeConfiguration<WalletSnapshot>
{
    public void Configure(EntityTypeBuilder<WalletSnapshot> builder)
    {
        // Table configuration
        builder.ToTable("WalletSnapshots", "Finance");
        builder.HasKey(x => x.Id);

        // Properties configuration
        builder.Property(x => x.WalletId)
            .IsRequired();

        builder.Property(x => x.ExternalUserId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Balance)
            .IsRequired()
            .HasConversion(
                v => v.AmountRials,
                v => new Nezam.Refahi.Shared.Domain.ValueObjects.Money(v));

        builder.Property(x => x.SnapshotDate)
            .IsRequired()
            .HasColumnType("date"); // Only date, no time

        builder.Property(x => x.TransactionCount)
            .IsRequired();

        builder.Property(x => x.TotalDeposits)
            .IsRequired()
            .HasConversion(
                v => v.AmountRials,
                v => new Nezam.Refahi.Shared.Domain.ValueObjects.Money(v));

        builder.Property(x => x.TotalWithdrawals)
            .IsRequired()
            .HasConversion(
                v => v.AmountRials,
                v => new Nezam.Refahi.Shared.Domain.ValueObjects.Money(v));

        builder.Property(x => x.NetChange)
            .IsRequired()
            .HasConversion(
                v => v.AmountRials,
                v => new Nezam.Refahi.Shared.Domain.ValueObjects.Money(v));

        builder.Property(x => x.LastTransactionAt);

        builder.Property(x => x.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        // Indexes
        builder.HasIndex(x => x.WalletId)
            .HasDatabaseName("IX_WalletSnapshots_WalletId");

        builder.HasIndex(x => x.ExternalUserId)
            .HasDatabaseName("IX_WalletSnapshots_ExternalUserId");

        builder.HasIndex(x => x.SnapshotDate)
            .HasDatabaseName("IX_WalletSnapshots_SnapshotDate");

        // Unique constraint for WalletId + SnapshotDate
        builder.HasIndex(x => new { x.WalletId, x.SnapshotDate })
            .IsUnique()
            .HasDatabaseName("IX_WalletSnapshots_WalletId_SnapshotDate_Unique");

        // Composite index for ExternalUserId + SnapshotDate
        builder.HasIndex(x => new { x.ExternalUserId, x.SnapshotDate })
            .HasDatabaseName("IX_WalletSnapshots_ExternalUserId_SnapshotDate");

        // Relationships
        builder.HasOne(x => x.Wallet)
            .WithMany(w => w.Snapshots)
            .HasForeignKey(x => x.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields - WalletSnapshot inherits from FullAggregateRoot which already has audit fields
    }
}
