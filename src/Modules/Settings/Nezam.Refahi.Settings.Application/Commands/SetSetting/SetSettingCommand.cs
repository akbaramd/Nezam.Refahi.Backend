using System.Text.Json.Serialization;
using MediatR;
using Nezam.Refahi.Settings.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Commands.SetSetting;

/// <summary>
/// Command to set a setting value (create if not exists, update if exists)
/// </summary>
public record SetSettingCommand : IRequest<ApplicationResult<SetSettingResponse>>
{
    /// <summary>
    /// The setting key
    /// </summary>
    public string Key { get; init; } = string.Empty;
    
    /// <summary>
    /// The setting value
    /// </summary>
    public string Value { get; init; } = string.Empty;
    
    /// <summary>
    /// The type of the setting value
    /// </summary>
    public SettingType Type { get; init; }
    
    /// <summary>
    /// The description of the setting
    /// </summary>
    public string Description { get; init; } = string.Empty;
    
    /// <summary>
    /// The ID of the category this setting belongs to
    /// </summary>
    public Guid CategoryId { get; init; }
    
    /// <summary>
    /// Whether the setting is read-only
    /// </summary>
    public bool IsReadOnly { get; init; } = false;
    
    /// <summary>
    /// The display order for sorting
    /// </summary>
    public int DisplayOrder { get; init; } = 0;
    
    /// <summary>
    /// The ID of the user making the change (will be set from current user context)
    /// </summary>
    [JsonIgnore]
    public Guid UserId { get; init; }
    
    /// <summary>
    /// Optional reason for the change
    /// </summary>
    public string? ChangeReason { get; init; }
}
