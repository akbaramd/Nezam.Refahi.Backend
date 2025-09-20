using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.RemoveClaims;

/// <summary>
/// Command to remove claims from a role
/// </summary>
public class RemoveClaimsFromRoleCommand : IRequest<ApplicationResult>
{
    /// <summary>
    /// Role ID to remove claims from
    /// </summary>
    public Guid RoleId { get; set; }
    
    /// <summary>
    /// List of claim values to remove from the role
    /// </summary>
    public List<string> ClaimValues { get; set; } = new();
}