using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ReactivateReservation;

/// <summary>
/// Command to reactivate an expired reservation back to Draft
/// </summary>
public sealed class ReactivateReservationCommand : IRequest<ApplicationResult<ReactivateReservationResponse>>
{
    public Guid ReservationId { get; init; }
    public Guid ExternalUserId { get; init; }
    public string? Reason { get; init; }
}

public sealed class ReactivateReservationResponse
{
    public Guid ReservationId { get; init; }
    public string Status { get; init; } = string.Empty;
}


