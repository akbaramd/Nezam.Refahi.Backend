namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// Result of eligibility validation for a member against tour requirements
/// </summary>
public class MemberEligibilityResult
{
    /// <summary>
    /// National code of the member
    /// </summary>
    public string NationalCode { get; set; } = string.Empty;

    /// <summary>
    /// Whether the member is eligible (meets all requirements)
    /// </summary>
    public bool IsEligible { get; set; }

    /// <summary>
    /// Whether member has active membership
    /// </summary>
    public bool HasActiveMembership { get; set; }

    /// <summary>
    /// Missing required capabilities
    /// </summary>
    public List<string> MissingCapabilities { get; set; } = new();

    /// <summary>
    /// Missing required features
    /// </summary>
    public List<string> MissingFeatures { get; set; } = new();

    /// <summary>
    /// Missing required agencies (member must be associated with at least one of the required agencies)
    /// </summary>
    public List<Guid> MissingAgencies { get; set; } = new();

    /// <summary>
    /// Validation errors (human-readable messages)
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Member details if available
    /// </summary>
    public MemberDetailDto? MemberDetail { get; set; }

    /// <summary>
    /// Creates a success result (member is eligible)
    /// </summary>
    public static MemberEligibilityResult Eligible(string nationalCode, MemberDetailDto? memberDetail = null)
    {
        return new MemberEligibilityResult
        {
            NationalCode = nationalCode,
            IsEligible = true,
            HasActiveMembership = true,
            MemberDetail = memberDetail
        };
    }

    /// <summary>
    /// Creates a failure result with specific reasons
    /// </summary>
    public static MemberEligibilityResult NotEligible(
        string nationalCode,
        bool hasActiveMembership,
        List<string>? missingCapabilities = null,
        List<string>? missingFeatures = null,
        List<Guid>? missingAgencies = null,
        List<string>? errors = null,
        MemberDetailDto? memberDetail = null)
    {
        return new MemberEligibilityResult
        {
            NationalCode = nationalCode,
            IsEligible = false,
            HasActiveMembership = hasActiveMembership,
            MissingCapabilities = missingCapabilities ?? new List<string>(),
            MissingFeatures = missingFeatures ?? new List<string>(),
            MissingAgencies = missingAgencies ?? new List<Guid>(),
            Errors = errors ?? new List<string>(),
            MemberDetail = memberDetail
        };
    }

    /// <summary>
    /// Creates a failure result for member not found
    /// </summary>
    public static MemberEligibilityResult MemberNotFound(string nationalCode)
    {
        return new MemberEligibilityResult
        {
            NationalCode = nationalCode,
            IsEligible = false,
            HasActiveMembership = false,
            Errors = new List<string> { "عضو یافت نشد" }
        };
    }
}

