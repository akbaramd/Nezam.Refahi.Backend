using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Surveying.Domain.Entities;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for QuestionOption entity
/// </summary>
public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
{
    public void Configure(EntityTypeBuilder<QuestionOption> builder)
    {
        // Table configuration
        builder.ToTable("QuestionOptions", "surveying");

        // Primary key
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(o => o.QuestionId)
            .IsRequired();

        builder.Property(o => o.Text)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.Order)
            .IsRequired();

        builder.Property(o => o.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(o => o.QuestionId);
        builder.HasIndex(o => new { o.QuestionId, o.Order });
        builder.HasIndex(o => o.IsActive);
    }
}
