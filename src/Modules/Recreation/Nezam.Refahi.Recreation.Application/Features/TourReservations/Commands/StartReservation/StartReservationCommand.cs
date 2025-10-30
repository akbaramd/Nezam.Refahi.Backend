using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.StartReservation;

public class StartReservationCommand : IRequest<ApplicationResult<StartReservationCommandResult>>
{
  public Guid TourId { get; set; }
  public Guid  CapacityId { get; set; }
  public string UserNationalNumber { get; set; } = string.Empty;
}
public class StartReservationCommandResult 
{
  public Guid ReservationId { get; set; }
}
