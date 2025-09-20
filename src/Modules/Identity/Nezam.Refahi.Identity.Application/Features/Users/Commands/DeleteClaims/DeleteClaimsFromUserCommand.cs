using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.DeleteClaims;

/// <summary>
/// Command to remove claims from a user by claim values/keys
/// </summary>
public class DeleteClaimsFromUserCommand : IRequest<ApplicationResult>
{
    /// <summary>
    /// User ID to remove claims from
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// List of claim values/keys to remove from the user
    /// </summary>
    public List<string> ClaimValues { get; set; } = new();
    
    /// <summary>
    /// Optional notes for audit purposes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// User ID of who is removing these claims (for audit)
    /// </summary>
    public Guid? RemovedBy { get; set; }
}