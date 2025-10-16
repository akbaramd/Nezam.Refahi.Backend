using System.Text.Json.Serialization;

namespace Nezam.Refahi.Facilities.Domain.Enums;

/// <summary>
/// Type of capability policy
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CapabilityPolicyType
{
    /// <summary>
    /// Capability is required for eligibility
    /// </summary>
    Required = 1,

    /// <summary>
    /// Capability is prohibited
    /// </summary>
    Prohibited = 2,

    /// <summary>
    /// Capability modifies amount
    /// </summary>
    AmountModifier = 3,

    /// <summary>
    /// Capability modifies quota
    /// </summary>
    QuotaModifier = 4,

    /// <summary>
    /// Capability modifies dispatch priority
    /// </summary>
    PriorityModifier = 5
}
