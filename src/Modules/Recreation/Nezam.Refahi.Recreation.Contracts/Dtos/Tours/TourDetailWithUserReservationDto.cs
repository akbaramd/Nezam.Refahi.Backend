namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Detailed Tour DTO enriched with the current user's reservation (summary or row model).
/// Uses ReservationDto for flexibility without heavy nested collections.
/// </summary>
public sealed class TourDetailWithUserReservationDto : TourDetailDto
{
    public ReservationDto? Reservation { get; set; } = null;
}


