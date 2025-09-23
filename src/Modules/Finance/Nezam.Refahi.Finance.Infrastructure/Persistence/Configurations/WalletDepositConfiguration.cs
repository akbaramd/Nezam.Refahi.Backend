using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for WalletDeposit
/// </summary>
public class WalletDepositConfiguration : IEntityTypeConfiguration<WalletDeposit>
{
    public void Configure(EntityTypeBuilder<WalletDeposit> builder)
    {
        // Table configuration
        builder.ToTable("WalletDeposits", "Finance");
        builder.HasKey(x => x.Id);

        // Properties configuration
        builder.Property(x => x.WalletId)
            .IsRequired();

        builder.Property(x => x.BillId);

        builder.Property(x => x.UserNationalNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasConversion(
                v => new { v.AmountRials, v.Currency },
                v => new Money(v.AmountRials));

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.ExternalReference)
            .HasMaxLength(100);

        builder.Property(x => x.RequestedAt)
            .IsRequired();

        builder.Property(x => x.CompletedAt);

        builder.Property(x => x.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        // Indexes
        builder.HasIndex(x => x.WalletId)
            .HasDatabaseName("IX_WalletDeposits_WalletId");

        builder.HasIndex(x => x.UserNationalNumber)
            .HasDatabaseName("IX_WalletDeposits_UserNationalNumber");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_WalletDeposits_Status");

        builder.HasIndex(x => x.ExternalReference)
            .HasDatabaseName("IX_WalletDeposits_ExternalReference");

        builder.HasIndex(x => x.RequestedAt)
            .HasDatabaseName("IX_WalletDeposits_RequestedAt");

        builder.HasIndex(x => new { x.WalletId, x.Status })
            .HasDatabaseName("IX_WalletDeposits_WalletId_Status");

        builder.HasIndex(x => new { x.UserNationalNumber, x.RequestedAt })
            .HasDatabaseName("IX_WalletDeposits_UserNationalNumber_RequestedAt");

        // Relationships
        builder.HasOne(x => x.Wallet)
            .WithMany()
            .HasForeignKey(x => x.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Bill)
            .WithMany()
            .HasForeignKey(x => x.BillId)
            .OnDelete(DeleteBehavior.SetNull);

        // Audit fields - WalletDeposit inherits from FullAggregateRoot which already has audit fields
    }
}
