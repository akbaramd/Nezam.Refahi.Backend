using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.UpdateRole;

/// <summary>
/// Handler for updating an existing role
/// </summary>
public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApplicationResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IClaimsAggregatorService _claimsAggregatorService;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateRoleCommandHandler> _logger;

    public UpdateRoleCommandHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        IClaimsAggregatorService claimsAggregatorService,
        IIdentityUnitOfWork unitOfWork,
        ILogger<UpdateRoleCommandHandler> logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _roleClaimRepository = roleClaimRepository ?? throw new ArgumentNullException(nameof(roleClaimRepository));
        _claimsAggregatorService = claimsAggregatorService ?? throw new ArgumentNullException(nameof(claimsAggregatorService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
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

            // 2. Check if name is being changed and if new name already exists
            if (role.Name != request.Name)
            {
                var existingRoleWithName = await _roleRepository.ExistsByNameAsync(request.Name, request.Id, cancellationToken:cancellationToken);
                if (existingRoleWithName)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult.Failure("A role with this name already exists");
                }
            }

            // 3. Update role details
            role.UpdateDetails(request.Name, request.Description, request.DisplayOrder);

            // 5. Save changes
            await _roleRepository.UpdateAsync(role, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully updated role {RoleId} with name {RoleName}", 
                request.Id, request.Name);

            return ApplicationResult.Success("Role updated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to update role {RoleId}", request.Id);
            return ApplicationResult.Failure("Failed to update role. Please try again.");
        }
    }
}