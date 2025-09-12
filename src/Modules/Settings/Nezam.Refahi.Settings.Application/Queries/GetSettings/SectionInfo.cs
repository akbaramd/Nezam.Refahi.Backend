namespace Nezam.Refahi.Settings.Application.Queries.GetSettings;

/// <summary>
/// Section information
/// </summary>
public class SectionInfo
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
}