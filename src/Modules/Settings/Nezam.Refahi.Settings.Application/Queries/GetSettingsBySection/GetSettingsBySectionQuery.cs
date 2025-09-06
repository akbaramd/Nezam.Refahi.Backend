using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Queries.GetSettingsBySection;

/// <summary>
/// Query to get settings organized by section and category
/// </summary>
public record GetSettingsBySectionQuery : IRequest<ApplicationResult<GetSettingsBySectionResponse>>
{
    /// <summary>
    /// The section name to filter by (optional)
    /// </summary>
    public string? SectionName { get; init; }
    
    /// <summary>
    /// Whether to include only active sections and categories
    /// </summary>
    public bool OnlyActive { get; init; } = true;
    
    /// <summary>
    /// Whether to include empty sections and categories
    /// </summary>
    public bool IncludeEmpty { get; init; } = false;
}
