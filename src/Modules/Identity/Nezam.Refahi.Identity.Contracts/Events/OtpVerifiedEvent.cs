using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when an OTP is successfully verified
/// </summary>
public class OtpVerifiedEvent : DomainEvent
{
  public Guid ChallengeId { get; }
  public string PhoneNumber { get; }
  public Guid UserId { get; }
  public DateTime VerifiedAt { get; }
  public DateTime OccurredAt { get; }

  public OtpVerifiedEvent(Guid challengeId, string phoneNumber, Guid userId, DateTime verifiedAt)
  {
    ChallengeId = challengeId;
    PhoneNumber = phoneNumber;
    UserId = userId;
    VerifiedAt = verifiedAt;
    OccurredAt = DateTime.UtcNow;
  }
}