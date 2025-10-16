using MediatR;

namespace Nezam.Refahi.Identity.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a user successfully logs in
/// Other contexts can subscribe to this event to sync user data with external systems
/// </summary>
public class UserLoggedInIntegrationEvent : INotification
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
    public DateTime LoggedInAt { get; set; }
    public string CorrelationId { get; set; } = null!;
    public string? IdempotencyKey { get; set; }
    public string DeviceId { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
    public string UserAgent { get; set; } = null!;
    public string Scope { get; set; } = null!;

    // User claims and roles
    public Dictionary<string, string> Claims { get; set; } = new();
    public List<string> Roles { get; set; } = new();
}
