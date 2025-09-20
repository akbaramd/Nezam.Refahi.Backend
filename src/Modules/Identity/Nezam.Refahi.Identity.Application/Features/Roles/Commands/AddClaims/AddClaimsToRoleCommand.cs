using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.AddClaims;

/// <summary>
/// Command to add claims to a role
/// </summary>
public class AddClaimsToRoleCommand : IRequest<ApplicationResult>
{
    /// <summary>
    /// Role ID to add claims to
    /// </summary>
    public Guid RoleId { get; set; }
    
    /// <summary>
    /// List of claim values to assign to the role
    /// </summary>
    public List<string> ClaimValues { get; set; } = new();
}