using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for Facility entity
/// </summary>
public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("Facilities");

        // Primary key
        builder.HasKey(f => f.Id);

        // Properties
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(f => f.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(f => f.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(f => f.Description)
            .HasMaxLength(1000);

        // Bank information
        builder.Property(f => f.BankName)
            .HasMaxLength(200);

        builder.Property(f => f.BankCode)
            .HasMaxLength(50);

        builder.Property(f => f.BankAccountNumber)
            .HasMaxLength(50);

        // Metadata as JSON
        builder.Property(f => f.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
            .HasColumnType("nvarchar(max)");

        // Indexes
        builder.HasIndex(f => f.Code).IsUnique();
        builder.HasIndex(f => f.Name);
        builder.HasIndex(f => f.Type);
        builder.HasIndex(f => f.Status);

        // Relationships
        builder.HasMany(f => f.Cycles)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Features)
            .WithOne()
            .HasForeignKey("FacilityId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.CapabilityPolicies)
            .WithOne()
            .HasForeignKey("FacilityId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
