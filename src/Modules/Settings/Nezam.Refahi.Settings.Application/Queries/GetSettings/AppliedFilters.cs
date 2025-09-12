using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Application.Queries.GetSettings;

/// <summary>
/// Applied filters information
/// </summary>
public class AppliedFilters
{
  /// <summary>
  /// The section name filter
  /// </summary>
  public string? SectionName { get; set; }
    
  /// <summary>
  /// The category name filter
  /// </summary>
  public string? CategoryName { get; set; }
    
  /// <summary>
  /// The search term filter
  /// </summary>
  public string? SearchTerm { get; set; }
    
  /// <summary>
  /// The type filter
  /// </summary>
  public SettingType? Type { get; set; }
    
  /// <summary>
  /// Whether only active settings are included
  /// </summary>
  public bool OnlyActive { get; set; }
    
  /// <summary>
  /// The sort field
  /// </summary>
  public string SortBy { get; set; } = string.Empty;
    
  /// <summary>
  /// Whether sorting is descending
  /// </summary>
  public bool SortDescending { get; set; }
}