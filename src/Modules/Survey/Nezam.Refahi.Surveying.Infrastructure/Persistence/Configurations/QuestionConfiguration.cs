using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Question entity
/// </summary>
public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        // Table configuration
        builder.ToTable("Questions", "surveying");

        // Primary key
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(q => q.SurveyId)
            .IsRequired();

        builder.Property(q => q.Kind)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.Order)
            .IsRequired();

        builder.Property(q => q.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);

        // RepeatPolicy as owned entity
        builder.OwnsOne(q => q.RepeatPolicy, rp =>
        {
            rp.Property(p => p.Kind)
                .HasColumnName("RepeatPolicyKind")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            rp.Property(p => p.MaxRepeats)
                .HasColumnName("MaxRepeats");
        });

        // Relationships
        builder.HasMany(q => q.Options)
            .WithOne()
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(q => q.SurveyId);
        builder.HasIndex(q => new { q.SurveyId, q.Order });
        builder.HasIndex(q => q.Kind);
    }
}
