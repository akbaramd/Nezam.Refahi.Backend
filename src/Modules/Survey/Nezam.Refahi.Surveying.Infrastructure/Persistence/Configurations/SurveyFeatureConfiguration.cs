using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Surveying.Domain.Entities;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for SurveyFeature entity
/// </summary>
public class SurveyFeatureConfiguration : IEntityTypeConfiguration<SurveyFeature>
{
    public void Configure(EntityTypeBuilder<SurveyFeature> builder)
    {
        // Table configuration
        builder.ToTable("SurveyFeatures", "surveying");

        // Primary key
        builder.HasKey(sf => sf.Id);
        builder.Property(sf => sf.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(sf => sf.SurveyId)
            .IsRequired();

        builder.Property(sf => sf.FeatureCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sf => sf.FeatureTitleSnapshot)
            .HasMaxLength(200);

        // Composite unique index
        builder.HasIndex(sf => new { sf.SurveyId, sf.FeatureCode }).IsUnique();

        // Indexes
        builder.HasIndex(sf => sf.SurveyId);
        builder.HasIndex(sf => sf.FeatureCode);
    }
}
