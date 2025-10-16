using MCA.SharedKernel.Domain;


namespace Nezam.Refahi.Surveying.Domain.Entities;

/// <summary>
/// Survey feature entity representing a link between a survey and a feature
/// Uses stable FeatureCode from Definitions for referential integrity
/// </summary>
public sealed class SurveyFeature : Entity<Guid>
{
    public Guid SurveyId { get; private set; }
    
    /// <summary>
    /// Stable feature code from Definitions (e.g., "has_license", "is_premium")
    /// </summary>
    public string FeatureCode { get; private set; } = string.Empty;
    
    /// <summary>
    /// Snapshot of feature title at the time of association (for historical display)
    /// </summary>
    public string? FeatureTitleSnapshot { get; private set; }

    // Private constructor for EF Core
    private SurveyFeature() : base() { }

    /// <summary>
    /// Creates a new survey feature link using stable code
    /// </summary>
    public SurveyFeature(Guid surveyId, string featureCode, string? featureTitleSnapshot = null)
        : base(Guid.NewGuid())
    {
        if (surveyId == Guid.Empty)
            throw new ArgumentException("Survey ID cannot be empty", nameof(surveyId));

        if (string.IsNullOrWhiteSpace(featureCode))
            throw new ArgumentException("Feature code cannot be empty", nameof(featureCode));

        SurveyId = surveyId;
        FeatureCode = featureCode.Trim();
        FeatureTitleSnapshot = featureTitleSnapshot?.Trim();
    }

    /// <summary>
    /// Updates the feature title snapshot (for historical display purposes)
    /// </summary>
    public void UpdateTitleSnapshot(string? titleSnapshot)
    {
        FeatureTitleSnapshot = titleSnapshot?.Trim();
    }
}
