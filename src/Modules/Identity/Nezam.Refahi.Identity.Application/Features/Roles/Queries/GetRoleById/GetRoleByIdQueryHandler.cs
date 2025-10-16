using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Domain.Dtos;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetRoleById;

/// <summary>
/// Handler for getting a role by ID
/// </summary>
public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, ApplicationResult<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetRoleByIdQueryHandler> _logger;

    public GetRoleByIdQueryHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        IUserRoleRepository userRoleRepository,
        ILogger<GetRoleByIdQueryHandler> logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _roleClaimRepository = roleClaimRepository ?? throw new ArgumentNullException(nameof(roleClaimRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the role
            var role = await _roleRepository.FindOneAsync(x=>x.Id==request.Id);
            if (role == null)
            {
                return ApplicationResult<RoleDto>.Failure("Role not found");
            }

            // Get role claims
            var roleClaims = await _roleClaimRepository.GetByRoleIdAsync(request.Id, cancellationToken:cancellationToken);
            
            // Get active user count
            var activeUserCount = await _userRoleRepository.GetActiveCountByRoleIdAsync(request.Id, cancellationToken:cancellationToken);

            // Map to DTO
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

            return ApplicationResult<RoleDto>.Success(roleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get role {RoleId}", request.Id);
            return ApplicationResult<RoleDto>.Failure("Failed to retrieve role. Please try again.");
        }
    }
}