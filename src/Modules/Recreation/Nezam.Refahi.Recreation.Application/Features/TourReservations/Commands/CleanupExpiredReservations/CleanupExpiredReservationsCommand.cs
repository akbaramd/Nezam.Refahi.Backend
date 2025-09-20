using MediatR;
using Nezam.Refahi.Shared.Application.Common.Contracts;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CleanupExpiredReservations;

/// <summary>
/// Command to manually trigger cleanup of expired reservations
/// </summary>
public record CleanupExpiredReservationsCommand : IRequest<ApplicationResult<CleanupExpiredReservationsResponse>>
{
    /// <summary>
    /// Optional cutoff time for expiry (default: current time)
    /// </summary>
    public DateTime? CutoffTime { get; init; }

    /// <summary>
    /// Whether to include grace period (default: true)
    /// </summary>
    public bool IncludeGracePeriod { get; init; } = true;

    /// <summary>
    /// Grace period in minutes (default: 2 minutes)
    /// </summary>
    public int GracePeriodMinutes { get; init; } = 2;

    /// <summary>
    /// Whether to also cleanup old idempotency records
    /// </summary>
    public bool CleanupIdempotency { get; init; } = false;

    /// <summary>
    /// Dry run mode - just count what would be cleaned up without making changes
    /// </summary>
    public bool DryRun { get; init; } = false;
}

/// <summary>
/// Response for cleanup command
/// </summary>
public class CleanupExpiredReservationsResponse
{
    /// <summary>
    /// Number of reservations processed/marked as expired
    /// </summary>
    public int ExpiredReservationsCount { get; set; }

    /// <summary>
    /// Number of participants released from capacity
    /// </summary>
    public int ReleasedParticipantsCount { get; set; }

    /// <summary>
    /// Number of idempotency records deleted
    /// </summary>
    public int DeletedIdempotencyRecordsCount { get; set; }

    /// <summary>
    /// Details of processed reservations
    /// </summary>
    public List<ProcessedReservationInfo> ProcessedReservations { get; init; } = new();

    /// <summary>
    /// Any errors encountered during cleanup
    /// </summary>
    public List<string> Errors { get; init; } = new();

    /// <summary>
    /// Whether this was a dry run
    /// </summary>
    public bool WasDryRun { get; init; }
}

/// <summary>
/// Information about a processed reservation
/// </summary>
public class ProcessedReservationInfo
{
    public Guid ReservationId { get; init; }
    public string TrackingCode { get; init; } = null!;
    public string PreviousStatus { get; set; } = null!;
    public string NewStatus { get; set; } = null!;
    public int ParticipantCount { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public Guid? CapacityId { get; init; }
}
