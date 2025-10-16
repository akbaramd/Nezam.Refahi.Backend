using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Surveying.Domain.Entities;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for QuestionAnswer entity
/// </summary>
public class QuestionAnswerConfiguration : IEntityTypeConfiguration<QuestionAnswer>
{
    public void Configure(EntityTypeBuilder<QuestionAnswer> builder)
    {
        // Table configuration
        builder.ToTable("QuestionAnswers", "surveying");

        // Primary key
        builder.HasKey(qa => qa.Id);
        builder.Property(qa => qa.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(qa => qa.ResponseId)
            .IsRequired();

        builder.Property(qa => qa.QuestionId)
            .IsRequired();

        builder.Property(qa => qa.RepeatIndex)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(qa => qa.TextAnswer)
            .HasMaxLength(2000);

        // Relationships
        builder.HasOne(qa => qa.Response)
            .WithMany(r => r.QuestionAnswers)
            .HasForeignKey(qa => qa.ResponseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Question>()
            .WithMany()
            .HasForeignKey(qa => qa.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure cascade delete for SelectedOptions
        builder.HasMany(qa => qa.SelectedOptions)
            .WithOne()
            .HasForeignKey(qao => qao.QuestionAnswerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(qa => qa.ResponseId);
        builder.HasIndex(qa => qa.QuestionId);
        builder.HasIndex(qa => qa.RepeatIndex);
        
        // Unique constraint for (ResponseId, QuestionId, RepeatIndex)
        builder.HasIndex(qa => new { qa.ResponseId, qa.QuestionId, qa.RepeatIndex }).IsUnique();
    }
}
