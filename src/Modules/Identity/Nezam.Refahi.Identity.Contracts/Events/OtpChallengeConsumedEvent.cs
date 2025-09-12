using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when an OTP challenge is consumed
/// </summary>
public class OtpChallengeConsumedEvent : DomainEvent
{
  public Guid ChallengeId { get; }
  public string PhoneNumber { get; }
  public Guid UserId { get; }
  public DateTime ConsumedAt { get; }
  public DateTime OccurredAt { get; }

  public OtpChallengeConsumedEvent(Guid challengeId, string phoneNumber, Guid userId, DateTime consumedAt)
  {
    ChallengeId = challengeId;
    PhoneNumber = phoneNumber;
    UserId = userId;
    ConsumedAt = consumedAt;
    OccurredAt = DateTime.UtcNow;
  }
}