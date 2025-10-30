using MediatR;
using Nezam.Refahi.Recreation.Application.Dtos;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CreateReservation;

/// <summary>
/// Command to create a new tour reservation
/// </summary>
public class CreateTourReservationCommand : IRequest<ApplicationResult<CreateTourReservationResponse>>
{
    /// <summary>
    /// Tour ID to make reservation for
    /// </summary>
    public Guid TourId { get; init; }
    
    /// <summary>
    /// Capacity ID to reserve for
    /// </summary>
    public Guid CapacityId { get; init; }

    /// <summary>
    /// Idempotency key to prevent duplicate requests
    /// </summary>
    public string? IdempotencyKey { get; init; } = null!;

    /// <summary>
    /// Guest participants to add to reservation
    /// </summary>
    public IEnumerable<GuestParticipantDto> Guests { get; init; } = [];
    
    /// <summary>
    /// Additional notes for reservation
    /// </summary>
    public string? Notes { get; init; }
}