using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Represents an idempotency key for API operations to prevent duplicate requests
/// </summary>
public sealed class ApiIdempotency : Entity<Guid>
{
    public string IdempotencyKey { get; private set; } = null!;
    public string Endpoint { get; private set; } = null!;
    public string? RequestPayloadHash { get; private set; }
    public string? ResponseData { get; private set; }
    public int StatusCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsProcessed { get; private set; }
    
    // Multi-tenancy support
    public string? TenantId { get; private set; }
    
    // User context
    public string? UserId { get; private set; }
    public string? UserAgent { get; private set; }
    public string? ClientIp { get; private set; }
    public string? CorrelationId { get; private set; }

    // Private constructor for EF Core
    private ApiIdempotency() : base() { }

    /// <summary>
    /// Creates a new idempotency record
    /// </summary>
    public ApiIdempotency(
        string idempotencyKey,
        string endpoint,
        string? requestPayloadHash = null,
        string? tenantId = null,
        string? userId = null,
        string? userAgent = null,
        string? clientIp = null,
        string? correlationId = null,
        TimeSpan? ttl = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("Idempotency key cannot be empty", nameof(idempotencyKey));
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new ArgumentException("Endpoint cannot be empty", nameof(endpoint));

        IdempotencyKey = idempotencyKey.Trim();
        Endpoint = endpoint.Trim();
        RequestPayloadHash = requestPayloadHash?.Trim();
        TenantId = tenantId;
        UserId = userId;
        UserAgent = userAgent?.Trim();
        ClientIp = clientIp?.Trim();
        CorrelationId = correlationId?.Trim();
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.Add(ttl ?? TimeSpan.FromHours(24)); // Default 24 hours TTL
        IsProcessed = false;
        StatusCode = 0;
    }

    /// <summary>
    /// Marks the request as processed with response data
    /// </summary>
    public void MarkAsProcessed(int statusCode, string? responseData = null)
    {
        IsProcessed = true;
        StatusCode = statusCode;
        ResponseData = responseData?.Trim();
    }

    /// <summary>
    /// Checks if the idempotency record is expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>
    /// Checks if the request payload matches
    /// </summary>
    public bool MatchesPayload(string? payloadHash)
    {
        if (string.IsNullOrWhiteSpace(RequestPayloadHash) && string.IsNullOrWhiteSpace(payloadHash))
            return true;
        
        return string.Equals(RequestPayloadHash, payloadHash, StringComparison.Ordinal);
    }

    /// <summary>
    /// Gets the composite key for uniqueness
    /// </summary>
    public string GetCompositeKey() => $"{TenantId ?? "default"}:{Endpoint}:{IdempotencyKey}";
}
