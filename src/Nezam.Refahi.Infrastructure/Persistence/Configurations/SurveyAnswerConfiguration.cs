using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the SurveyAnswer entity.
/// </summary>
public class SurveyAnswerConfiguration : IEntityTypeConfiguration<SurveyAnswer>
{
    public void Configure(EntityTypeBuilder<SurveyAnswer> builder)
    {
        // Table configuration
        builder.ToTable("SurveyAnswers");
        
        // Primary key
        builder.HasKey(a => a.Id);
        
        // Base entity properties
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.ModifiedAt).IsRequired();
        
        // Properties configuration
        builder.Property(a => a.ResponseId)
            .IsRequired();
            
        builder.Property(a => a.QuestionId)
            .IsRequired();
            
        builder.Property(a => a.OptionId)
            .IsRequired(false);
            
        builder.Property(a => a.TextAnswer)
            .IsRequired(false)
            .HasMaxLength(4000);
            
        builder.Property(a => a.FilePath)
            .IsRequired(false)
            .HasMaxLength(1000);
            
        // Relationships with other entities
        builder.HasOne(a => a.Option)
            .WithMany()
            .HasForeignKey(a => a.OptionId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete from option to answer
            
        // Create indexes for commonly queried fields
        builder.HasIndex(a => a.ResponseId);
        builder.HasIndex(a => a.QuestionId);
        builder.HasIndex(a => a.OptionId);
        builder.HasIndex(a => new { a.ResponseId, a.QuestionId }).IsUnique(); // Ensure one answer per question per response
        builder.HasIndex(a => new { a.QuestionId, a.OptionId }); // For analyzing which options were selected for a question
    }
}
