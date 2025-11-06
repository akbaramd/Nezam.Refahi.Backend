using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Domain.Repositories;

/// <summary>
/// Repository interface for tour reservations
/// </summary>
public interface ITourReservationRepository : IRepository<TourReservation, Guid>
{
    /// <summary>
    /// Gets all reservations for a specific tour
    /// </summary>
    Task<IEnumerable<TourReservation>> GetByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reservations by tour ID and status
    /// </summary>
    Task<IEnumerable<TourReservation>> GetByTourIdAndStatusAsync(Guid tourId, ReservationStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets confirmed reservations for a tour
    /// </summary>
    Task<IEnumerable<TourReservation>> GetConfirmedReservationsByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pending reservations for a tour
    /// </summary>
    Task<IEnumerable<TourReservation>> GetPendingReservationsByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired reservations that need cleanup
    /// </summary>
    Task<IEnumerable<TourReservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets reservations that have expired beyond the specified cutoff time
    /// </summary>
    Task<IEnumerable<TourReservation>> GetExpiredReservationsAsync(DateTime cutoffTime, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets reservations that are about to expire (for notification purposes)
    /// </summary>
    Task<IEnumerable<TourReservation>> GetExpiringReservationsAsync(DateTime expiryTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reservation by tracking code
    /// </summary>
    Task<TourReservation?> GetByTrackingCodeAsync(string trackingCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total confirmed participant count for a tour
    /// </summary>
    Task<int> GetConfirmedParticipantCountAsync(Guid tourId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total pending participant count for a tour
    /// </summary>
    Task<int> GetPendingParticipantCountAsync(Guid tourId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if tracking code exists
    /// </summary>
    Task<bool> TrackingCodeExistsAsync(string trackingCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reservations by tour ID and participant national number
    /// </summary>
    Task<IEnumerable<TourReservation>> GetByTourIdAndNationalNumberAsync(Guid tourId, string nationalNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reservations by multiple tour IDs and participant national number
    /// </summary>
    Task<IEnumerable<TourReservation>> GetByTourIdsAndNationalNumberAsync(IEnumerable<Guid> tourIds, string nationalNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reservations by multiple tour IDs and the reservation owner (ExternalUserId)
    /// </summary>
    Task<IEnumerable<TourReservation>> GetByTourIdsAndExternalUserIdAsync(IEnumerable<Guid> tourIds, Guid externalUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a reservation by ID including its participants
    /// </summary>
    Task<TourReservation?> GetByIdWithParticipantsAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current utilization (participant count) for a specific capacity
    /// </summary>
    Task<int> GetCapacityUtilizationAsync(Guid capacityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current utilization (participant count) for a tour across all capacities
    /// </summary>
    Task<int> GetTourUtilizationAsync(Guid tourId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets utilization for multiple tours in a single query for better performance
    /// </summary>
    Task<Dictionary<Guid, int>> GetTourUtilizationBatchAsync(IEnumerable<Guid> tourIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets utilization for multiple capacities in a single query for better performance
    /// </summary>
    Task<Dictionary<Guid, int>> GetCapacityUtilizationBatchAsync(IEnumerable<Guid> capacityIds, CancellationToken cancellationToken = default);
}