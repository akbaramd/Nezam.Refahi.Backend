using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Surveying.Domain.Entities;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Response entity
/// </summary>
public class ResponseConfiguration : IEntityTypeConfiguration<Response>
{
    public void Configure(EntityTypeBuilder<Response> builder)
    {
        // Table configuration
        builder.ToTable("Responses", "surveying");

        // Primary key
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(r => r.SurveyId)
            .IsRequired();

        builder.Property(r => r.AttemptNumber)
            .IsRequired();

        builder.Property(r => r.AttemptStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.SubmittedAt)
            .HasColumnType("datetimeoffset");

        builder.Property(r => r.CanceledAt)
            .HasColumnType("datetimeoffset");

        builder.Property(r => r.ExpiredAt)
            .HasColumnType("datetimeoffset");

        // Navigation state properties
        builder.Property(r => r.CurrentQuestionId)
            .HasColumnName("CurrentQuestionId");

        builder.Property(r => r.CurrentRepeatIndex)
            .HasColumnName("CurrentRepeatIndex")
            .IsRequired()
            .HasDefaultValue(1);

        // Value objects as owned entities
        builder.OwnsOne(r => r.Participant, p =>
        {
            p.Property(participant => participant.IsAnonymous)
                .HasColumnName("IsAnonymous")
                .IsRequired();

            p.Property(participant => participant.MemberId)
                .HasColumnName("MemberId");

            p.Property(participant => participant.ParticipantHash)
                .HasColumnName("ParticipantHash")
                .HasMaxLength(100);

            // Indexes for owned properties
            p.HasIndex(participant => participant.MemberId);
            p.HasIndex(participant => participant.ParticipantHash);
            p.HasIndex(participant => participant.IsAnonymous);
        });

        builder.OwnsOne(r => r.DemographySnapshot, ds =>
        {
            ds.Property(d => d.SchemaVersion)
                .HasColumnName("DemographySchemaVersion")
                .IsRequired()
                .HasDefaultValue(1);

            ds.Property(d => d.Data)
                .HasColumnName("DemographyData")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
                )
                .HasMaxLength(2000);
        });

        // Relationships
        builder.HasOne<Survey>()
            .WithMany()
            .HasForeignKey(r => r.SurveyId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure cascade delete for QuestionAnswers
        builder.HasMany(r => r.QuestionAnswers)
            .WithOne(qa => qa.Response)
            .HasForeignKey(qa => qa.ResponseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(r => r.SurveyId);
        builder.HasIndex(r => r.SubmittedAt);
        builder.HasIndex(r => r.AttemptStatus);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => new { r.SurveyId, r.AttemptNumber });
        
        // Note: Unique constraints for (SurveyId, Participant, AttemptNumber) will be enforced
        // at the application level since EF Core doesn't support unique constraints on owned entity properties
    }
}
