using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the SurveyResponse entity.
/// </summary>
public class SurveyResponseConfiguration : IEntityTypeConfiguration<SurveyResponse>
{
    public void Configure(EntityTypeBuilder<SurveyResponse> builder)
    {
        // Table configuration
        builder.ToTable("SurveyResponses");
        
        // Primary key
        builder.HasKey(r => r.Id);
        
        // Base entity properties
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.ModifiedAt).IsRequired();
        
        // Properties configuration
        builder.Property(r => r.SurveyId)
            .IsRequired();
            
        builder.Property(r => r.ResponderId)
            .IsRequired(false);
            
        builder.Property(r => r.StartedAtUtc)
            .IsRequired();
            
        builder.Property(r => r.SubmittedAtUtc)
            .IsRequired(false);
            
        builder.Property(r => r.TimedOut)
            .IsRequired()
            .HasDefaultValue(false);
            
        // Relationships within the aggregate boundary
        builder.HasMany(r => r.Answers)
            .WithOne(a => a.Response)
            .HasForeignKey(a => a.ResponseId)
            .OnDelete(DeleteBehavior.Cascade); // Within aggregate boundary
            
        // Configure backing field for collections
        builder.Metadata.FindNavigation(nameof(SurveyResponse.Answers))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        // Relationships with other aggregates
        builder.HasOne(r => r.Responder)
            .WithMany()
            .HasForeignKey(r => r.ResponderId)
            .OnDelete(DeleteBehavior.Restrict); // Cross-aggregate boundary
            
        // Create indexes for commonly queried fields
        builder.HasIndex(r => r.SurveyId);
        builder.HasIndex(r => r.ResponderId);
        builder.HasIndex(r => r.SubmittedAtUtc);
        builder.HasIndex(r => new { r.SurveyId, r.SubmittedAtUtc }); // For filtering completed responses
        builder.HasIndex(r => new { r.SurveyId, r.ResponderId }); // For finding a user's response to a survey
        builder.HasIndex(r => new { r.SurveyId, r.TimedOut }); // For filtering timed out responses
    }
}
