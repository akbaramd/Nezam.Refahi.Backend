namespace Nezam.Refahi.Settings.Application.Commands.BulkUpdateSettings;

/// <summary>
/// Information about a failed update
/// </summary>
public class FailedUpdate
{
  /// <summary>
  /// The setting ID
  /// </summary>
  public Guid SettingId { get; set; }
    
  /// <summary>
  /// The reason for failure
  /// </summary>
  public string FailureReason { get; set; } = string.Empty;
}