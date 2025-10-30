using MCA.SharedKernel.Infrastructure.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Bill entity
/// </summary>
public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        // Table configuration
        builder.ToTable("Bills", "finance");

        // Primary key
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(b => b.BillNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.ReferenceId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(b => b.ReferenceTrackCode)
          .IsRequired()
          .HasDefaultValue("sdad")
          .HasMaxLength(100);
        builder.Property(b => b.ReferenceType)
          .HasColumnName("ReferenceType")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.ExternalUserId)
            .IsRequired()
            .HasMaxLength(10)
            .IsFixedLength();

        builder.Property(b => b.UserFullName)
            .HasMaxLength(200);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.IssueDate)
            .IsRequired();

        builder.Property(b => b.DueDate);

        builder.Property(b => b.FullyPaidDate);

        // Money value objects - Configure as owned entities without keys
        builder.OwnsOne(b => b.TotalAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("TotalAmountRials")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
        });

        builder.OwnsOne(b => b.PaidAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("PaidAmountRials")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            // Ensure no key is configured for owned entity
        });

        builder.OwnsOne(b => b.RemainingAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("RemainingAmountRials")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            // Ensure no key is configured for owned entity
        });

        // Discount properties
        builder.Property(b => b.DiscountCode)
            .HasMaxLength(50);

        builder.Property(b => b.DiscountCodeId);

        // Discount amount as owned entity
        builder.OwnsOne(b => b.DiscountAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("DiscountAmountRials")
                .HasColumnType("decimal(18,2)");
        });

        // Metadata as JSON
        builder.Property(b => b.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null!),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions)null!) ?? new Dictionary<string, string>())
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasMany(b => b.Items)
            .WithOne(i => i.Bill)
            .HasForeignKey(i => i.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Payments)
            .WithOne(p => p.Bill)
            .HasForeignKey(p => p.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Refunds)
            .WithOne(r => r.Bill)
            .HasForeignKey(r => r.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(b => b.BillNumber)
            .IsUnique();

        builder.HasIndex(b => new { b.ReferenceId, BillType = b.ReferenceType });

        builder.HasIndex(b => b.ExternalUserId);

        builder.HasIndex(b => b.Status);

        builder.HasIndex(b => b.IssueDate);

        builder.HasIndex(b => b.DueDate);
        
        builder.ConfigureFullAuditableEntity();

    }   
}