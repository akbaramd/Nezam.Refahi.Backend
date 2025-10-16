using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Survey entity
/// </summary>
public class SurveyConfiguration : IEntityTypeConfiguration<Survey>
{
    public void Configure(EntityTypeBuilder<Survey> builder)
    {
        // Table configuration
        builder.ToTable("Surveys", "surveying");

        // Primary key
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.State)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.StartAt)
            .HasColumnType("datetimeoffset");

        builder.Property(s => s.EndAt)
            .HasColumnType("datetimeoffset");

        builder.Property(s => s.IsAnonymous)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.Version)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(s => s.StructureVersion)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(s => s.IsStructureFrozen)
            .IsRequired()
            .HasDefaultValue(false);

        // Value objects as owned entities
        builder.OwnsOne(s => s.ParticipationPolicy, pp =>
        {
            pp.Property(p => p.MaxAttemptsPerMember)
                .HasColumnName("MaxAttemptsPerMember")
                .IsRequired();

            pp.Property(p => p.AllowMultipleSubmissions)
                .HasColumnName("AllowMultipleSubmissions")
                .IsRequired();

            pp.Property(p => p.CoolDownSeconds)
                .HasColumnName("CoolDownSeconds");
        });

        builder.OwnsOne(s => s.AudienceFilter, af =>
        {
            af.Property(a => a.FilterExpression)
                .HasColumnName("AudienceFilter")
                .HasMaxLength(2000);

            af.Property(a => a.FilterVersion)
                .HasColumnName("AudienceFilterVersion")
                .IsRequired()
                .HasDefaultValue(1);
        });

        // Relationships
        builder.HasMany(s => s.Questions)
            .WithOne()
            .HasForeignKey(q => q.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Responses)
            .WithOne()
            .HasForeignKey(r => r.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.SurveyFeatures)
            .WithOne()
            .HasForeignKey(sf => sf.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.SurveyCapabilities)
            .WithOne()
            .HasForeignKey(sc => sc.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => s.State);
        builder.HasIndex(s => s.StartAt);
        builder.HasIndex(s => s.EndAt);
        builder.HasIndex(s => s.IsAnonymous);
    }
}
