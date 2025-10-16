using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Surveying.Domain.Entities;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for QuestionAnswerOption entity
/// </summary>
public class QuestionAnswerOptionConfiguration : IEntityTypeConfiguration<QuestionAnswerOption>
{
    public void Configure(EntityTypeBuilder<QuestionAnswerOption> builder)
    {
        // Table configuration
        builder.ToTable("QuestionAnswerOptions", "surveying");

        // Primary key
        builder.HasKey(qao => qao.Id);
        builder.Property(qao => qao.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(qao => qao.QuestionAnswerId)
            .IsRequired();

        builder.Property(qao => qao.OptionId)
            .IsRequired();

        // Relationships
        builder.HasOne<QuestionAnswer>()
            .WithMany(qa => qa.SelectedOptions)
            .HasForeignKey(qao => qao.QuestionAnswerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<QuestionOption>()
            .WithMany()
            .HasForeignKey(qao => qao.OptionId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(qao => qao.QuestionAnswerId);
        builder.HasIndex(qao => qao.OptionId);
        builder.HasIndex(qao => new { qao.QuestionAnswerId, qao.OptionId }).IsUnique();
    }
}
