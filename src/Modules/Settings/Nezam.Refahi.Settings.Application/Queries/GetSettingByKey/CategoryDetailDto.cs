namespace Nezam.Refahi.Settings.Application.Queries.GetSettingByKey;

/// <summary>
/// Detailed category information
/// </summary>
public class CategoryDetailDto
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
}