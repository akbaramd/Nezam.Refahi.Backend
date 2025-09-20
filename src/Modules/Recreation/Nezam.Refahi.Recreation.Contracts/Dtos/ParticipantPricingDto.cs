namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Pricing details for individual participant
/// </summary>
public class ParticipantPricingDto
{
  public Guid ParticipantId { get; init; }
  public string FullName { get; init; } = null!;
  public string NationalNumber { get; init; } = null!;
  public string ParticipantType { get; init; } = null!;
  public decimal RequiredAmount { get; init; }
  public decimal PaidAmount { get; init; }
  public decimal RemainingAmount { get; init; }
  public bool IsFullyPaid { get; init; }
  public DateTime? PaymentDate { get; init; }
  public DateTime RegistrationDate { get; init; }
}