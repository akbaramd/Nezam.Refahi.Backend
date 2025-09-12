namespace Nezam.Refahi.Settings.Application.Commands.BulkUpdateSettings;

/// <summary>
/// Information about a successful update
/// </summary>
public class SuccessfulUpdate
{
  /// <summary>
  /// The setting ID
  /// </summary>
  public Guid SettingId { get; set; }
    
  /// <summary>
  /// The new value
  /// </summary>
  public string NewValue { get; set; } = string.Empty;
    
  /// <summary>
  /// The change event ID created
  /// </summary>
  public Guid ChangeEventId { get; set; }
}