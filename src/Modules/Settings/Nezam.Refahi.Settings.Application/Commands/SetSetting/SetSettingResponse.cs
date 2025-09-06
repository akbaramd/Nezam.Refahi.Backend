using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Application.Commands.SetSetting;

/// <summary>
/// Response data for the SetSettingCommand
/// </summary>
public class SetSettingResponse
{
    /// <summary>
    /// The ID of the setting
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
    /// The type of the setting value
    /// </summary>
    public SettingType Type { get; set; }
    
    /// <summary>
    /// Whether the setting was created (true) or updated (false)
    /// </summary>
    public bool WasCreated { get; set; }
    
    /// <summary>
    /// The ID of the change event if this was an update
    /// </summary>
    public Guid? ChangeEventId { get; set; }
    
    /// <summary>
    /// When the setting was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}
