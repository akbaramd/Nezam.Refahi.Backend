using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Contracts.Dtos;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetAllRoles;

/// <summary>
/// Handler for getting all roles
/// </summary>
public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, ApplicationResult<List<RoleDto>>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetAllRolesQueryHandler> _logger;

    public GetAllRolesQueryHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        IUserRoleRepository userRoleRepository,
        ILogger<GetAllRolesQueryHandler> logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _roleClaimRepository = roleClaimRepository ?? throw new ArgumentNullException(nameof(roleClaimRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<List<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get roles based on filters
            IEnumerable<Domain.Entities.Role> roles;
            
            if (request.IsActive == true)
            {
                roles = await _roleRepository.GetActiveRolesAsync(cancellationToken);
            }
            else
            {
                roles = await _roleRepository.FindAsync(x=>true);
            }
            
            // Apply system role filter if specified
            if (request.IsSystemRole.HasValue)
            {
                if (request.IsSystemRole.Value)
                {
                    roles = roles.Where(r => r.IsSystemRole);
                }
                else
                {
                    roles = roles.Where(r => !r.IsSystemRole);
                }
            }
            
            // Apply active filter if specified and not already applied
            if (request.IsActive.HasValue && request.IsActive != true)
            {
                roles = roles.Where(r => r.IsActive == request.IsActive.Value);
            }
            
            // Sort by display order, then by name
            roles = roles.OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name);
            
            var roleDtos = new List<RoleDto>();
            
            foreach (var role in roles)
            {
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    IsSystemRole = role.IsSystemRole,
                    DisplayOrder = role.DisplayOrder,
                    CreatedAt = role.CreatedAt,
                    CreatedBy = role.CreatedBy,
                    UpdatedAt = role.LastModifiedAt,
                    UpdatedBy = role.LastModifiedBy
                };
                
                // Include claims if requested
                if (request.IncludeClaims)
                {
                    var roleClaims = await _roleClaimRepository.GetByRoleIdAsync(role.Id, cancellationToken:cancellationToken);
                    roleDto.Claims = roleClaims.Select(rc => new RoleClaimDto
                    {
                        Id = rc.Id,
                        RoleId = rc.RoleId,
                        Claim = new ClaimDto
                        {
                            Type = rc.Claim.Type,
                            Value = rc.Claim.Value,
                            ValueType = rc.Claim.ValueType
                        }
                    }).ToList();
                }
                
                // Include user count if requested
                if (request.IncludeUserCount)
                {
                    roleDto.ActiveUserCount = await _userRoleRepository.GetActiveCountByRoleIdAsync(role.Id, cancellationToken:cancellationToken);
                }
                
                roleDtos.Add(roleDto);
            }
            
            return ApplicationResult<List<RoleDto>>.Success(roleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all roles");
            return ApplicationResult<List<RoleDto>>.Failure("Failed to retrieve roles. Please try again.");
        }
    }
}