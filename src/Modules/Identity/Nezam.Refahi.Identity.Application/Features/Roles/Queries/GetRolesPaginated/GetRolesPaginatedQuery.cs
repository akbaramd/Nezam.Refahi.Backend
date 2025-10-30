using MCA.SharedKernel.Domain.Models;
using MediatR;
using Nezam.Refahi.Identity.Domain.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetRolesPaginated;

/// <summary>
/// Query to get roles with pagination and filtering
/// </summary>
public class GetRolesPaginatedQuery : IRequest<ApplicationResult<PaginatedResult<RoleDto>>>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Search term for role name or description
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }
    
    /// <summary>
    /// Filter by system role status
    /// </summary>
    public bool? IsSystemRole { get; set; }
    
    /// <summary>
    /// Sort field (Name, DisplayOrder, CreatedAt)
    /// </summary>
    public string SortBy { get; set; } = "DisplayOrder";
    
    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}