namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.FinalizeReservation;

/// <summary>
/// Response for finalizing a reservation
/// </summary>
public class FinalizeReservationResponse
{
    /// <summary>
    /// Reservation ID
    /// </summary>
    public Guid ReservationId { get; set; }

    /// <summary>
    /// Tracking code for the reservation
    /// </summary>
    public string TrackingCode { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the reservation
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Bill ID created for this reservation
    /// </summary>
    public Guid BillId { get; set; }

    /// <summary>
    /// Bill number
    /// </summary>
    public string BillNumber { get; set; } = string.Empty;

    /// <summary>
    /// Total amount in Rials
    /// </summary>
    public decimal TotalAmountRials { get; set; }

    /// <summary>
    /// Expiry date for the hold
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Number of participants
    /// </summary>
    public int ParticipantCount { get; set; }

    /// <summary>
    /// Tour title
    /// </summary>
    public string TourTitle { get; set; } = string.Empty;
}

