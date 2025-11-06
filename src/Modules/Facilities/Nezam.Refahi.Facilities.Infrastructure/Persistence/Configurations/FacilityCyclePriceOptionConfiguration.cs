using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for FacilityCyclePriceOption entity
/// </summary>
public class FacilityCyclePriceOptionConfiguration : IEntityTypeConfiguration<FacilityCyclePriceOption>
{
    public void Configure(EntityTypeBuilder<FacilityCyclePriceOption> builder)
    {
        builder.ToTable("FacilityCyclePriceOptions");

        // Primary key
        builder.HasKey(po => po.Id);

        // Properties
        builder.Property(po => po.FacilityCycleId)
            .IsRequired();

        builder.Property(po => po.Description)
            .HasMaxLength(500);

        builder.Property(po => po.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(po => po.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Amount as owned entity
        builder.OwnsOne(po => po.Amount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("AmountRials")
                .IsRequired()
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .HasDefaultValue("IRR");
        });

        // Indexes
        builder.HasIndex(po => po.FacilityCycleId);
        builder.HasIndex(po => new { po.FacilityCycleId, po.DisplayOrder });

        // Relationships
        builder.HasOne(po => po.FacilityCycle)
            .WithMany(c => c.PriceOptions)
            .HasForeignKey(po => po.FacilityCycleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

