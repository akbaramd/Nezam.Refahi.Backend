using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Commands.BulkUpdateSettings;

/// <summary>
/// Command to bulk update multiple settings
/// </summary>
public record BulkUpdateSettingsCommand : IRequest<ApplicationResult<BulkUpdateSettingsResponse>>
{
    /// <summary>
    /// Dictionary of setting ID to new value
    /// </summary>
    public Dictionary<Guid, string> SettingUpdates { get; init; } = new();
    
    /// <summary>
    /// The ID of the user making the changes
    /// </summary>
    public Guid UserId { get; init; }
    
    /// <summary>
    /// Optional reason for the changes
    /// </summary>
    public string? ChangeReason { get; init; }
}
