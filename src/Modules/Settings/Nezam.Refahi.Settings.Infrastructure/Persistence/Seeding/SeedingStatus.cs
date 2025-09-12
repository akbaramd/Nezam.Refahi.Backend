namespace Nezam.Refahi.Settings.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeding status information
/// </summary>
public class SeedingStatus
{
  public bool IsSeeded { get; set; }
  public int SectionsCount { get; set; }
  public int CategoriesCount { get; set; }
  public int SettingsCount { get; set; }
  public string? Error { get; set; }
}