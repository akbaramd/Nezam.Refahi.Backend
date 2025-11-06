using System.Text.Json.Serialization;

namespace Nezam.Refahi.Recreation.Domain.Enums;

/// <summary>
/// Represents the status of a tour.
/// Registration status is determined by registration dates, not by this enum.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TourStatus
{
    /// <summary>
    /// Tour is draft and not published yet
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Tour is published and available (registration status determined by dates)
    /// </summary>
    Published = 1,

    /// <summary>
    /// Tour is currently in progress
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Tour has been completed
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Tour has been cancelled
    /// </summary>
    Cancelled = 4
}