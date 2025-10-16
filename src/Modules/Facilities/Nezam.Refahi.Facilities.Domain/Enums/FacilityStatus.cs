using System.Text.Json.Serialization;

namespace Nezam.Refahi.Facilities.Domain.Enums;

/// <summary>
/// Status of a facility
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FacilityStatus
{
    /// <summary>
    /// Facility is drafted but not active yet
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Facility is active and available for applications
    /// </summary>
    Active = 2,

    /// <summary>
    /// Facility is temporarily suspended
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// Facility is permanently closed
    /// </summary>
    Closed = 4,

    /// <summary>
    /// Facility is under maintenance
    /// </summary>
    Maintenance = 5
}
