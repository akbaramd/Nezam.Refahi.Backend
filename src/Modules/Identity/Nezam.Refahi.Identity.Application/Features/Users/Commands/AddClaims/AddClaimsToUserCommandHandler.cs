using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.AddClaims;

/// <summary>
/// Handler for adding claims to a user by looking up claim keys from aggregated claim providers
/// </summary>
public class AddClaimsToUserCommandHandler : IRequestHandler<AddClaimsToUserCommand, ApplicationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly IClaimsAggregatorService _claimsAggregatorService;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ILogger<AddClaimsToUserCommandHandler> _logger;

    public AddClaimsToUserCommandHandler(
        IUserRepository userRepository,
        IUserClaimRepository userClaimRepository,
        IClaimsAggregatorService claimsAggregatorService,
        IIdentityUnitOfWork unitOfWork,
        ILogger<AddClaimsToUserCommandHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userClaimRepository = userClaimRepository ?? throw new ArgumentNullException(nameof(userClaimRepository));
        _claimsAggregatorService = claimsAggregatorService ?? throw new ArgumentNullException(nameof(claimsAggregatorService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult> Handle(AddClaimsToUserCommand request, CancellationToken cancellationToken)
    {
        if (request.ClaimValues?.Any() != true)
        {
            return ApplicationResult.Failure("هیچ دسترسی برای افزودن ارائه نشده است");
        }

        await _unitOfWork.BeginAsync(cancellationToken);

        try
        {
            // 1. Verify user exists
            var user = await _userRepository.FindOneAsync(x=>x.Id==request.UserId);
            if (user == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("کاربر مورد نظر یافت نشد");
            }

            // 2. Get all available claims from aggregated providers
            var availableClaims = await _claimsAggregatorService.GetAllDistinctClaimsAsync(cancellationToken);
            var availableClaimsDict = availableClaims.ToDictionary(c => c.Value, c => c, StringComparer.OrdinalIgnoreCase);

            _logger.LogDebug("Retrieved {AvailableClaimsCount} available claims from aggregated providers", 
                availableClaimsDict.Count);

            // 3. Validate that all requested claim values exist in available claims
            var invalidClaimValues = request.ClaimValues
                .Where(cv => !availableClaimsDict.ContainsKey(cv))
                .ToList();

            if (invalidClaimValues.Any())
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure(
                    $"دسترسی های نامعتبر: {string.Join(", ", invalidClaimValues)}. " +
                    "این دسترسی ها در سیستم تعریف نشده اند.");
            }

            // 4. Filter out claims the user already has (active ones)
            var existingUserClaims = await _userClaimRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken:cancellationToken);
            var existingClaimValues = existingUserClaims.Select(uc => uc.Claim.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var claimsToAdd = request.ClaimValues
                .Where(cv => !existingClaimValues.Contains(cv))
                .ToList();

            if (!claimsToAdd.Any())
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("کاربر قبلا تمامی دسترسی های درخواستی را دارد");
            }

            // 5. Create and add new UserClaim entities
            var addedClaims = new List<UserClaim>();
            
            foreach (var claimValue in claimsToAdd)
            {
                var availableClaim = availableClaimsDict[claimValue];
                var claim = new Claim(availableClaim.Type, availableClaim.Value, availableClaim.ValueType);
                
                var userClaim = new UserClaim(
                    userId: request.UserId,
                    claim: claim,
                    expiresAt: request.ExpiresAt,
                    assignedBy: request.AssignedBy?.ToString(),
                    notes: request.Notes
                );

                await _userClaimRepository.AddAsync(userClaim, cancellationToken:cancellationToken);
                addedClaims.Add(userClaim);
            }

            // 6. Save changes
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully added {ClaimCount} claims to user {UserId}: {ClaimValues}", 
                addedClaims.Count, request.UserId, string.Join(", ", claimsToAdd));

            var skippedCount = request.ClaimValues.Count - claimsToAdd.Count;
            var message = $"{addedClaims.Count} دسترسی با موفقیت به کاربر اضافه شد";
            if (skippedCount > 0)
            {
                message += $" ({skippedCount} دسترسی قبلا به کاربر اختصاص داده شده بود)";
            }

            return ApplicationResult.Success(message);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Concurrency error while adding claims to user {UserId}", request.UserId);
            return ApplicationResult.Failure("خطای همزمانی: اطلاعات کاربر یا دسترسی ها توسط فرآیند دیگری تغییر کرده است. لطفا مجددا تلاش کنید.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to add claims to user {UserId}", request.UserId);
            return ApplicationResult.Failure("خطا در افزودن دسترسی به کاربر. لطفا مجددا تلاش کنید.");
        }
    }
}
