using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ConfirmReservation;

/// <summary>
/// Command to confirm a tour reservation after payment
/// </summary>
public class ConfirmReservationCommand : IRequest<ApplicationResult>
{
    /// <summary>
    /// The ID of the reservation to confirm
    /// </summary>
    public Guid ReservationId { get; set; }

    /// <summary>
    /// Total amount paid in Rials
    /// </summary>
    public long? TotalAmountRials { get; set; }

    /// <summary>
    /// Payment reference or transaction ID
    /// </summary>
    public string? PaymentReference { get; set; }

   

    public ConfirmReservationCommand(Guid reservationId, long? totalAmountRials = null, string? paymentReference = null)
    {
        ReservationId = reservationId;
        TotalAmountRials = totalAmountRials;
        PaymentReference = paymentReference;
    }

    public ConfirmReservationCommand() { }
}