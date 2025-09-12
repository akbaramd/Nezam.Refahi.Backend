namespace Nezam.Refahi.Settings.Application.Queries.GetSettingByKey;

/// <summary>
/// Detailed section information
/// </summary>
public class SectionDetailDto
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
}