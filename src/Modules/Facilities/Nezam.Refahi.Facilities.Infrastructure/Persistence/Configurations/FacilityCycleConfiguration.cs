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

        builder.Property(c => c.RestrictToPreviousCycles)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.ApprovalMessage)
            .HasMaxLength(2000);

        builder.Property(c => c.InterestRate)
            .HasPrecision(5, 4);

        builder.Property(c => c.PaymentMonths);

        builder.Property(c => c.Quota)
            .IsRequired();

        // UsedQuota is now a computed property from Requests.Count
        // We ignore it in EF Core configuration since it's calculated, not stored
        builder.Ignore(c => c.UsedQuota);

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
        // Note: Facility and FacilityCycle are separate Aggregate Roots
        // Navigation property is included for querying purposes only (read-only access)
        builder.HasOne(c => c.Facility)
            .WithMany(f => f.Cycles)
            .HasForeignKey(c => c.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Note: FacilityRequest and FacilityCycle are separate Aggregate Roots
        // Navigation property is included for querying purposes only (read-only access)
        // The relationship is configured in FacilityRequestConfiguration
        builder.HasMany(c => c.Requests)
            .WithOne(r => r.FacilityCycle)
            .HasForeignKey(r => r.FacilityCycleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Dependencies)
            .WithOne(d => d.FacilityCycle)
            .HasForeignKey(d => d.CycleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.PriceOptions)
            .WithOne(po => po.FacilityCycle)
            .HasForeignKey(po => po.FacilityCycleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Features)
            .WithOne(f => f.FacilityCycle)
            .HasForeignKey(f => f.FacilityCycleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Capabilities)
            .WithOne(cap => cap.FacilityCycle)
            .HasForeignKey(cap => cap.FacilityCycleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
