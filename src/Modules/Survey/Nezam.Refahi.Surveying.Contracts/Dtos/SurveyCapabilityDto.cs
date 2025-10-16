namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Survey capability data transfer object for client consumption
/// Uses stable capability codes from Definitions for referential integrity
/// </summary>
public class SurveyCapabilityDto
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    
    /// <summary>
    /// Stable capability code from Definitions (e.g., "can_export", "can_analyze")
    /// </summary>
    public string CapabilityCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Snapshot of capability title at the time of association (for historical display)
    /// </summary>
    public string? CapabilityTitleSnapshot { get; set; }
}
