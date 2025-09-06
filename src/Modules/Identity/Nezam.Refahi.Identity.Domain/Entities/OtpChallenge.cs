using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Identity.Domain.Enums;
using Nezam.Refahi.Identity.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Domain.Entities;

/// <summary>
/// Represents an OTP challenge for phone number verification
/// </summary>
public class OtpChallenge : Entity<Guid>
{
    /// <summary>
    /// Unique identifier for the challenge
    /// </summary>
    public string ChallengeId { get; private set; } = string.Empty;
    
    /// <summary>
    /// Phone number for which OTP is requested
    /// </summary>
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    
    /// <summary>
    /// Client identifier for this challenge
    /// </summary>
    public ClientId ClientId { get; private set; } = null!;

    /// <summary>
    /// When the challenge was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// Device fingerprint for this challenge
    /// </summary>
    public DeviceFingerprint? DeviceFingerprint { get; private set; }
    
    /// <summary>
    /// IP address from which the challenge was requested
    /// </summary>
    public IpAddress? IpAddress { get; private set; }
    
    /// <summary>
    /// OTP policy applied to this challenge
    /// </summary>
    public OtpPolicy Policy { get; private set; } = null!;
    
    /// <summary>
    /// Hashed OTP code (not stored in plain text)
    /// </summary>
    public string OtpHash { get; private set; } = string.Empty;
    
    /// <summary>
    /// Nonce used in OTP hash generation
    /// </summary>
    public string Nonce { get; private set; } = string.Empty;
    
    /// <summary>
    /// Current status of the challenge
    /// </summary>
    public ChallengeStatus Status { get; private set; } = ChallengeStatus.Created;
    
    /// <summary>
    /// Delivery status of the OTP
    /// </summary>
    public DeliveryStatus DeliveryStatus { get; private set; } = DeliveryStatus.Pending;
    
    /// <summary>
    /// When the challenge expires
    /// </summary>
    public DateTime ExpiresAt { get; private set; }
    
    /// <summary>
    /// Number of verification attempts remaining
    /// </summary>
    public int AttemptsLeft { get; private set; }
    
    /// <summary>
    /// Number of resend attempts remaining
    /// </summary>
    public int ResendLeft { get; private set; }
    
    /// <summary>
    /// When the OTP was last sent
    /// </summary>
    public DateTime? LastSentAt { get; private set; }
    
    /// <summary>
    /// When the challenge was verified (if successful)
    /// </summary>
    public DateTime? VerifiedAt { get; private set; }
    
    /// <summary>
    /// When the challenge was consumed (if successful)
    /// </summary>
    public DateTime? ConsumedAt { get; private set; }
    
    /// <summary>
    /// When the challenge was locked (if applicable)
    /// </summary>
    public DateTime? LockedAt { get; private set; }
    
    /// <summary>
    /// Reason for locking (if applicable)
    /// </summary>
    public LockReason LockReason { get; private set; } = Enums.LockReason.None;
    
    /// <summary>
    /// User ID if verification was successful
    /// </summary>
    public Guid? UserId { get; private set; }

    // Private constructor for EF Core
    private OtpChallenge() : base() { }

