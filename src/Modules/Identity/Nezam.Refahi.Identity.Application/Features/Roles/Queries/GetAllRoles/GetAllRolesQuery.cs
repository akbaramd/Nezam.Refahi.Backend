using MediatR;
using Nezam.Refahi.Identity.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetAllRoles;

/// <summary>
/// Query to get all roles
/// </summary>
public class GetAllRolesQuery : IRequest<ApplicationResult<List<RoleDto>>>
{
    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }
    
    /// <summary>
    /// Filter by system role status
    /// </summary>
    public bool? IsSystemRole { get; set; }
    
    /// <summary>
    /// Include user count in results
    /// </summary>
    public bool IncludeUserCount { get; set; } = false;
    
    /// <summary>
    /// Include claims in results
    /// </summary>
    public bool IncludeClaims { get; set; } = false;
}