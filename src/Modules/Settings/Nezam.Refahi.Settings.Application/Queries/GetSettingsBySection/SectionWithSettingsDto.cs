namespace Nezam.Refahi.Settings.Application.Queries.GetSettingsBySection;

/// <summary>
/// DTO for a section with its categories and settings
/// </summary>
public class SectionWithSettingsDto
{
  /// <summary>
  /// The section ID
  /// </summary>
  public Guid Id { get; set; }
    
  /// <summary>
  /// The section name
  /// </summary>
  public string Name { get; set; } = string.Empty;
    
  /// <summary>
  /// The section description
  /// </summary>
  public string Description { get; set; } = string.Empty;
    
  /// <summary>
  /// The display order
  /// </summary>
  public int DisplayOrder { get; set; }
    
  /// <summary>
  /// Whether the section is active
  /// </summary>
  public bool IsActive { get; set; }
    
  /// <summary>
  /// The categories in this section
  /// </summary>
  public List<CategoryWithSettingsDto> Categories { get; set; } = new();
    
  /// <summary>
  /// The number of categories in this section
  /// </summary>
  public int CategoryCount { get; set; }
    
  /// <summary>
  /// The total number of settings in this section
  /// </summary>
  public int TotalSettings { get; set; }
}