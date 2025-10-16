using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Surveying.Domain.Entities;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence;

/// <summary>
/// Survey DbContext for Entity Framework Core
/// </summary>
public class SurveyDbContext : DbContext
{
    public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options)
    {
    }

    // DbSets for all Survey entities
    public DbSet<Survey> Surveys { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<QuestionOption> QuestionOptions { get; set; } = null!;
    public DbSet<Response> Responses { get; set; } = null!;
    public DbSet<QuestionAnswer> QuestionAnswers { get; set; } = null!;
    public DbSet<QuestionAnswerOption> QuestionAnswerOptions { get; set; } = null!;
    public DbSet<SurveyFeature> SurveyFeatures { get; set; } = null!;
    public DbSet<SurveyCapability> SurveyCapabilities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SurveyDbContext).Assembly);

        // Set default schema for Survey module
        modelBuilder.HasDefaultSchema("surveying");
    }
}
