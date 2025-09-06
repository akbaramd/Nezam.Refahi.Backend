namespace Nezam.Refahi.Settings.Application.Commands.UpdateSetting;

/// <summary>
/// Response data for the UpdateSettingCommand
/// </summary>
public class UpdateSettingResponse
{
    /// <summary>
    /// The ID of the updated setting
    /// </summary>
    public Guid SettingId { get; set; }
    
    /// <summary>
    /// The new value of the setting
    /// </summary>
    public string NewValue { get; set; } = string.Empty;
    
    /// <summary>
    /// The ID of the change event created
    /// </summary>
    public Guid ChangeEventId { get; set; }
    
    /// <summary>
    /// When the setting was updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// The ID of the user who made the change
    /// </summary>
    public Guid ChangedByUserId { get; set; }
}
