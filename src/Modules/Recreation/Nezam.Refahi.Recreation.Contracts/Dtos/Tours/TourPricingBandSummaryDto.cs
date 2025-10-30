namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Minimal, per-participant-type pricing band for list ribbons.
/// </summary>
public sealed class TourPricingBandSummaryDto
{
  public string ParticipantType { get; set; } = string.Empty;
  public decimal EffectiveMinPriceRials { get; set; } = 0;
  public bool HasDiscount { get; set; } = false;
}