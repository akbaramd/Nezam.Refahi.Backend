using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Domain.Repositories;

/// <summary>
/// Repository interface for participants
/// </summary>
public interface IParticipantRepository : IRepository<Participant, Guid>
{
    /// <summary>
    /// Gets all participants for a specific reservation
    /// </summary>
    Task<IEnumerable<Participant>> GetByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the main participant for a specific reservation
    /// </summary>
    Task<Participant?> GetMainParticipantByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all guest participants for a specific reservation
    /// </summary>
    Task<IEnumerable<Participant>> GetGuestParticipantsByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets participants by reservation ID and participant type
    /// </summary>
    Task<IEnumerable<Participant>> GetByReservationIdAndTypeAsync(Guid reservationId, ParticipantType participantType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets participant by phone number
    /// </summary>
    Task<Participant?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets participants by email
    /// </summary>
    Task<IEnumerable<Participant>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets participants who have paid in a specific reservation
    /// </summary>
    Task<IEnumerable<Participant>> GetPaidParticipantsByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets participants who have not paid in a specific reservation
    /// </summary>
    Task<IEnumerable<Participant>> GetUnpaidParticipantsByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a national number is already registered for a reservation
    /// </summary>
    Task<bool> IsNationalNumberRegisteredForReservationAsync(Guid reservationId, string nationalNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets participant by reservation ID and national number
    /// </summary>
    Task<Participant?> GetByReservationIdAndNationalNumberAsync(Guid reservationId, string nationalNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total participant count for a reservation
    /// </summary>
    Task<int> GetParticipantCountAsync(Guid reservationId, CancellationToken cancellationToken = default);
}