using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.AddRole;

/// <summary>
/// Command to add a role to a user
/// </summary>
public class AddRoleToUserCommand : IRequest<ApplicationResult>
{
    /// <summary>
    /// User ID to add role to
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Role ID to assign to the user
    /// </summary>
    public Guid RoleId { get; set; }
    
    /// <summary>
    /// Optional expiration date for the role assignment
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// User ID of who is assigning this role (for audit)
    /// </summary>
    public Guid? AssignedBy { get; set; }
    
    /// <summary>
    /// Optional notes for audit purposes
    /// </summary>
    public string? Notes { get; set; }
}