using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when an OTP challenge expires
/// </summary>
public class OtpChallengeExpiredEvent : DomainEvent
{
  public Guid ChallengeId { get; }
  public string PhoneNumber { get; }
  public DateTime ExpiredAt { get; }
  public DateTime OccurredAt { get; }

  public OtpChallengeExpiredEvent(Guid challengeId, string phoneNumber, DateTime expiredAt)
  {
    ChallengeId = challengeId;
    PhoneNumber = phoneNumber;
    ExpiredAt = expiredAt;
    OccurredAt = DateTime.UtcNow;
  }
}