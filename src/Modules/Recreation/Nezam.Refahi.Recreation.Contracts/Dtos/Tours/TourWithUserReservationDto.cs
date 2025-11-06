namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Tour list item enriched with the current user's reservation (summary) if exists.
/// </summary>
public sealed class TourWithUserReservationDto : TourDto
{
    public ReservationSummaryDto? Reservation { get; set; } = null;
}


