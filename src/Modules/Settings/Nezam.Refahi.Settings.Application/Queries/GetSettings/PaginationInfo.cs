namespace Nezam.Refahi.Settings.Application.Queries.GetSettings;

/// <summary>
/// Pagination information
/// </summary>
public class PaginationInfo
{
  /// <summary>
  /// Current page number
  /// </summary>
  public int PageNumber { get; set; }
    
  /// <summary>
  /// Page size
  /// </summary>
  public int PageSize { get; set; }
    
  /// <summary>
  /// Total number of pages
  /// </summary>
  public int TotalPages { get; set; }
    
  /// <summary>
  /// Whether there is a previous page
  /// </summary>
  public bool HasPreviousPage { get; set; }
    
  /// <summary>
  /// Whether there is a next page
  /// </summary>
  public bool HasNextPage { get; set; }
}