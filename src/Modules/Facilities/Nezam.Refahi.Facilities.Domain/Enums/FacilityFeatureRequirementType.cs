using System.Text.Json.Serialization;

namespace Nezam.Refahi.Facilities.Domain.Enums;

/// <summary>
/// Type of facility feature requirement
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FacilityFeatureRequirementType
{
    /// <summary>
    /// Feature is required for eligibility
    /// </summary>
    Required = 1,

    /// <summary>
    /// Feature is prohibited (member must not have this feature)
    /// </summary>
    Prohibited = 2,

    /// <summary>
    /// Feature modifies conditions (bonus/penalty)
    /// </summary>
    Modifier = 3
}
