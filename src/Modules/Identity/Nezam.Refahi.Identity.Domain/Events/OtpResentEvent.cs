using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when an OTP is resent
/// </summary>
public class OtpResentEvent : DomainEvent
{
  public Guid ChallengeId { get; }
  public string PhoneNumber { get; }
  public int ResendsLeft { get; }
  public DateTime ResentAt { get; }
  public DateTime OccurredAt { get; }

  public OtpResentEvent(Guid challengeId, string phoneNumber, int resendsLeft, DateTime resentAt)
  {
    ChallengeId = challengeId;
    PhoneNumber = phoneNumber;
    ResendsLeft = resendsLeft;
    ResentAt = resentAt;
    OccurredAt = DateTime.UtcNow;
  }
}