using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.RemoveClaims;

/// <summary>
/// Handler for removing claims from a role
/// </summary>
public class RemoveClaimsFromRoleCommandHandler : IRequestHandler<RemoveClaimsFromRoleCommand, ApplicationResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveClaimsFromRoleCommandHandler> _logger;

    public RemoveClaimsFromRoleCommandHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        IIdentityUnitOfWork unitOfWork,
        ILogger<RemoveClaimsFromRoleCommandHandler> logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _roleClaimRepository = roleClaimRepository ?? throw new ArgumentNullException(nameof(roleClaimRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult> Handle(RemoveClaimsFromRoleCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);

        try
        {
            // 1. Find the role
            var role = await _roleRepository.FindOneAsync(x=>x.Id==request.RoleId);
            if (role == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("Role not found");
            }

            // 2. Get existing role claims to remove
            var existingRoleClaims = await _roleClaimRepository.GetByRoleIdAsync(request.RoleId, cancellationToken:cancellationToken);
            
            // 3. Find claims to remove by matching claim values
            var claimsToRemove = existingRoleClaims
                .Where(rc => request.ClaimValues.Contains(rc.Claim.Value, StringComparer.OrdinalIgnoreCase))
                .ToList();
                
            var notFoundClaims = request.ClaimValues
                .Where(cv => !existingRoleClaims.Any(rc => rc.Claim.Value.Equals(cv, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (!claimsToRemove.Any())
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("None of the specified claims were found in this role");
            }

            // 4. Remove the role claims
            var removedClaims = new List<string>();
            foreach (var roleClaim in claimsToRemove)
            {
                await _roleClaimRepository.DeleteAsync(roleClaim, cancellationToken:cancellationToken);
                removedClaims.Add(roleClaim.Claim.Value);
            }

            // 5. Save changes
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully removed {RemovedCount} claims from role {RoleId}. {NotFoundCount} claims were not found", 
                removedClaims.Count, request.RoleId, notFoundClaims.Count);

            var message = $"Successfully removed {removedClaims.Count} claims from role";
            if (notFoundClaims.Any())
            {
                message += $" ({notFoundClaims.Count} claims were not found in the role)";
            }

            return ApplicationResult.Success(message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to remove claims from role {RoleId}", request.RoleId);
            return ApplicationResult.Failure("Failed to remove claims from role. Please try again.");
        }
    }
}