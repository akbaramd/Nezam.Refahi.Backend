namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Survey feature data transfer object for client consumption
/// Uses stable feature codes from Definitions for referential integrity
/// </summary>
public class SurveyFeatureDto
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    
    /// <summary>
    /// Stable feature code from Definitions (e.g., "has_license", "is_premium")
    /// </summary>
    public string FeatureCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Snapshot of feature title at the time of association (for historical display)
    /// </summary>
    public string? FeatureTitleSnapshot { get; set; }
}
