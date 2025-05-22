using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Survey aggregate root.
/// </summary>
public class SurveyConfiguration : IEntityTypeConfiguration<Survey>
{
    public void Configure(EntityTypeBuilder<Survey> builder)
    {
        // Table configuration
        builder.ToTable("Surveys");
        
        // Primary key
        builder.HasKey(s => s.Id);
        
        // Base entity properties
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.ModifiedAt).IsRequired();
        
        // Properties configuration
        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(s => s.Description)
            .HasMaxLength(2000);
            
        builder.Property(s => s.Mode)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(s => s.OpensAtUtc)
            .IsRequired();
            
        builder.Property(s => s.ClosesAtUtc)
            .IsRequired(false);
            
        builder.Property(s => s.TimeLimitSeconds)
            .IsRequired(false);
            
        builder.Property(s => s.CreatorId)
            .IsRequired();
            
        // Relationships within the aggregate boundary
        builder.HasMany(s => s.Questions)
            .WithOne(q => q.Survey)
            .HasForeignKey(q => q.SurveyId)
            .OnDelete(DeleteBehavior.Cascade); // Within aggregate boundary
            
        builder.HasMany(s => s.Responses)
            .WithOne(r => r.Survey)
            .HasForeignKey(r => r.SurveyId)
            .OnDelete(DeleteBehavior.Cascade); // Within aggregate boundary
            
        // Configure backing field for collections
        builder.Metadata.FindNavigation(nameof(Survey.Questions))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        builder.Metadata.FindNavigation(nameof(Survey.Responses))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        // Relationships with other aggregates
        builder.HasOne(s => s.Creator)
            .WithMany()
            .HasForeignKey(s => s.CreatorId)
            .OnDelete(DeleteBehavior.Restrict); // Cross-aggregate boundary
            
        // Create indexes for commonly queried fields
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.CreatorId);
        builder.HasIndex(s => s.OpensAtUtc);
        builder.HasIndex(s => new { s.Status, s.OpensAtUtc }); // Composite index for filtering active surveys
        builder.HasIndex(s => new { s.CreatorId, s.Status }); // For filtering surveys by creator and status
    }
}
