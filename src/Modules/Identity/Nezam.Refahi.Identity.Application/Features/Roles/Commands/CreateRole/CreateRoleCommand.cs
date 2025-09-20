using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.CreateRole;

/// <summary>
/// Command to create a new role
/// </summary>
public class CreateRoleCommand : IRequest<ApplicationResult<Guid>>
{
    /// <summary>
    /// Role name (required, unique)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional role description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Whether this is a system role (cannot be deleted by users)
    /// </summary>
    public bool IsSystemRole { get; set; }
    
    /// <summary>
    /// Display order for sorting roles
    /// </summary>
    public int DisplayOrder { get; set; }
}