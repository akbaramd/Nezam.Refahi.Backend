using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.RemoveRole;

/// <summary>
/// Command to remove a role from a user
/// </summary>
public class RemoveRoleFromUserCommand : IRequest<ApplicationResult>
{
    /// <summary>
    /// User ID to remove role from
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Role ID to remove from the user
    /// </summary>
    public Guid RoleId { get; set; }
    
    /// <summary>
    /// User ID of who is removing this role (for audit)
    /// </summary>
    public Guid? RemovedBy { get; set; }
    
    /// <summary>
    /// Optional reason for removing the role
    /// </summary>
    public string? Reason { get; set; }
}