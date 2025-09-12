namespace Nezam.Refahi.Settings.Application.Queries.GetSettingsBySection;

/// <summary>
/// DTO for a category with its settings
/// </summary>
public class CategoryWithSettingsDto
{
  /// <summary>
  /// The category ID
  /// </summary>
  public Guid Id { get; set; }
    
  /// <summary>
  /// The category name
  /// </summary>
  public string Name { get; set; } = string.Empty;
    
  /// <summary>
  /// The category description
  /// </summary>
  public string Description { get; set; } = string.Empty;
    
  /// <summary>
  /// The display order
  /// </summary>
  public int DisplayOrder { get; set; }
    
  /// <summary>
  /// Whether the category is active
  /// </summary>
  public bool IsActive { get; set; }
    
  /// <summary>
  /// The settings in this category
  /// </summary>
  public List<SimpleSettingDto> Settings { get; set; } = new();
    
  /// <summary>
  /// The number of settings in this category
  /// </summary>
  public int SettingCount { get; set; }
}