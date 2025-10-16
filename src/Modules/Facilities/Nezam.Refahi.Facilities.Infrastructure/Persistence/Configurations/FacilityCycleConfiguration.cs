using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for FacilityCycle entity
/// </summary>
public class FacilityCycleConfiguration : IEntityTypeConfiguration<FacilityCycle>
{
    public void Configure(EntityTypeBuilder<FacilityCycle> builder)
    {
        builder.ToTable("FacilityCycles");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.FacilityId)
            .IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.AdmissionStrategy)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("FIFO");

        builder.Property(c => c.WaitlistCapacity);

        builder.Property(c => c.Quota)
            .IsRequired();

        builder.Property(c => c.UsedQuota)
            .IsRequired()
            .HasDefaultValue(0);

        // Amount limits for this cycle
        builder.OwnsOne(c => c.MinAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("MinAmountRials")
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .HasColumnName("MinCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("IRR");
        });

        builder.OwnsOne(c => c.MaxAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("MaxAmountRials")
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .HasColumnName("MaxCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("IRR");
        });

        builder.OwnsOne(c => c.DefaultAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("DefaultAmountRials")
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .HasColumnName("DefaultCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("IRR");
        });

        // Payment terms
        builder.Property(c => c.PaymentMonths)
            .IsRequired()
            .HasDefaultValue(12);

        builder.Property(c => c.InterestRate)
            .HasPrecision(5, 4);

        // Cooldown and rules
        builder.Property(c => c.CooldownDays)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.IsRepeatable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.IsExclusive)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.ExclusiveSetId)
            .HasMaxLength(100);

        builder.Property(c => c.MaxActiveAcrossCycles);

        // Collections
        builder.Property(c => c.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        // Indexes
        builder.HasIndex(c => c.FacilityId);
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.StartDate);
        builder.HasIndex(c => c.EndDate);
        builder.HasIndex(c => new { c.FacilityId, c.StartDate, c.EndDate });

        // Relationships
        builder.HasOne(c => c.Facility)
            .WithMany(f => f.Cycles)
            .HasForeignKey(c => c.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Applications)
            .WithOne(a => a.FacilityCycle)
            .HasForeignKey(a => a.FacilityCycleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Dependencies)
            .WithOne(d => d.FacilityCycle)
            .HasForeignKey(d => d.CycleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
