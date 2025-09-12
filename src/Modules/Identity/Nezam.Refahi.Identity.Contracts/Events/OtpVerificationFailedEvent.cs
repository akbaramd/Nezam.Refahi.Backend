using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when OTP verification fails
/// </summary>
public class OtpVerificationFailedEvent : DomainEvent
{
  public Guid ChallengeId { get; }
  public string PhoneNumber { get; }
  public int AttemptsLeft { get; }
  public bool IsLockedAfterFailure { get; }
  public DateTime OccurredAt { get; }

  public OtpVerificationFailedEvent(Guid challengeId, string phoneNumber, int attemptsLeft, bool isLockedAfterFailure = false)
  {
    ChallengeId = challengeId;
    PhoneNumber = phoneNumber;
    AttemptsLeft = attemptsLeft;
    IsLockedAfterFailure = isLockedAfterFailure;
    OccurredAt = DateTime.UtcNow;
  }
}