using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.AddClaims;

/// <summary>
/// Handler for adding claims to a role
/// </summary>
public class AddClaimsToRoleCommandHandler : IRequestHandler<AddClaimsToRoleCommand, ApplicationResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IClaimsAggregatorService _claimsAggregatorService;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ILogger<AddClaimsToRoleCommandHandler> _logger;

    public AddClaimsToRoleCommandHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        IClaimsAggregatorService claimsAggregatorService,
        IIdentityUnitOfWork unitOfWork,
        ILogger<AddClaimsToRoleCommandHandler> logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _roleClaimRepository = roleClaimRepository ?? throw new ArgumentNullException(nameof(roleClaimRepository));
        _claimsAggregatorService = claimsAggregatorService ?? throw new ArgumentNullException(nameof(claimsAggregatorService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult> Handle(AddClaimsToRoleCommand request, CancellationToken cancellationToken)
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

            // 2. Get all available claims from aggregated providers
            var availableClaims = await _claimsAggregatorService.GetAllDistinctClaimsAsync(cancellationToken);
            var availableClaimsDict = availableClaims.ToDictionary(c => c.Value, c => c, StringComparer.OrdinalIgnoreCase);

            // 3. Validate that all requested claim values exist
            var invalidClaimValues = request.ClaimValues
                .Where(cv => !availableClaimsDict.ContainsKey(cv))
                .ToList();

            if (invalidClaimValues.Any())
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure(
                    $"Invalid claim values: {string.Join(", ", invalidClaimValues)}. " +
                    "These claims are not available from registered claim providers.");
            }

            // 4. Get existing claims to avoid duplicates
            var existingRoleClaims = await _roleClaimRepository.GetByRoleIdAsync(request.RoleId, cancellationToken:cancellationToken);
            var existingClaimValues = existingRoleClaims.Select(rc => rc.Claim.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            
            // 5. Filter out claims that already exist
            var claimsToAdd = request.ClaimValues
                .Where(cv => !existingClaimValues.Contains(cv))
                .ToList();
            
            var skippedClaims = request.ClaimValues
                .Where(cv => existingClaimValues.Contains(cv))
                .ToList();

            if (!claimsToAdd.Any())
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("All specified claims are already assigned to this role");
            }

            // 6. Create new RoleClaim entities
            var addedClaims = new List<string>();
            foreach (var claimValue in claimsToAdd)
            {
                var availableClaim = availableClaimsDict[claimValue];
                var claim = new Claim(availableClaim.Type, availableClaim.Value, availableClaim.ValueType);
                
                var roleClaim = new RoleClaim(request.RoleId, claim);
                await _roleClaimRepository.AddAsync(roleClaim, cancellationToken:cancellationToken);
                addedClaims.Add(claimValue);
            }

            // 7. Save changes
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully added {AddedCount} claims to role {RoleId}. Skipped {SkippedCount} existing claims", 
                addedClaims.Count, request.RoleId, skippedClaims.Count);

            var message = $"Successfully added {addedClaims.Count} claims to role";
            if (skippedClaims.Any())
            {
                message += $" ({skippedClaims.Count} claims were already assigned)";
            }

            return ApplicationResult.Success(message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to add claims to role {RoleId}", request.RoleId);
            return ApplicationResult.Failure("Failed to add claims to role. Please try again.");
        }
    }
}