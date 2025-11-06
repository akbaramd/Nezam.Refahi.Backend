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

        builder.Property(f => f.Description)
            .HasMaxLength(1000);

        // Bank information
        builder.Property(f => f.BankName)
            .HasMaxLength(200);

        builder.Property(f => f.BankCode)
            .HasMaxLength(50);

        builder.Property(f => f.BankAccountNumber)
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(f => f.Code).IsUnique();
        builder.HasIndex(f => f.Name);

        // Relationships
        // Note: Cycles are separate Aggregate Roots, but we include navigation property
        // for querying purposes only (read-only access via EF Core)
        // The relationship is configured in FacilityCycleConfiguration
    }
}
