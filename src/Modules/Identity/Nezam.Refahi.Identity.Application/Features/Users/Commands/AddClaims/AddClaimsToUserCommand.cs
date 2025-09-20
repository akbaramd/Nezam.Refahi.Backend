using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.AddClaims;

/// <summary>
/// Command to add claims to a user by looking up claim keys from aggregated claim providers
/// </summary>
public class AddClaimsToUserCommand : IRequest<ApplicationResult>
{
    /// <summary>
    /// User ID to add claims to
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// List of claim values/keys to lookup and add to the user
    /// These will be validated against available claims from claim providers
    /// </summary>
    public List<string> ClaimValues { get; set; } = new();
    
    /// <summary>
    /// Optional notes for audit purposes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// User ID of who is assigning these claims (for audit)
    /// </summary>
    public Guid? AssignedBy { get; set; }
    
    /// <summary>
    /// Optional expiration date for the claims
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}