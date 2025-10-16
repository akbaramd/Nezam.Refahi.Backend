using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Domain.Dtos;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetRolesPaginated;

/// <summary>
/// Handler for getting roles with pagination
/// </summary>
public class GetRolesPaginatedQueryHandler : IRequestHandler<GetRolesPaginatedQuery, ApplicationResult<PaginatedResult<RoleDto>>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetRolesPaginatedQueryHandler> _logger;

    public GetRolesPaginatedQueryHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        IUserRoleRepository userRoleRepository,
        ILogger<GetRolesPaginatedQueryHandler> logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _roleClaimRepository = roleClaimRepository ?? throw new ArgumentNullException(nameof(roleClaimRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<PaginatedResult<RoleDto>>> Handle(GetRolesPaginatedQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get all roles first (for simple filtering and sorting)
            var allRoles = await _roleRepository.FindAsync(x=>true);
            
            // Apply filters
            var filteredRoles = allRoles.AsQueryable();
            
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                filteredRoles = filteredRoles.Where(r => 
                    r.Name.ToLower().Contains(searchTerm) || 
                    (r.Description != null && r.Description.ToLower().Contains(searchTerm)));
            }
            
            if (request.IsActive.HasValue)
            {
                filteredRoles = filteredRoles.Where(r => r.IsActive == request.IsActive.Value);
            }
            
            if (request.IsSystemRole.HasValue)
            {
                filteredRoles = filteredRoles.Where(r => r.IsSystemRole == request.IsSystemRole.Value);
            }
            
            // Apply sorting
            filteredRoles = request.SortBy.ToLower() switch
            {
                "name" => request.SortDirection.ToLower() == "desc" 
                    ? filteredRoles.OrderByDescending(r => r.Name)
                    : filteredRoles.OrderBy(r => r.Name),
                "createdat" => request.SortDirection.ToLower() == "desc" 
                    ? filteredRoles.OrderByDescending(r => r.CreatedAt)
                    : filteredRoles.OrderBy(r => r.CreatedAt),
                "modifiedat" => request.SortDirection.ToLower() == "desc" 
                    ? filteredRoles.OrderByDescending(r => r.LastModifiedAt)
                    : filteredRoles.OrderBy(r => r.LastModifiedAt),
                _ => request.SortDirection.ToLower() == "desc" 
                    ? filteredRoles.OrderByDescending(r => r.DisplayOrder)
                    : filteredRoles.OrderBy(r => r.DisplayOrder)
            };
            
            // Get total count
            var totalCount = filteredRoles.Count();
            
            // Apply pagination
            var pagedRoles = filteredRoles
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            // Get role claims and user counts for the paged results
            var roleDtos = new List<RoleDto>();
            
            foreach (var role in pagedRoles)
            {
                var roleClaims = await _roleClaimRepository.GetByRoleIdAsync(role.Id, cancellationToken:cancellationToken);
                var activeUserCount = await _userRoleRepository.GetActiveCountByRoleIdAsync(role.Id, cancellationToken:cancellationToken);
                
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    IsSystemRole = role.IsSystemRole,
                    DisplayOrder = role.DisplayOrder,
                    Claims = roleClaims.Select(rc => new RoleClaimDto
                    {
                        Id = rc.Id,
                        RoleId = rc.RoleId,
                       Claim = new ClaimDto()
                       {
                         Type = rc.Claim.Type,
                         Value = rc.Claim.Value,
                         ValueType = rc.Claim.ValueType,
                     
                       }
                    }).ToList(),
                    CreatedAt = role.CreatedAt,
                    CreatedBy = role.CreatedBy,
                    UpdatedAt = role.LastModifiedAt,
                    UpdatedBy = role.LastModifiedBy,
                    ActiveUserCount = activeUserCount
                };
                
                roleDtos.Add(roleDto);
            }
            
            var result = new PaginatedResult<RoleDto>
            {
                Items = roleDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
            
            return ApplicationResult<PaginatedResult<RoleDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get paginated roles");
            return ApplicationResult<PaginatedResult<RoleDto>>.Failure("Failed to retrieve roles. Please try again.");
        }
    }
}