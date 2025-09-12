namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeding statistics
/// </summary>
public class IdentitySeedingStatistics
{
  public int RoleCount { get; set; }
  public int AdminUserCount { get; set; }
  public int VerifiedUserCount { get; set; }
  public DateTime Timestamp { get; set; }
}