using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Command to create a user with comprehensive seeding support
/// Single source of truth for user creation with idempotency and external source tracking
/// </summary>
public class CreateUserCommand : IRequest<ApplicationResult<Guid>>
{
    // Core identity fields
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    
    // Optional fields for comprehensive user profiles
    public string? Email { get; set; }
    public string? Username { get; set; }
    
    // External source tracking
    public Guid? ExternalUserId { get; set; }
    public string? SourceSystem { get; set; }
    public string? SourceVersion { get; set; }
    public string? SourceChecksum { get; set; }
    
    // Claims and roles
    public Dictionary<string, string> Claims { get; set; } = new();
    public List<string> Roles { get; set; } = new();
    
    // Profile snapshot for audit and reconciliation
    public string? ProfileSnapshot { get; set; }
    
    // Idempotency and correlation
    public string? IdempotencyKey { get; set; }
    public string? CorrelationId { get; set; }
    
    // Validation flags
    public bool SkipDuplicateCheck { get; set; } = false;
    public bool IsSeedingOperation { get; set; } = false;
}
