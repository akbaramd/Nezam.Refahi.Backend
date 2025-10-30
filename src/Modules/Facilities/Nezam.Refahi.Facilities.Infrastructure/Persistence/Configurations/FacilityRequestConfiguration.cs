using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for FacilityRequest entity
/// </summary>
public class FacilityRequestConfiguration : IEntityTypeConfiguration<FacilityRequest>
{
    public void Configure(EntityTypeBuilder<FacilityRequest> builder)
    {
        builder.ToTable("FacilityRequests");

        // Primary key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.FacilityId)
            .IsRequired();

        builder.Property(r => r.FacilityCycleId)
            .IsRequired();

        builder.Property(r => r.MemberId)
            .IsRequired();

        builder.Property(r => r.UserFullName)
            .HasMaxLength(200);

        builder.Property(r => r.UserNationalId)
            .HasMaxLength(20);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.RequestNumber)
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.RejectionReason)
            .HasMaxLength(1000);

        // Optional reference to FacilityRejection record
        builder.Property(r => r.RejectionId);

        builder.Property(r => r.BankAppointmentReference)
            .HasMaxLength(100);

        builder.Property(r => r.DisbursementReference)
            .HasMaxLength(100);

        builder.Property(r => r.IdempotencyKey)
            .HasMaxLength(100);

        builder.Property(r => r.CorrelationId)
            .HasMaxLength(100);

        // Money value objects
        builder.OwnsOne(r => r.RequestedAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("RequestedAmountRials")
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .HasColumnName("RequestedCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("IRR");
        });

        builder.OwnsOne(r => r.ApprovedAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("ApprovedAmountRials")
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .HasColumnName("ApprovedCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("IRR");
        });

        // Collections
        builder.Property(r => r.PolicySnapshot)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        builder.Property(r => r.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        // Indexes
        builder.HasIndex(r => r.FacilityId);
        builder.HasIndex(r => r.FacilityCycleId);
        builder.HasIndex(r => r.MemberId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.RequestNumber).IsUnique();
        builder.HasIndex(r => r.IdempotencyKey).IsUnique();
        builder.HasIndex(r => r.CorrelationId);
        builder.HasIndex(r => r.CreatedAt);
        builder.HasIndex(r => r.ApprovedAt);
        builder.HasIndex(r => r.BankAppointmentDate);

        // Relationships
        builder.HasOne(r => r.Facility)
            .WithMany()
            .HasForeignKey(r => r.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.FacilityCycle)
            .WithMany(c => c.Applications)
            .HasForeignKey(r => r.FacilityCycleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship to FacilityRejection (optional one-to-one via RejectionId)
        builder.HasOne<FacilityRejection>()
            .WithOne(x => x.Request)
            .HasForeignKey<FacilityRejection>(x => x.RequestId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<FacilityRejection>()
            .WithMany()
            .HasForeignKey(r => r.RejectionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
