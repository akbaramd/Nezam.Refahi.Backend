using Nezam.Refahi.Settings.Domain.ValueObjects;

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

/// <summary>
/// DTO for a setting
/// </summary>
public class SettingDto
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
    
    /// <summary>
    /// The category information
    /// </summary>
    public CategoryInfo Category { get; set; } = new();
    
    /// <summary>
    /// The section information
    /// </summary>
    public SectionInfo Section { get; set; } = new();
    
    /// <summary>
    /// When the setting was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}

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
