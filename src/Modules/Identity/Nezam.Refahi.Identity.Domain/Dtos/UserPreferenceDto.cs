namespace Nezam.Refahi.Identity.Domain.Dtos;

/// <summary>
/// ترجیح کاربر
/// </summary>
public class UserPreferenceDto
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }

  public string Key { get; set; } = string.Empty;
  public PreferenceValueDto Value { get; set; } = new();

  public string Category { get; set; } = string.Empty;
  public int DisplayOrder { get; set; }
  public bool IsActive { get; set; }
}