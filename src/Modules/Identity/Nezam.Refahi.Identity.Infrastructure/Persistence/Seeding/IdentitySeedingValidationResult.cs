namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Seeding;

/// <summary>
/// Result of seeding validation
/// </summary>
public class IdentitySeedingValidationResult
{
  public bool IsValid { get; set; }
  public bool RolesValid { get; set; }
  public bool UsersValid { get; set; }
  public int RoleCount { get; set; }
  public int AdminUserCount { get; set; }
  public int VerifiedUserCount { get; set; }
  public string? ValidationError { get; set; }
}