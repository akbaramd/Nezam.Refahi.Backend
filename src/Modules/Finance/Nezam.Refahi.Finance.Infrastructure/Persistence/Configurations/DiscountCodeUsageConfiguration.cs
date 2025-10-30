using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for DiscountCodeUsage entity
/// </summary>
public class DiscountCodeUsageConfiguration : IEntityTypeConfiguration<DiscountCodeUsage>
{
    public void Configure(EntityTypeBuilder<DiscountCodeUsage> builder)
    {
        builder.ToTable("DiscountCodeUsages", "finance");

        // Primary key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.DiscountCodeId)
            .IsRequired()
            .HasComment("شناسه کد تخفیف");

        builder.Property(x => x.BillId)
            .IsRequired()
            .HasComment("شناسه فاکتور");

        builder.Property(x => x.ExternalUserId)
            .IsRequired()
            .HasComment("شناسه کاربر");

        builder.Property(x => x.UserFullName)
            .HasMaxLength(200)
            .HasComment("نام کامل کاربر");

        builder.Property(x => x.UsedAt)
            .IsRequired()
            .HasComment("تاریخ استفاده");

        builder.Property(x => x.Notes)
            .HasMaxLength(1000)
            .HasComment("یادداشت‌ها");

        // Money value objects
        builder.OwnsOne(x => x.BillAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("BillAmountRials")
                .HasPrecision(18, 2)
                .HasComment("مبلغ فاکتور");
        });

        builder.OwnsOne(x => x.DiscountAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("DiscountAmountRials")
                .HasPrecision(18, 2)
                .HasComment("مبلغ تخفیف");
        });

        // Metadata as JSON
        builder.Property(x => x.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
            )
            .HasComment("اطلاعات اضافی");

        // Indexes
        builder.HasIndex(x => x.DiscountCodeId)
            .HasDatabaseName("IX_DiscountCodeUsages_DiscountCodeId");

        builder.HasIndex(x => x.BillId)
            .HasDatabaseName("IX_DiscountCodeUsages_BillId");

        builder.HasIndex(x => x.ExternalUserId)
            .HasDatabaseName("IX_DiscountCodeUsages_ExternalUserId");

        builder.HasIndex(x => x.UsedAt)
            .HasDatabaseName("IX_DiscountCodeUsages_UsedAt");

        builder.HasIndex(x => new { x.DiscountCodeId, x.ExternalUserId })
            .HasDatabaseName("IX_DiscountCodeUsages_DiscountCodeId_ExternalUserId");

        builder.HasIndex(x => new { x.DiscountCodeId, x.BillId })
            .HasDatabaseName("IX_DiscountCodeUsages_DiscountCodeId_BillId");

        // Foreign key relationships
        builder.HasOne<DiscountCode>()
            .WithMany()
            .HasForeignKey(x => x.DiscountCodeId)
            .OnDelete(DeleteBehavior.NoAction);

      
    }
}
