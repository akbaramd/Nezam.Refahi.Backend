namespace Nezam.Refahi.Recreation.Infrastructure.Services;

/// <summary>
/// Service interface for seeding tour data
/// </summary>
public interface ITourSeedingService
{
    /// <summary>
    /// Seeds example tours with all related data
    /// </summary>
    Task SeedToursAsync();
}