using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.RemoveGuest;

public sealed class RemoveGuestFromReservationCommand : IRequest<ApplicationResult<RemoveGuestFromReservationResponse>>
{
  public Guid ReservationId { get; init; }
  public Guid ParticipantId { get; init; }              // ID of the participant to remove
  public Guid ExternalUserId { get; init; }             // Owner of the reservation (caller)
}

public sealed class RemoveGuestFromReservationResponse
{
  public Guid ReservationId { get; init; }
  public Guid? CapacityId { get; init; }
}
