using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for FacilityCycleDependency entity
/// </summary>
public class FacilityCycleDependencyConfiguration : IEntityTypeConfiguration<FacilityCycleDependency>
{
    public void Configure(EntityTypeBuilder<FacilityCycleDependency> builder)
    {
        // Table name
        builder.ToTable("FacilityCycleDependencies", "facilities");

        // Primary key
        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.Id)
            .IsRequired();

        builder.Property(d => d.CycleId)
            .IsRequired();

        builder.Property(d => d.RequiredFacilityId)
            .IsRequired();

        builder.Property(d => d.RequiredFacilityName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.MustBeCompleted)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(d => d.CycleId);
        builder.HasIndex(d => d.RequiredFacilityId);
        builder.HasIndex(d => new { d.CycleId, d.RequiredFacilityId })
            .IsUnique();

        // Foreign key relationships
        builder.HasOne<FacilityCycle>()
            .WithMany(c => c.Dependencies)
            .HasForeignKey(d => d.CycleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_FacilityCycleDependencies_CycleId_NotEqual_RequiredFacilityId",
            "CycleId != RequiredFacilityId"));
    }
}
