using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when an OTP challenge is created
/// </summary>
public class OtpChallengeCreatedEvent : DomainEvent
{
    public Guid ChallengeId { get; }
    public string PhoneNumber { get; }
    public string ClientId { get; }
    public DateTime ExpiresAt { get; }
    public DateTime OccurredAt { get; }

    public OtpChallengeCreatedEvent(Guid challengeId, string phoneNumber, string clientId, DateTime expiresAt)
    {
        ChallengeId = challengeId;
        PhoneNumber = phoneNumber;
        ClientId = clientId;
        ExpiresAt = expiresAt;
        OccurredAt = DateTime.UtcNow;
    }
}