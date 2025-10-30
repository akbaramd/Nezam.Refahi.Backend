using MediatR;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.AddGuest;

/// <summary>
/// Command to add a guest to an existing reservation
/// </summary>
public class AddGuestToReservationCommand : IRequest<ApplicationResult<AddGuestToReservationResponse>>
{
    /// <summary>
    /// Reservation ID to add guest to
    /// </summary>
    public Guid ReservationId { get; init; }
    public Guid ExternalUserId { get; init; }

    /// <summary>
    /// Guest participant details
    /// </summary>
    public GuestParticipantDto Guest { get; init; } = null!;
}