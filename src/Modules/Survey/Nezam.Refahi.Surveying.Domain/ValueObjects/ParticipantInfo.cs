using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Surveying.Domain.ValueObjects;

/// <summary>
/// Minimal value object representing participant information for survey responses
/// Contains only essential identification data without PII (Personally Identifiable Information)
/// </summary>
public sealed class ParticipantInfo : ValueObject
{
    /// <summary>
    /// Whether this is an anonymous participant
    /// </summary>
    public bool IsAnonymous { get; private set; }

    /// <summary>
    /// Member ID for non-anonymous surveys
    /// </summary>
    public Guid? MemberId { get; private set; }

    /// <summary>
    /// Participant hash for anonymous surveys
    /// </summary>
    public string? ParticipantHash { get; private set; }

    /// <summary>
    /// Creates participant info for a known member
    /// Enforces one-of constraint: MemberId XOR ParticipantHash
    /// </summary>
    public static ParticipantInfo ForMember(Guid memberId)
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));

        var participantInfo = new ParticipantInfo
        {
            IsAnonymous = false,
            MemberId = memberId,
            ParticipantHash = null
        };

        ValidateOneOfConstraint(participantInfo);
        return participantInfo;
    }

    /// <summary>
    /// Creates participant info for anonymous participation
    /// Enforces one-of constraint: MemberId XOR ParticipantHash
    /// </summary>
    public static ParticipantInfo ForAnonymous(string participantHash)
    {
        if (string.IsNullOrWhiteSpace(participantHash))
            throw new ArgumentException("Participant hash cannot be empty", nameof(participantHash));

        var participantInfo = new ParticipantInfo
        {
            IsAnonymous = true,
            MemberId = null,
            ParticipantHash = participantHash.Trim()
        };

        ValidateOneOfConstraint(participantInfo);
        return participantInfo;
    }

    /// <summary>
    /// Validates the one-of constraint: MemberId XOR ParticipantHash
    /// </summary>
    private static void ValidateOneOfConstraint(ParticipantInfo participantInfo)
    {
        if (participantInfo.IsAnonymous)
        {
            if (participantInfo.MemberId.HasValue)
                throw new InvalidOperationException("Anonymous participants cannot have MemberId");
            if (string.IsNullOrWhiteSpace(participantInfo.ParticipantHash))
                throw new InvalidOperationException("Anonymous participants must have ParticipantHash");
        }
        else
        {
            if (!participantInfo.MemberId.HasValue)
                throw new InvalidOperationException("Registered members must have MemberId");
            if (!string.IsNullOrWhiteSpace(participantInfo.ParticipantHash))
                throw new InvalidOperationException("Registered members cannot have ParticipantHash");
        }
    }

    /// <summary>
    /// Gets the participant identifier (either MemberId or ParticipantHash)
    /// </summary>
    public string GetParticipantIdentifier()
    {
        return MemberId?.ToString() ?? ParticipantHash ?? throw new InvalidOperationException("Invalid participant info");
    }

    /// <summary>
    /// Gets display name for the participant
    /// </summary>
    public string GetDisplayName()
    {
        return IsAnonymous ? $"Anonymous ({ParticipantHash?[..8]})" : $"Member {MemberId}";
    }

    /// <summary>
    /// Gets short identifier for the participant (for privacy)
    /// </summary>
    public string GetShortIdentifier()
    {
        if (IsAnonymous)
        {
            if (string.IsNullOrEmpty(ParticipantHash))
                return "Anonymous";
            
            return ParticipantHash.Length >= 8 ? ParticipantHash[..8] : ParticipantHash;
        }
        
        var memberIdString = MemberId?.ToString() ?? "";
        return memberIdString.Length >= 8 ? memberIdString[..8] : memberIdString;
    }

    private ParticipantInfo() { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsAnonymous;
        yield return MemberId ?? Guid.Empty;
        yield return ParticipantHash ?? string.Empty;
    }
}
