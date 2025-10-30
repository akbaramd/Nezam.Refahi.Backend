// =======================
// Contract
// =======================
using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationPricing;

public sealed class GetReservationPricingQuery : IRequest<ApplicationResult<ReservationPricingResponse>>
{
  public Guid ReservationId { get; init; }
}

public sealed class ReservationPricingResponse
{
  public Guid ReservationId { get; init; }
  public decimal TotalRequiredAmount { get; init; }
  public decimal TotalPaidAmount { get; init; }
  public decimal TotalRemainingAmount { get; init; }
  public bool   IsFullyPaid { get; init; }
  public IReadOnlyList<ParticipantPricingDto> Participants { get; init; } = Array.Empty<ParticipantPricingDto>();
}

public sealed class ParticipantPricingDto
{
  public Guid ParticipantId { get; init; }
  public string ParticipantType { get; init; } = string.Empty; // "Member" | "Guest"
  public decimal RequiredAmount { get; init; }
  public decimal PaidAmount { get; init; }
  public decimal RemainingAmount { get; init; }
  public bool   IsFullyPaid { get; init; }
}
