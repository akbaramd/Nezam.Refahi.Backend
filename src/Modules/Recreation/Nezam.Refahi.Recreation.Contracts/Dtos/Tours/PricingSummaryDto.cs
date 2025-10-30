namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Pricing summary for list screens. No "best/min/max" fields. All amounts are decimal (rials).
/// </summary>
public sealed class PricingSummaryDto
{
  public Guid Id { get; set; } = Guid.Empty;
  public string ParticipantType { get; set; } = string.Empty;
  public decimal BasePriceRials { get; set; } = 0;
  public decimal EffectivePriceRials { get; set; } = 0;
  public decimal? DiscountPercentage { get; set; } = null;
  public DateTime? ValidFrom { get; set; } = null;
  public DateTime? ValidTo { get; set; } = null;
  public bool IsActive { get; set; } = false;
  public bool IsEarlyBird { get; set; } = false;
  public bool IsLastMinute { get; set; } = false;
  public string? Description { get; set; } = string.Empty;
}