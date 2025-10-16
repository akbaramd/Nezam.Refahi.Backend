using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when OTP delivery fails
/// </summary>
public class OtpDeliveryFailedEvent : DomainEvent
{
  public Guid ChallengeId { get; }
  public string PhoneNumber { get; }
  public string? ErrorReason { get; }
  public DateTime OccurredAt { get; }

  public OtpDeliveryFailedEvent(Guid challengeId, string phoneNumber, string? errorReason = null)
  {
    ChallengeId = challengeId;
    PhoneNumber = phoneNumber;
    ErrorReason = errorReason;
    OccurredAt = DateTime.UtcNow;
  }
}