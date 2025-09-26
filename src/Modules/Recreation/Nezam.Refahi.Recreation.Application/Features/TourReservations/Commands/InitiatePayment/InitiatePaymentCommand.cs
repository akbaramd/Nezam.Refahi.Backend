using MediatR;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.InitiatePayment;

/// <summary>
/// Command to initiate payment for a tour reservation
/// </summary>
public record InitiatePaymentCommand : IRequest<ApplicationResult<InitiatePaymentResponse>>
{
    /// <summary>
    /// The ID of the reservation to initiate payment for
    /// </summary>
    public Guid ReservationId { get; init; }

    /// <summary>
    /// Payment method (optional, defaults to system default)
    /// </summary>
    public string? PaymentMethod { get; init; }

    /// <summary>
    /// The ID of the external user initiating the payment
    /// </summary>
    public Guid ExternalUserId { get; init; }
}