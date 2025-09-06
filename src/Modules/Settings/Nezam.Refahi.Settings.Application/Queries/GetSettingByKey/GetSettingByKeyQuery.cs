using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Queries.GetSettingByKey;

/// <summary>
/// Query to get a specific setting by its key
/// </summary>
public record GetSettingByKeyQuery : IRequest<ApplicationResult<GetSettingByKeyResponse>>
{
    /// <summary>
    /// The setting key to search for (will be set from route parameter)
    /// </summary>
    public string SettingKey { get; init; } = string.Empty;
    
    /// <summary>
    /// Whether to include only active settings
    /// </summary>
    public bool OnlyActive { get; init; } = true;
}
