using MediatR;

namespace Nezam.Refahi.Identity.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a user is created in Identity context
/// Other contexts can subscribe to this event to create their own entities
/// Enhanced with seeding metadata and idempotency support
/// </summary>
public class UserCreatedIntegrationEvent : INotification
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? NationalId { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string? Email { get; set; }
    public string? Username { get; set; }
    public Guid? ExternalUserId { get; set; }
    public string? SourceSystem { get; set; }
    public string? SourceVersion { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CorrelationId { get; set; } = null!;
    public string? IdempotencyKey { get; set; }
    public bool IsSeedingOperation { get; set; } = false;

    // get claims
    public Dictionary<string, string> Claims { get; set; } = new();
}