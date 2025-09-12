namespace Nezam.Refahi.Settings.Application.Commands.BulkUpdateSettings;

/// <summary>
/// Response data for the BulkUpdateSettingsCommand
/// </summary>
public class BulkUpdateSettingsResponse
{
    /// <summary>
    /// The total number of settings processed
    /// </summary>
    public int TotalProcessed { get; set; }
    
    /// <summary>
    /// The number of settings successfully updated
    /// </summary>
    public int SuccessfullyUpdated { get; set; }
    
    /// <summary>
    /// The number of settings that failed to update
    /// </summary>
    public int FailedToUpdate { get; set; }
    
    /// <summary>
    /// The number of change events created
    /// </summary>
    public int ChangeEventsCreated { get; set; }
    
    /// <summary>
    /// Details of successful updates
    /// </summary>
    public List<SuccessfulUpdate> SuccessfulUpdates { get; set; } = new();
    
    /// <summary>
    /// Details of failed updates
    /// </summary>
    public List<FailedUpdate> FailedUpdates { get; set; } = new();
    
    /// <summary>
    /// When the bulk update was completed
    /// </summary>
    public DateTime CompletedAt { get; set; }
}