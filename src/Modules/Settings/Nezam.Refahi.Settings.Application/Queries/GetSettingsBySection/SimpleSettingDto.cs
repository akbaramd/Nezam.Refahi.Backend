using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Application.Queries.GetSettingsBySection;

/// <summary>
/// Simple DTO for a setting
/// </summary>
public class SimpleSettingDto
{
  /// <summary>
  /// The setting ID
  /// </summary>
  public Guid Id { get; set; }
    
  /// <summary>
  /// The setting key
  /// </summary>
  public string Key { get; set; } = string.Empty;
    
  /// <summary>
  /// The setting value
  /// </summary>
  public string Value { get; set; } = string.Empty;
    
  /// <summary>
  /// The setting type
  /// </summary>
  public SettingType Type { get; set; }
    
  /// <summary>
  /// The setting description
  /// </summary>
  public string Description { get; set; } = string.Empty;
    
  /// <summary>
  /// Whether the setting is read-only
  /// </summary>
  public bool IsReadOnly { get; set; }
    
  /// <summary>
  /// Whether the setting is active
  /// </summary>
  public bool IsActive { get; set; }
    
  /// <summary>
  /// The display order
  /// </summary>
  public int DisplayOrder { get; set; }
}