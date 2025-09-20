using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.DeleteRole;

/// <summary>
/// Command to delete a role
/// </summary>
public class DeleteRoleCommand : IRequest<ApplicationResult>
{
    /// <summary>
    /// Role ID to delete
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Force delete even if users are assigned (deactivates instead)
    /// </summary>
    public bool ForceDelete { get; set; } = false;
}