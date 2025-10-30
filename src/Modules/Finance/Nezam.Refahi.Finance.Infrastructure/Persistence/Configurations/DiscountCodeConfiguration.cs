using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for DiscountCode entity
/// </summary>
public class DiscountCodeConfiguration : IEntityTypeConfiguration<DiscountCode>
{
    public void Configure(EntityTypeBuilder<DiscountCode> builder)
    {
        builder.ToTable("DiscountCodes", "finance");

        // Primary key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("کد تخفیف");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("عنوان کد تخفیف");

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .HasComment("توضیحات کد تخفیف");

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasComment("نوع تخفیف (درصدی یا مبلغی)");

        builder.Property(x => x.DiscountValue)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasComment("مقدار تخفیف");

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasComment("وضعیت کد تخفیف");

        builder.Property(x => x.ValidFrom)
            .IsRequired()
            .HasComment("تاریخ شروع اعتبار");

        builder.Property(x => x.ValidTo)
            .IsRequired()
            .HasComment("تاریخ پایان اعتبار");

        builder.Property(x => x.UsageLimit)
            .HasComment("حد مجاز استفاده");

        builder.Property(x => x.UsedCount)
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("تعداد استفاده شده");

        builder.Property(x => x.IsSingleUse)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("آیا یکبار مصرف است");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("آیا فعال است");

        builder.Property(x => x.CreatedByExternalUserId)
            .HasComment("شناسه کاربر ایجادکننده");

        builder.Property(x => x.CreatedByUserFullName)
            .HasMaxLength(200)
            .HasComment("نام کامل کاربر ایجادکننده");

        // Money value objects
        builder.OwnsOne(x => x.MaximumDiscountAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("MaximumDiscountAmountRials")
                .HasPrecision(18, 2)
                .HasComment("حداکثر مبلغ تخفیف");
        });

        builder.OwnsOne(x => x.MinimumBillAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("MinimumBillAmountRials")
                .HasPrecision(18, 2)
                .HasComment("حداقل مبلغ فاکتور");
        });

        // Metadata as JSON
        builder.Property(x => x.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
            )
            .HasComment("اطلاعات اضافی");

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_DiscountCodes_Code");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_DiscountCodes_Status");

        builder.HasIndex(x => x.ValidFrom)
            .HasDatabaseName("IX_DiscountCodes_ValidFrom");

        builder.HasIndex(x => x.ValidTo)
            .HasDatabaseName("IX_DiscountCodes_ValidTo");

        builder.HasIndex(x => new { x.Status, x.ValidFrom, x.ValidTo })
            .HasDatabaseName("IX_DiscountCodes_Status_ValidDates");

        // Navigation properties
        builder.HasMany(x => x.Usages)
            .WithOne()
            .HasForeignKey("DiscountCodeId")
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasComment("تاریخ ایجاد");

        builder.Property(x => x.LastModifiedAt)
            .HasComment("تاریخ آخرین تغییر");

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100)
            .HasComment("ایجادکننده");

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(100)
            .HasComment("آخرین تغییردهنده");

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("آیا حذف شده");

        builder.Property(x => x.DeletedAt)
            .HasComment("تاریخ حذف");

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(100)
            .HasComment("حذف‌کننده");
    }
}
