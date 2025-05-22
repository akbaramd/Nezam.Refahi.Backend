using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Surveis;
using System.Reflection;

namespace Nezam.Refahi.Infrastructure.Persistence;

  /// <summary>
  /// Main database context for the application
  /// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    // Accommodation bounded context
    public  DbSet<Hotel> Hotels { get; set; } = default!;
    public  DbSet<Reservation> Reservations { get; set; } = default!;

    // Identity bounded context
    public  DbSet<User> Users { get; set; } = default!;

    // Location bounded context
    public  DbSet<City> Cities { get; set; } = default!;
    public  DbSet<Province> Provinces { get; set; } = default!;

    // Payment bounded context
    public  DbSet<PaymentTransaction> PaymentTransactions { get; set; } = default!;

    // Survey bounded context
    public  DbSet<Survey> Surveys { get; set; } = default!;
    public  DbSet<SurveyQuestion> SurveyQuestions { get; set; } = default!;
    public  DbSet<SurveyOptions> SurveyOptions { get; set; } = default!;
    public  DbSet<SurveyResponse> SurveyResponses { get; set; } = default!;
    public  DbSet<SurveyAnswer> SurveyAnswers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
