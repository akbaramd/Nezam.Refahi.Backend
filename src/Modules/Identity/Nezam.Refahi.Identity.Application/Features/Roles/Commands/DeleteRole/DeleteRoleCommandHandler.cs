using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.DeleteRole;

/// <summary>
/// Handler for deleting a role
/// </summary>
public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ApplicationResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteRoleCommandHandler> _logger;

    public DeleteRoleCommandHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        IUserRoleRepository userRoleRepository,
        IIdentityUnitOfWork unitOfWork,
        ILogger<DeleteRoleCommandHandler> logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _roleClaimRepository = roleClaimRepository ?? throw new ArgumentNullException(nameof(roleClaimRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);

        try
        {
            // 1. Find the role
            var role = await _roleRepository.FindOneAsync(x=>x.Id==request.Id);
            if (role == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("Role not found");
            }

            // 2. Check if it's a system role
            if (role.IsSystemRole)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("System roles cannot be deleted");
            }

            // 3. Check if users are assigned to this role
            var activeUserCount = await _userRoleRepository.GetActiveCountByRoleIdAsync(request.Id, cancellationToken:cancellationToken);
            if (activeUserCount > 0 && !request.ForceDelete)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure($"Cannot delete role. {activeUserCount} users are currently assigned to this role. Use force delete to deactivate the role instead.");
            }

            if (activeUserCount > 0 && request.ForceDelete)
            {
                // Deactivate the role instead of deleting
                role.Deactivate();
                await _roleRepository.UpdateAsync(role, cancellationToken:cancellationToken);
                
                _logger.LogInformation("Deactivated role {RoleId} with name {RoleName} due to {UserCount} active user assignments", 
                    request.Id, role.Name, activeUserCount);
                
                await _unitOfWork.SaveAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                
                return ApplicationResult.Success("Role has been deactivated because users are assigned to it");
            }

            // 4. Delete role claims first
            var roleClaims = await _roleClaimRepository.GetByRoleIdAsync(request.Id, cancellationToken:cancellationToken);
            foreach (var roleClaim in roleClaims)
            {
                await _roleClaimRepository.DeleteAsync(roleClaim, cancellationToken:cancellationToken);
            }

            // 5. Delete the role
            await _roleRepository.DeleteAsync(role, cancellationToken:cancellationToken);

            // 6. Save changes
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully deleted role {RoleId} with name {RoleName}", 
                request.Id, role.Name);

            return ApplicationResult.Success("Role deleted successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to delete role {RoleId}", request.Id);
            return ApplicationResult.Failure("Failed to delete role. Please try again.");
        }
    }
}