    /// <summary>
    /// Creates a new OTP challenge
    /// </summary>
    public OtpChallenge(
        string challengeId,
        PhoneNumber phoneNumber,
        ClientId clientId,
        string otpHash,
        string nonce,
        OtpPolicy policy,
        DeviceFingerprint? deviceFingerprint = null,
        IpAddress? ipAddress = null) : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(challengeId))
            throw new ArgumentException("Challenge ID cannot be empty", nameof(challengeId));
            
        if (phoneNumber == null)
            throw new ArgumentNullException(nameof(phoneNumber));
            
        if (clientId == null)
            throw new ArgumentNullException(nameof(clientId));
            
        if (string.IsNullOrWhiteSpace(otpHash))
            throw new ArgumentException("OTP hash cannot be empty", nameof(otpHash));
            
        if (string.IsNullOrWhiteSpace(nonce))
            throw new ArgumentException("Nonce cannot be empty", nameof(nonce));
            
        if (policy == null)
            throw new ArgumentNullException(nameof(policy));

        ChallengeId = challengeId;
        PhoneNumber = phoneNumber;
        ClientId = clientId;
        OtpHash = otpHash;
        Nonce = nonce;
        Policy = policy;
        DeviceFingerprint = deviceFingerprint;
        IpAddress = ipAddress;
        
        CreatedAt = DateTime.UtcNow;
        Status = ChallengeStatus.Created;
        DeliveryStatus = DeliveryStatus.Pending;
        ExpiresAt = DateTime.UtcNow.AddSeconds(policy.TtlSeconds);
        AttemptsLeft = policy.MaxVerifyAttempts;
        ResendLeft = policy.MaxResends;
        LockReason = Enums.LockReason.None;
    }

    /// <summary>
    /// Marks the OTP as sent
    /// </summary>
    public void MarkAsSent()
    {
        if (Status != ChallengeStatus.Created)
            throw new InvalidOperationException("Challenge must be in Created status to mark as sent");

        Status = ChallengeStatus.Sent;
        DeliveryStatus = DeliveryStatus.Sent;
        LastSentAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the OTP delivery as failed
    /// </summary>
    public void MarkDeliveryFailed()
    {
        DeliveryStatus = DeliveryStatus.Failed;
    }

    /// <summary>
    /// Attempts to verify the OTP code
    /// </summary>
    /// <param name="otpHash">Hashed OTP code to verify</param>
    /// <param name="userId">User ID if verification succeeds</param>
    /// <returns>True if verification succeeds, false otherwise</returns>
    public bool AttemptVerification(string otpHash, Guid userId)
    {
        if (Status != ChallengeStatus.Sent)
            return false;

        if (DateTime.UtcNow > ExpiresAt)
        {
            Status = ChallengeStatus.Expired;
            return false;
        }

        if (AttemptsLeft <= 0)
        {
            Status = ChallengeStatus.Locked;
            LockedAt = DateTime.UtcNow;
            LockReason = Enums.LockReason.TooManyAttempts;
            return false;
        }

        AttemptsLeft--;

        if (otpHash == OtpHash)
        {
            Status = ChallengeStatus.Verified;
            VerifiedAt = DateTime.UtcNow;
            UserId = userId;
            return true;
        }

        if (AttemptsLeft <= 0)
        {
            Status = ChallengeStatus.Locked;
            LockedAt = DateTime.UtcNow;
            LockReason = Enums.LockReason.TooManyAttempts;
        }

        return false;
    }

    /// <summary>
    /// Consumes the challenge after successful verification
    /// </summary>
    public void Consume()
    {
        if (Status != ChallengeStatus.Verified)
            throw new InvalidOperationException("Challenge must be verified before consumption");

        Status = ChallengeStatus.Consumed;
        ConsumedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Attempts to resend the OTP
    /// </summary>
    /// <param name="newOtpHash">New hashed OTP code</param>
    /// <param name="newNonce">New nonce</param>
    /// <returns>True if resend is allowed, false otherwise</returns>
    public bool AttemptResend(string newOtpHash, string newNonce)
    {
        if (Status != ChallengeStatus.Sent && Status != ChallengeStatus.Created)
            return false;

        if (ResendLeft <= 0)
            return false;

        if (DateTime.UtcNow > ExpiresAt)
        {
            Status = ChallengeStatus.Expired;
            return false;
        }

        ResendLeft--;
        OtpHash = newOtpHash;
        Nonce = newNonce;
        ExpiresAt = DateTime.UtcNow.AddSeconds(Policy.TtlSeconds);
        Status = ChallengeStatus.Sent;
        DeliveryStatus = DeliveryStatus.Sent;
        LastSentAt = DateTime.UtcNow;

        return true;
    }

    /// <summary>
    /// Locks the challenge for security reasons
    /// </summary>
    /// <param name="reason">Reason for locking</param>
    public void Lock(LockReason reason)
    {
        if (Status == ChallengeStatus.Verified || Status == ChallengeStatus.Consumed)
            throw new InvalidOperationException("Cannot lock a verified or consumed challenge");

        Status = ChallengeStatus.Locked;
        LockedAt = DateTime.UtcNow;
        LockReason = reason;
    }

    /// <summary>
    /// Checks if the challenge is expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>
    /// Checks if the challenge can be verified
    /// </summary>
    public bool CanBeVerified => Status == ChallengeStatus.Sent && 
                                !IsExpired && 
                                AttemptsLeft > 0 && 
                                Status != ChallengeStatus.Locked;

    /// <summary>
    /// Checks if the challenge can be resent
    /// </summary>
    public bool CanBeResent => (Status == ChallengeStatus.Sent || Status == ChallengeStatus.Created) && 
                              !IsExpired && 
                              ResendLeft > 0 && 
                              Status != ChallengeStatus.Locked;

    /// <summary>
    /// Gets the remaining time in seconds before expiration
    /// </summary>
    public int RemainingSeconds
    {
        get
        {
            var remaining = ExpiresAt - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? (int)remaining.TotalSeconds : 0;
        }
    }
}
