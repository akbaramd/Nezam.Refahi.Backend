using MCA.SharedKernel.Domain;


namespace Nezam.Refahi.Surveying.Domain.Entities;

/// <summary>
/// Survey capability entity representing a link between a survey and a capability
/// Uses stable CapabilityCode from Definitions for referential integrity
/// </summary>
public sealed class SurveyCapability : Entity<Guid>
{
    public Guid SurveyId { get; private set; }
    
    /// <summary>
    /// Stable capability code from Definitions (e.g., "can_export", "can_analyze")
    /// </summary>
    public string CapabilityCode { get; private set; } = string.Empty;
    
    /// <summary>
    /// Snapshot of capability title at the time of association (for historical display)
    /// </summary>
    public string? CapabilityTitleSnapshot { get; private set; }

    // Private constructor for EF Core
    private SurveyCapability() : base() { }

    /// <summary>
    /// Creates a new survey capability link using stable code
    /// </summary>
    public SurveyCapability(Guid surveyId, string capabilityCode, string? capabilityTitleSnapshot = null)
        : base(Guid.NewGuid())
    {
        if (surveyId == Guid.Empty)
            throw new ArgumentException("Survey ID cannot be empty", nameof(surveyId));

        if (string.IsNullOrWhiteSpace(capabilityCode))
            throw new ArgumentException("Capability code cannot be empty", nameof(capabilityCode));

        SurveyId = surveyId;
        CapabilityCode = capabilityCode.Trim();
        CapabilityTitleSnapshot = capabilityTitleSnapshot?.Trim();
    }

    /// <summary>
    /// Updates the capability title snapshot (for historical display purposes)
    /// </summary>
    public void UpdateTitleSnapshot(string? titleSnapshot)
    {
        CapabilityTitleSnapshot = titleSnapshot?.Trim();
    }
}
