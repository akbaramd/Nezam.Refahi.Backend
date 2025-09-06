using System.Text.Json.Serialization;
using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Commands.UpdateSetting;

/// <summary>
/// Command to update an existing setting value
/// </summary>
public record UpdateSettingCommand : IRequest<ApplicationResult<UpdateSettingResponse>>
{
    /// <summary>
    /// The ID of the setting to update
    /// </summary>
    public Guid SettingId { get; init; }
    
    /// <summary>
    /// The new value for the setting
    /// </summary>
    public string NewValue { get; init; } = string.Empty;
    
    /// <summary>s
    /// The ID of the user making the change (will be set from current user context)
    /// </summary>
    [JsonIgnore]
    public Guid UserId { get; init; }
    
    /// <summary>
    /// Optional reason for the change
    /// </summary>
    public string? ChangeReason { get; init; }
}
