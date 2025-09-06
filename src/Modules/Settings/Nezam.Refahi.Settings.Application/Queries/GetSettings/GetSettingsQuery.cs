using MediatR;
using Nezam.Refahi.Settings.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Queries.GetSettings;

/// <summary>
/// Query to get settings with filters and pagination
/// </summary>
public record GetSettingsQuery : IRequest<ApplicationResult<GetSettingsResponse>>
{
    /// <summary>
    /// The section name to filter by
    /// </summary>
    public string? SectionName { get; init; }
    
    /// <summary>
    /// The category name to filter by
    /// </summary>
    public string? CategoryName { get; init; }
    
    /// <summary>
    /// The setting key to search for
    /// </summary>
    public string? SearchTerm { get; init; }
    
    /// <summary>
    /// The setting type to filter by
    /// </summary>
    public SettingType? Type { get; init; }
    
    /// <summary>
    /// Whether to include only active settings
    /// </summary>
    public bool OnlyActive { get; init; } = true;
    
    /// <summary>
    /// The page number (1-based)
    /// </summary>
    public int PageNumber { get; init; } = 1;
    
    /// <summary>
    /// The page size
    /// </summary>
    public int PageSize { get; init; } = 20;
    
    /// <summary>
    /// The field to sort by
    /// </summary>
    public string SortBy { get; init; } = "DisplayOrder";
    
    /// <summary>
    /// Whether to sort in descending order
    /// </summary>
    public bool SortDescending { get; init; } = false;
}
