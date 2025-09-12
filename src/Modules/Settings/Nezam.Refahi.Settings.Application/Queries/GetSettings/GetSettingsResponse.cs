namespace Nezam.Refahi.Settings.Application.Queries.GetSettings;

/// <summary>
/// Response data for the GetSettingsQuery
/// </summary>
public class GetSettingsResponse
{
    /// <summary>
    /// The list of settings
    /// </summary>
    public List<SettingDto> Settings { get; set; } = new();
    
    /// <summary>
    /// Pagination information
    /// </summary>
    public PaginationInfo Pagination { get; set; } = new();
    
    /// <summary>
    /// Total count of settings matching the filters
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// The applied filters
    /// </summary>
    public AppliedFilters Filters { get; set; } = new();
}