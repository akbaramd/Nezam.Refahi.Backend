using System.Text.Json.Serialization;

namespace Nezam.Refahi.Facilities.Domain.Enums;

/// <summary>
/// Status of a facility cycle
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FacilityCycleStatus
{
    /// <summary>
    /// Cycle is drafted but not active yet
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Cycle is active and accepting applications
    /// </summary>
    Active = 2,

    /// <summary>
    /// Cycle is closed for new applications
    /// </summary>
    Closed = 3,

    /// <summary>
    /// Cycle is under review - all requests in PendingApproval status will be changed to UnderReview
    /// </summary>
    UnderReview = 4,

    /// <summary>
    /// Cycle is completed
    /// </summary>
    Completed = 5,

    /// <summary>
    /// Cycle is cancelled
    /// </summary>
    Cancelled = 6
}
