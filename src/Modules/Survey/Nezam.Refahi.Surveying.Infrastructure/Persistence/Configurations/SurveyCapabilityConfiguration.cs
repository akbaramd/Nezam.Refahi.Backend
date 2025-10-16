using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Surveying.Domain.Entities;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for SurveyCapability entity
/// </summary>
public class SurveyCapabilityConfiguration : IEntityTypeConfiguration<SurveyCapability>
{
    public void Configure(EntityTypeBuilder<SurveyCapability> builder)
    {
        // Table configuration
        builder.ToTable("SurveyCapabilities", "surveying");

        // Primary key
        builder.HasKey(sc => sc.Id);
        builder.Property(sc => sc.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(sc => sc.SurveyId)
            .IsRequired();

        builder.Property(sc => sc.CapabilityCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sc => sc.CapabilityTitleSnapshot)
            .HasMaxLength(200);

        // Composite unique index
        builder.HasIndex(sc => new { sc.SurveyId, sc.CapabilityCode }).IsUnique();

        // Indexes
        builder.HasIndex(sc => sc.SurveyId);
        builder.HasIndex(sc => sc.CapabilityCode);
    }
}
