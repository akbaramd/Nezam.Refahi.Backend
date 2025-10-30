using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for FacilityRejection entity
/// </summary>
public class FacilityRejectionConfiguration : IEntityTypeConfiguration<FacilityRejection>
{
    public void Configure(EntityTypeBuilder<FacilityRejection> builder)
    {
        builder.ToTable("FacilityRejections");

        // Primary key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.RequestId)
            .IsRequired();

        builder.Property(r => r.Reason)
            .IsRequired();

        builder.Property(r => r.Details)
            .HasMaxLength(2000);

        builder.Property(r => r.RejectedByUserId)
            .IsRequired();

        builder.Property(r => r.RejectedByUserName)
            .HasMaxLength(200);

        builder.Property(r => r.RejectedAt)
            .IsRequired();

        builder.Property(r => r.RejectionType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.Property(r => r.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        // Indexes
        builder.HasIndex(r => r.RequestId).IsUnique();
        builder.HasIndex(r => r.RejectedAt);
        builder.HasIndex(r => r.RejectedByUserId);

        // Relationships
        builder.HasOne(r => r.Request)
            .WithOne()
            .HasForeignKey<FacilityRejection>(x => x.RequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}



