using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when an OTP is sent
/// </summary>
public class OtpSentEvent : DomainEvent
{
  public Guid ChallengeId { get; }
  public string PhoneNumber { get; }
  public DateTime SentAt { get; }
  public DateTime OccurredAt { get; }

  public OtpSentEvent(Guid challengeId, string phoneNumber, DateTime sentAt)
  {
    ChallengeId = challengeId;
    PhoneNumber = phoneNumber;
    SentAt = sentAt;
    OccurredAt = DateTime.UtcNow;
  }
}