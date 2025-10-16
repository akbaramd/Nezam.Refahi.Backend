using Nezam.Refahi.Facilities.Domain.Exceptions;

namespace Nezam.Refahi.Facilities.Domain.Exceptions;

/// <summary>
/// استثنای دامنه تسهیلات
/// </summary>
public class FacilityDomainException : Exception
{
    public string ErrorCode { get; }
    public Dictionary<string, object> Details { get; }

    public FacilityDomainException(string errorCode, string message, Dictionary<string, object>? details = null)
        : base(message)
    {
        ErrorCode = errorCode;
        Details = details ?? new Dictionary<string, object>();
    }

    public FacilityDomainException(string errorCode, string message, Exception innerException, Dictionary<string, object>? details = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Details = details ?? new Dictionary<string, object>();
    }
}

/// <summary>
/// استثنای نقض کول‌داون
/// </summary>
public class CooldownViolationException : FacilityDomainException
{
    public DateTime LastDisbursementDate { get; }
    public int RequiredCooldownDays { get; }
    public DateTime CooldownEndDate { get; }

    public CooldownViolationException(DateTime lastDisbursementDate, int requiredCooldownDays)
        : base(
            FacilityErrorCodes.COOLDOWN_VIOLATION,
            FacilityErrorMessages.COOLDOWN_VIOLATION,
            new Dictionary<string, object>
            {
                ["LastDisbursementDate"] = lastDisbursementDate,
                ["RequiredCooldownDays"] = requiredCooldownDays,
                ["CooldownEndDate"] = lastDisbursementDate.AddDays(requiredCooldownDays)
            })
    {
        LastDisbursementDate = lastDisbursementDate;
        RequiredCooldownDays = requiredCooldownDays;
        CooldownEndDate = lastDisbursementDate.AddDays(requiredCooldownDays);
    }
}

/// <summary>
/// استثنای نقض انحصار
/// </summary>
public class ExclusiveSetViolationException : FacilityDomainException
{
    public string ExclusiveSetId { get; }
    public Guid ConflictingApplicationId { get; }

    public ExclusiveSetViolationException(string exclusiveSetId, Guid conflictingApplicationId)
        : base(
            FacilityErrorCodes.EXCLUSIVE_CONFLICT,
            FacilityErrorMessages.EXCLUSIVE_CONFLICT,
            new Dictionary<string, object>
            {
                ["ExclusiveSetId"] = exclusiveSetId,
                ["ConflictingApplicationId"] = conflictingApplicationId
            })
    {
        ExclusiveSetId = exclusiveSetId;
        ConflictingApplicationId = conflictingApplicationId;
    }
}

/// <summary>
/// استثنای نقض سهمیه
/// </summary>
public class QuotaExceededException : FacilityDomainException
{
    public int RequestedQuota { get; }
    public int AvailableQuota { get; }
    public int TotalQuota { get; }

    public QuotaExceededException(int requestedQuota, int availableQuota, int totalQuota)
        : base(
            FacilityErrorCodes.QUOTA_EXCEEDED,
            FacilityErrorMessages.QUOTA_EXCEEDED,
            new Dictionary<string, object>
            {
                ["RequestedQuota"] = requestedQuota,
                ["AvailableQuota"] = availableQuota,
                ["TotalQuota"] = totalQuota
            })
    {
        RequestedQuota = requestedQuota;
        AvailableQuota = availableQuota;
        TotalQuota = totalQuota;
    }
}

/// <summary>
/// استثنای نقض محدوده مبلغ
/// </summary>
public class AmountViolationException : FacilityDomainException
{
    public decimal RequestedAmount { get; }
    public decimal? MinAmount { get; }
    public decimal? MaxAmount { get; }

    public AmountViolationException(decimal requestedAmount, decimal? minAmount, decimal? maxAmount)
        : base(
            minAmount.HasValue && requestedAmount < minAmount.Value ? FacilityErrorCodes.AMOUNT_BELOW_MINIMUM : FacilityErrorCodes.AMOUNT_ABOVE_MAXIMUM,
            minAmount.HasValue && requestedAmount < minAmount.Value ? FacilityErrorMessages.AMOUNT_BELOW_MINIMUM : FacilityErrorMessages.AMOUNT_ABOVE_MAXIMUM,
            new Dictionary<string, object>
            {
                ["RequestedAmount"] = requestedAmount,
                ["MinAmount"] = minAmount ?? 0,
                ["MaxAmount"] = maxAmount ?? 0
            })
    {
        RequestedAmount = requestedAmount;
        MinAmount = minAmount;
        MaxAmount = maxAmount;
    }
}

