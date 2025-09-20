using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.UpdateRole;

/// <summary>
/// Command to update an existing role
/// </summary>
public class UpdateRoleCommand : IRequest<ApplicationResult>
{
    /// <summary>
    /// Role ID to update
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Updated role name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Updated role description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Updated display order
    /// </summary>
    public int DisplayOrder { get; set; }
    
 
}