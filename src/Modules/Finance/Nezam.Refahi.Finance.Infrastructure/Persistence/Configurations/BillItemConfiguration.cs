using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for BillItem entity
/// </summary>
public class BillItemConfiguration : IEntityTypeConfiguration<BillItem>
{
    public void Configure(EntityTypeBuilder<BillItem> builder)
    {
        // Table configuration
        builder.ToTable("BillItems", "finance");

        // Primary key
        builder.HasKey(bi => bi.Id);
        builder.Property(bi => bi.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(bi => bi.BillId)
            .IsRequired();

        builder.Property(bi => bi.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(bi => bi.Description)
            .HasMaxLength(500);

        builder.Property(bi => bi.Quantity)
            .IsRequired();

        builder.Property(bi => bi.DiscountPercentage)
            .HasColumnType("decimal(5,2)");

        builder.Property(bi => bi.CreatedAt)
            .IsRequired();

        // Money value objects
        builder.OwnsOne(bi => bi.UnitPrice, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("UnitPriceRials")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        builder.OwnsOne(bi => bi.LineTotal, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("LineTotalRials")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        // Relationships
        builder.HasOne(bi => bi.Bill)
            .WithMany(b => b.Items)
            .HasForeignKey(bi => bi.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(bi => bi.BillId);

        builder.HasIndex(bi => bi.CreatedAt);
    }
}