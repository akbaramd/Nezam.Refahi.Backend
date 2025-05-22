using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the SurveyQuestion entity.
/// </summary>
public class SurveyQuestionConfiguration : IEntityTypeConfiguration<SurveyQuestion>
{
    public void Configure(EntityTypeBuilder<SurveyQuestion> builder)
    {
        // Table configuration
        builder.ToTable("SurveyQuestions");
        
        // Primary key
        builder.HasKey(q => q.Id);
        
        // Base entity properties
        builder.Property(q => q.CreatedAt).IsRequired();
        builder.Property(q => q.ModifiedAt).IsRequired();
        
        // Properties configuration
        builder.Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(1000);
            
        builder.Property(q => q.Type)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(q => q.IsRequired)
            .IsRequired();
            
        builder.Property(q => q.Order)
            .IsRequired();
            
        builder.Property(q => q.SurveyId)
            .IsRequired();
            
        // Relationships within the aggregate boundary
        builder.HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade); // Within aggregate boundary
            
        builder.HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade); // Within aggregate boundary
            
        // Configure backing field for collections
        builder.Metadata.FindNavigation(nameof(SurveyQuestion.Options))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        builder.Metadata.FindNavigation(nameof(SurveyQuestion.Answers))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        // Create indexes for commonly queried fields
        builder.HasIndex(q => q.SurveyId);
        builder.HasIndex(q => new { q.SurveyId, q.Order }); // For ordering questions within a survey
        builder.HasIndex(q => new { q.SurveyId, q.Type }); // For filtering questions by type within a survey
    }
}
