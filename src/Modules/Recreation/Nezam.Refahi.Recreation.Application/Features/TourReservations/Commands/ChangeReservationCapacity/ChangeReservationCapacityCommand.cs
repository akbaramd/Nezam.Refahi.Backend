using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ChangeReservationCapacity;

public sealed class ChangeReservationCapacityCommand : IRequest<ApplicationResult<ChangeReservationCapacityCommandResult>>
{
  public Guid   ReservationId      { get; set; }
  public Guid   NewCapacityId      { get; set; }
  public Guid ExternalUserId { get; set; } 
}

public sealed class ChangeReservationCapacityCommandResult
{
  public Guid ReservationId { get; init; }
  public Guid CapacityId    { get; init; }
}