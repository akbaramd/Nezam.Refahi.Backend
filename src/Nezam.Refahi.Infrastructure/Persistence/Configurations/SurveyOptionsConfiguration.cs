using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the SurveyOptions entity.
/// </summary>
public class SurveyOptionsConfiguration : IEntityTypeConfiguration<SurveyOptions>
{
    public void Configure(EntityTypeBuilder<SurveyOptions> builder)
    {
        // Table configuration
        builder.ToTable("SurveyOptions");
        
        // Primary key
        builder.HasKey(o => o.Id);
        
        // Base entity properties
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.ModifiedAt).IsRequired();
        
        // Properties configuration
        builder.Property(o => o.Text)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(o => o.DisplayOrder)
            .IsRequired();
            
        builder.Property(o => o.QuestionId)
            .IsRequired();
            
        // Create indexes for commonly queried fields
        builder.HasIndex(o => o.QuestionId);
        builder.HasIndex(o => new { o.QuestionId, o.DisplayOrder }); // For ordering options within a question
    }
}
