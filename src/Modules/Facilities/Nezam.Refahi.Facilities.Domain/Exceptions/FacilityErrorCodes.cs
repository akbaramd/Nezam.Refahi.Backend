namespace Nezam.Refahi.Facilities.Domain.Exceptions;

/// <summary>
/// کدهای خطای استاندارد دامنه
/// </summary>
public static class FacilityErrorCodes
{
    // Cooldown violations
    public const string COOLDOWN_VIOLATION = "FACILITY_COOLDOWN_VIOLATION";
    public const string COOLDOWN_NOT_SATISFIED = "FACILITY_COOLDOWN_NOT_SATISFIED";
    
    // Exclusive set conflicts
    public const string EXCLUSIVE_CONFLICT = "FACILITY_EXCLUSIVE_CONFLICT";
    public const string EXCLUSIVE_SET_VIOLATION = "FACILITY_EXCLUSIVE_SET_VIOLATION";
    
    // Quota violations
    public const string QUOTA_EXCEEDED = "FACILITY_QUOTA_EXCEEDED";
    public const string QUOTA_REACHED = "FACILITY_QUOTA_REACHED";
    public const string QUOTA_INSUFFICIENT = "FACILITY_QUOTA_INSUFFICIENT";
    
    // Amount violations
    public const string AMOUNT_BELOW_MINIMUM = "FACILITY_AMOUNT_BELOW_MINIMUM";
    public const string AMOUNT_ABOVE_MAXIMUM = "FACILITY_AMOUNT_ABOVE_MAXIMUM";
    public const string AMOUNT_INVALID = "FACILITY_AMOUNT_INVALID";
    
    // Feature/Capability violations
    public const string FEATURE_REQUIRED_MISSING = "FACILITY_FEATURE_REQUIRED_MISSING";
    public const string FEATURE_BLACKLISTED = "FACILITY_FEATURE_BLACKLISTED";
    public const string CAPABILITY_REQUIRED_MISSING = "FACILITY_CAPABILITY_REQUIRED_MISSING";
    public const string CAPABILITY_BLACKLISTED = "FACILITY_CAPABILITY_BLACKLISTED";
    public const string FEATURE_CAPABILITY_CONFLICT = "FACILITY_FEATURE_CAPABILITY_CONFLICT";
    
    // Cycle violations
    public const string CYCLE_NOT_ACTIVE = "FACILITY_CYCLE_NOT_ACTIVE";
    public const string CYCLE_EXPIRED = "FACILITY_CYCLE_EXPIRED";
    public const string CYCLE_NOT_STARTED = "FACILITY_CYCLE_NOT_STARTED";
    public const string CYCLE_CLOSED = "FACILITY_CYCLE_CLOSED";
    
    // Application violations
    public const string APPLICATION_DUPLICATE = "FACILITY_APPLICATION_DUPLICATE";
    public const string APPLICATION_NOT_ELIGIBLE = "FACILITY_APPLICATION_NOT_ELIGIBLE";
    public const string APPLICATION_INVALID_STATE = "FACILITY_APPLICATION_INVALID_STATE";
    public const string APPLICATION_NOT_FOUND = "FACILITY_APPLICATION_NOT_FOUND";
    
    // Document violations
    public const string DOCUMENT_REQUIRED_MISSING = "FACILITY_DOCUMENT_REQUIRED_MISSING";
    public const string DOCUMENT_INVALID = "FACILITY_DOCUMENT_INVALID";
    public const string DOCUMENT_EXPIRED = "FACILITY_DOCUMENT_EXPIRED";
    
    // Bank integration violations
    public const string BANK_DISPATCH_FAILED = "FACILITY_BANK_DISPATCH_FAILED";
    public const string BANK_ACK_TIMEOUT = "FACILITY_BANK_ACK_TIMEOUT";
    public const string BANK_APPOINTMENT_FAILED = "FACILITY_BANK_APPOINTMENT_FAILED";
    public const string BANK_PROCESSING_FAILED = "FACILITY_BANK_PROCESSING_FAILED";
    
    // Policy violations
    public const string POLICY_VERSION_MISMATCH = "FACILITY_POLICY_VERSION_MISMATCH";
    public const string POLICY_NOT_EFFECTIVE = "FACILITY_POLICY_NOT_EFFECTIVE";
    public const string POLICY_CONFLICT = "FACILITY_POLICY_CONFLICT";
    
    // Idempotency violations
    public const string IDEMPOTENCY_KEY_EXISTS = "FACILITY_IDEMPOTENCY_KEY_EXISTS";
    public const string IDEMPOTENCY_KEY_INVALID = "FACILITY_IDEMPOTENCY_KEY_INVALID";
    
    // Concurrency violations
    public const string CONCURRENT_MODIFICATION = "FACILITY_CONCURRENT_MODIFICATION";
    public const string OPTIMISTIC_LOCK_FAILED = "FACILITY_OPTIMISTIC_LOCK_FAILED";
    
    // Validation violations
    public const string VALIDATION_FAILED = "FACILITY_VALIDATION_FAILED";
    public const string REQUIRED_FIELD_MISSING = "FACILITY_REQUIRED_FIELD_MISSING";
    public const string INVALID_FORMAT = "FACILITY_INVALID_FORMAT";
    
    // Business rule violations
    public const string BUSINESS_RULE_VIOLATION = "FACILITY_BUSINESS_RULE_VIOLATION";
    public const string WORKFLOW_VIOLATION = "FACILITY_WORKFLOW_VIOLATION";
    public const string STATE_TRANSITION_INVALID = "FACILITY_STATE_TRANSITION_INVALID";
}

/// <summary>
/// پیام‌های خطای استاندارد
/// </summary>
public static class FacilityErrorMessages
{
    public const string COOLDOWN_VIOLATION = "User is in cooldown period and cannot apply for this facility";
    public const string EXCLUSIVE_CONFLICT = "User has active application in exclusive set";
    public const string QUOTA_EXCEEDED = "Facility quota has been exceeded";
    public const string AMOUNT_BELOW_MINIMUM = "Requested amount is below minimum allowed";
    public const string AMOUNT_ABOVE_MAXIMUM = "Requested amount is above maximum allowed";
    public const string FEATURE_REQUIRED_MISSING = "Required feature is missing";
    public const string FEATURE_BLACKLISTED = "Feature is blacklisted for this facility";
    public const string FEATURE_CAPABILITY_CONFLICT = "Feature/Capability conflict detected";
    public const string CYCLE_NOT_ACTIVE = "Facility cycle is not active";
    public const string APPLICATION_DUPLICATE = "Duplicate application detected";
    public const string DOCUMENT_REQUIRED_MISSING = "Required document is missing";
    public const string BANK_DISPATCH_FAILED = "Failed to dispatch application to bank";
    public const string POLICY_VERSION_MISMATCH = "Policy version mismatch detected";
    public const string IDEMPOTENCY_KEY_EXISTS = "Idempotency key already exists";
    public const string CONCURRENT_MODIFICATION = "Concurrent modification detected";
    public const string VALIDATION_FAILED = "Validation failed";
    public const string BUSINESS_RULE_VIOLATION = "Business rule violation";
    public const string STATE_TRANSITION_INVALID = "Invalid state transition";
}
