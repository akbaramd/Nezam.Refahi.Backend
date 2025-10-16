using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when an OTP challenge is locked
/// </summary>
public class OtpChallengeLockedEvent : DomainEvent
{
  public Guid ChallengeId { get; }
  public string PhoneNumber { get; }
  public string LockReason { get; }
  public DateTime LockedAt { get; }
  public DateTime OccurredAt { get; }

  public OtpChallengeLockedEvent(Guid challengeId, string phoneNumber, string lockReason, DateTime lockedAt)
  {
    ChallengeId = challengeId;
    PhoneNumber = phoneNumber;
    LockReason = lockReason;
    LockedAt = lockedAt;
    OccurredAt = DateTime.UtcNow;
  }
}