/// <summary>
/// استثنای نقض ویژگی/قابلیت
/// </summary>
public class FeatureCapabilityViolationException : FacilityDomainException
{
    public string FeatureOrCapabilityId { get; }
    public string ViolationType { get; } // Required, Blacklisted, Conflict

    public FeatureCapabilityViolationException(string featureOrCapabilityId, string violationType)
        : base(
            violationType switch
            {
                "Required" => FacilityErrorCodes.FEATURE_REQUIRED_MISSING,
                "Blacklisted" => FacilityErrorCodes.FEATURE_BLACKLISTED,
                "Conflict" => FacilityErrorCodes.FEATURE_CAPABILITY_CONFLICT,
                _ => FacilityErrorCodes.BUSINESS_RULE_VIOLATION
            },
            violationType switch
            {
                "Required" => FacilityErrorMessages.FEATURE_REQUIRED_MISSING,
                "Blacklisted" => FacilityErrorMessages.FEATURE_BLACKLISTED,
                "Conflict" => FacilityErrorMessages.FEATURE_CAPABILITY_CONFLICT,
                _ => FacilityErrorMessages.BUSINESS_RULE_VIOLATION
            },
            new Dictionary<string, object>
            {
                ["FeatureOrCapabilityId"] = featureOrCapabilityId,
                ["ViolationType"] = violationType
            })
    {
        FeatureOrCapabilityId = featureOrCapabilityId;
        ViolationType = violationType;
    }
}

/// <summary>
/// استثنای نقض وضعیت دوره
/// </summary>
public class CycleStateViolationException : FacilityDomainException
{
    public Guid CycleId { get; }
    public string CurrentStatus { get; }
    public string RequiredStatus { get; }

    public CycleStateViolationException(Guid cycleId, string currentStatus, string requiredStatus)
        : base(
            FacilityErrorCodes.CYCLE_NOT_ACTIVE,
            FacilityErrorMessages.CYCLE_NOT_ACTIVE,
            new Dictionary<string, object>
            {
                ["CycleId"] = cycleId,
                ["CurrentStatus"] = currentStatus,
                ["RequiredStatus"] = requiredStatus
            })
    {
        CycleId = cycleId;
        CurrentStatus = currentStatus;
        RequiredStatus = requiredStatus;
    }
}

/// <summary>
/// استثنای نقض وضعیت درخواست
/// </summary>
public class ApplicationStateViolationException : FacilityDomainException
{
    public Guid ApplicationId { get; }
    public string CurrentStatus { get; }
    public string RequiredStatus { get; }

    public ApplicationStateViolationException(Guid applicationId, string currentStatus, string requiredStatus)
        : base(
            FacilityErrorCodes.STATE_TRANSITION_INVALID,
            FacilityErrorMessages.STATE_TRANSITION_INVALID,
            new Dictionary<string, object>
            {
                ["ApplicationId"] = applicationId,
                ["CurrentStatus"] = currentStatus,
                ["RequiredStatus"] = requiredStatus
            })
    {
        ApplicationId = applicationId;
        CurrentStatus = currentStatus;
        RequiredStatus = requiredStatus;
    }
}

/// <summary>
/// استثنای نقض Idempotency
/// </summary>
public class IdempotencyViolationException : FacilityDomainException
{
    public string IdempotencyKey { get; }
    public Guid? ExistingApplicationId { get; }

    public IdempotencyViolationException(string idempotencyKey, Guid? existingApplicationId = null)
        : base(
            FacilityErrorCodes.IDEMPOTENCY_KEY_EXISTS,
            FacilityErrorMessages.IDEMPOTENCY_KEY_EXISTS,
            new Dictionary<string, object>
            {
                ["IdempotencyKey"] = idempotencyKey,
                ["ExistingApplicationId"] = existingApplicationId ?? Guid.Empty
            })
    {
        IdempotencyKey = idempotencyKey;
        ExistingApplicationId = existingApplicationId;
    }
}
