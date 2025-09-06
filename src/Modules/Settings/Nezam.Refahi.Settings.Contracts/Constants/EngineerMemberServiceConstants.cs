namespace Nezam.Refahi.Settings.Contracts.Constants;

/// <summary>
/// Constants specifically for Engineer Member Service webhook integration
/// </summary>
public static class EngineerMemberServiceConstants
{
    /// <summary>
    /// API Endpoints for Engineer Member Service
    /// </summary>
    public static class Endpoints
    {
        public const string FindByNationalId = "/api/members/find-by-national-id";
        public const string GetMemberDetails = "/api/members/{id}";
        public const string ValidateMember = "/api/members/validate";
        public const string HealthCheck = "/api/health";
    }

    /// <summary>
    /// HTTP Headers for Engineer Member Service
    /// </summary>
    public static class Headers
    {
        public const string ApiKey = "X-API-Key";
        public const string Authorization = "Authorization";
        public const string ContentType = "Content-Type";
        public const string UserAgent = "User-Agent";
        public const string RequestId = "X-Request-ID";
        public const string Timestamp = "X-Timestamp";
        public const string Signature = "X-Signature";
    }

    /// <summary>
    /// Request/Response Models
    /// </summary>
    public static class Models
    {
        public const string FindByNationalIdRequest = "FindByNationalIdRequest";
        public const string FindByNationalIdResponse = "FindByNationalIdResponse";
        public const string MemberDetailsResponse = "MemberDetailsResponse";
        public const string ValidationRequest = "ValidationRequest";
        public const string ValidationResponse = "ValidationResponse";
        public const string ErrorResponse = "ErrorResponse";
    }

    /// <summary>
    /// Error Codes
    /// </summary>
    public static class ErrorCodes
    {
        public const string MemberNotFound = "MEMBER_NOT_FOUND";
        public const string InvalidNationalId = "INVALID_NATIONAL_ID";
        public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";
        public const string AuthenticationFailed = "AUTHENTICATION_FAILED";
        public const string RateLimitExceeded = "RATE_LIMIT_EXCEEDED";
        public const string InvalidRequest = "INVALID_REQUEST";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    }

    /// <summary>
    /// HTTP Status Codes
    /// </summary>
    public static class StatusCodes
    {
        public const int Success = 200;
        public const int NotFound = 404;
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int TooManyRequests = 429;
        public const int InternalServerError = 500;
        public const int ServiceUnavailable = 503;
    }

    /// <summary>
    /// Timeout and Retry Settings
    /// </summary>
    public static class Timeouts
    {
        public const int DefaultTimeoutSeconds = 30;
        public const int QuickTimeoutSeconds = 10;
        public const int LongTimeoutSeconds = 60;
        public const int MaxRetryCount = 3;
        public const int RetryDelayMs = 1000;
    }

    /// <summary>
    /// Cache Keys
    /// </summary>
    public static class CacheKeys
    {
        public const string MemberByNationalId = "engineer_member_national_id_{0}";
        public const string MemberById = "engineer_member_id_{0}";
        public const string ServiceHealth = "engineer_member_service_health";
        public const string ServiceConfig = "engineer_member_service_config";
    }

    /// <summary>
    /// Cache Expiration Times
    /// </summary>
    public static class CacheExpiration
    {
        public const int MemberDetailsMinutes = 30;
        public const int ServiceHealthMinutes = 5;
        public const int ServiceConfigMinutes = 60;
    }

    /// <summary>
    /// Logging Messages
    /// </summary>
    public static class LogMessages
    {
        public const string ServiceCallStarted = "Engineer Member Service call started for NationalId: {NationalId}";
        public const string ServiceCallCompleted = "Engineer Member Service call completed for NationalId: {NationalId}, Status: {Status}";
        public const string ServiceCallFailed = "Engineer Member Service call failed for NationalId: {NationalId}, Error: {Error}";
        public const string MemberFound = "Engineer member found for NationalId: {NationalId}, MemberId: {MemberId}";
        public const string MemberNotFound = "Engineer member not found for NationalId: {NationalId}";
        public const string ServiceUnavailable = "Engineer Member Service is unavailable";
        public const string ServiceEnabled = "Engineer Member Service is enabled";
        public const string ServiceDisabled = "Engineer Member Service is disabled";
        public const string InvalidConfiguration = "Engineer Member Service configuration is invalid";
    }

    /// <summary>
    /// Validation Rules
    /// </summary>
    public static class Validation
    {
        public const int MinNationalIdLength = 10;
        public const int MaxNationalIdLength = 10;
        public const string NationalIdPattern = @"^\d{10}$";
        public const int MinApiKeyLength = 16;
        public const int MaxApiKeyLength = 128;
    }

    /// <summary>
    /// Feature Flags
    /// </summary>
    public static class Features
    {
        public const string EnableMemberLookup = "ENABLE_ENGINEER_MEMBER_LOOKUP";
        public const string EnableCaching = "ENABLE_ENGINEER_MEMBER_CACHING";
        public const string EnableRetry = "ENABLE_ENGINEER_MEMBER_RETRY";
        public const string EnableLogging = "ENABLE_ENGINEER_MEMBER_LOGGING";
        public const string EnableMetrics = "ENABLE_ENGINEER_MEMBER_METRICS";
    }
}
