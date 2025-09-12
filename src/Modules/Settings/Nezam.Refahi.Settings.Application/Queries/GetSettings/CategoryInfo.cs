namespace Nezam.Refahi.Settings.Application.Queries.GetSettings;

/// <summary>
/// Category information
/// </summary>
public class CategoryInfo
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
}