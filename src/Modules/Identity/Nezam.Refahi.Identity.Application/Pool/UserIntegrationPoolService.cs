using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Contracts.Dtos;
using Nezam.Refahi.Identity.Contracts.Pool;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Pool;

/// <summary>
/// Anti-Corruption Layer implementation for integrating external user data into Identity context
/// This service encapsulates external user dependencies and provides Identity-specific operations
/// Implementation can be swapped to use HTTP clients, message brokers, or other communication mechanisms
/// </summary>
public class UserIntegrationPoolService : IUserIntegrationPool
{
    private readonly IMemberService _iMemberService;
    private readonly ILogger<UserIntegrationPoolService> _logger;

    public UserIntegrationPoolService(
        IMemberService iMemberService,
        ILogger<UserIntegrationPoolService> logger)
    {
        _iMemberService = iMemberService ?? throw new ArgumentNullException(nameof(iMemberService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<ExternalUserInfo?> GetExternalUserInfoAsync(
      NationalId nationalCode, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            _logger.LogWarning("GetExternalUserInfo called with empty national code");
            return null;
        }

        try
        {
            _logger.LogDebug("Getting external user info: {NationalCode}", nationalCode);
            
            var memberDto = await _iMemberService.GetMemberByNationalCodeAsync(nationalCode);
            if (memberDto == null)
            {
                _logger.LogDebug("External user not found for national code: {NationalCode}", nationalCode);
                return null;
            }

            var userInfo = new ExternalUserInfo
            {
                ExternalUserId = memberDto.Id,
                NationalCode = memberDto.NationalCode,
                FirstName = memberDto.FirstName,
                LastName = memberDto.LastName,
                PhoneNumber = memberDto.PhoneNumber,
                Email = memberDto.Email,
                ExternalId = memberDto.MembershipNumber,
                IsActiveUser = memberDto.IsActive,
                StatusStartDate = memberDto.MembershipStartDate,
                StatusEndDate = memberDto.MembershipEndDate,
                UserRole = memberDto.IsActive ? "Member" : "InactiveMember"
            };

            _logger.LogDebug("Retrieved external user info for {NationalCode}: Status={Status}", 
                nationalCode, userInfo.UserStatus);

            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting external user info: {NationalCode}", nationalCode);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<UserValidationInfo?> ValidateUserEligibilityAsync(
      NationalId nationalCode, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            _logger.LogWarning("ValidateUserEligibility called with empty national code");
            return null;
        }

        try
        {
            _logger.LogDebug("Validating user eligibility: {NationalCode}", nationalCode);
            
            var basicInfo = await _iMemberService.GetBasicMemberInfoAsync(nationalCode);
            if (basicInfo == null)
            {
                _logger.LogDebug("UserDetail not found for eligibility check: {NationalCode}", nationalCode);
                return null;
            }

            var memberDto = await _iMemberService.GetMemberByNationalCodeAsync(nationalCode);
            
            var validationInfo = new UserValidationInfo
            {
                UserId = basicInfo.Id,
                NationalCode = basicInfo.NationalCode,
                FirstName = memberDto?.FirstName ?? "",
                LastName = memberDto?.LastName ?? "",
                PhoneNumber = memberDto?.PhoneNumber,
                Email = memberDto?.Email,
                ExternalId = basicInfo.MembershipNumber,
                IsActiveUser = basicInfo.IsActive,
                HasValidStatus = basicInfo.HasActiveMembership,
                StatusExpiryDate = memberDto?.MembershipEndDate,
                UserRole = basicInfo.IsActive ? "Member" : "InactiveMember"
            };

            _logger.LogDebug("UserDetail eligibility validation completed for {NationalCode}: CanCreateAccount={CanCreate}", 
                nationalCode, validationInfo.CanCreateUserAccount);

            return validationInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user eligibility: {NationalCode}", nationalCode);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> HasActiveStatusAsync(
      NationalId nationalCode, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            _logger.LogWarning("HasActiveStatus called with empty national code");
            return false;
        }

        try
        {
            _logger.LogDebug("Checking active status for: {NationalCode}", nationalCode);
            
            var hasActive = await _iMemberService.HasActiveMembershipAsync(nationalCode);
            
            _logger.LogDebug("Active status check for {NationalCode}: {HasActive}", nationalCode, hasActive);
            return hasActive;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking active status for: {NationalCode}", nationalCode);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<UserLookupResult> ValidateUserExistenceAsync(
      NationalId searchKey, 
        UserLookupType lookupType, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchKey))
        {
            _logger.LogWarning("ValidateUserExistence called with empty search key");
            return UserLookupResult.Error(searchKey, lookupType, "Search key cannot be empty");
        }

        try
        {
            _logger.LogDebug("Validating user existence: {SearchKey} ({LookupType})", searchKey, lookupType);

            var memberDto = lookupType switch
            {
                UserLookupType.NationalCode => await _iMemberService.GetMemberByNationalCodeAsync(searchKey),
                UserLookupType.ExternalId => await _iMemberService.GetMemberByMembershipNumberAsync(searchKey),
                UserLookupType.PhoneNumber => await _iMemberService.GetMemberByPhoneNumberAsync(searchKey),
                UserLookupType.Email => await _iMemberService.GetMemberByEmailAsync(searchKey),
                _ => throw new ArgumentOutOfRangeException(nameof(lookupType), lookupType, "Invalid lookup type")
            };

            if (memberDto == null)
            {
                _logger.LogDebug("UserDetail not found for {SearchKey} ({LookupType})", searchKey, lookupType);
                return UserLookupResult.NotFound(searchKey, lookupType);
            }

            var validationInfo = new UserValidationInfo
            {
                UserId = memberDto.Id,
                NationalCode = memberDto.NationalCode,
                FirstName = memberDto.FirstName,
                LastName = memberDto.LastName,
                PhoneNumber = memberDto.PhoneNumber,
                Email = memberDto.Email,
                ExternalId = memberDto.MembershipNumber,
                IsActiveUser = memberDto.IsActive,
                HasValidStatus = memberDto.HasActiveMembership,
                StatusExpiryDate = memberDto.MembershipEndDate,
                UserRole = memberDto.IsActive ? "Member" : "InactiveMember"
            };

            _logger.LogDebug("UserDetail found for {SearchKey} ({LookupType}): {UserId}", 
                searchKey, lookupType, memberDto.Id);

            return UserLookupResult.Success(searchKey, lookupType, validationInfo);
        }
        catch (ArgumentOutOfRangeException)
        {
            var errorMessage = $"Invalid lookup type: {lookupType}";
            _logger.LogWarning(errorMessage);
            return UserLookupResult.Error(searchKey, lookupType, errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user existence: {SearchKey} ({LookupType})", searchKey, lookupType);
            return UserLookupResult.Error(searchKey, lookupType, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserValidationInfo>> BatchValidateUsersAsync(
        IEnumerable<NationalId> nationalCodes, 
        CancellationToken cancellationToken = default)
    {
        if (nationalCodes == null)
        {
            _logger.LogWarning("BatchValidateUsers called with null national codes");
            return Enumerable.Empty<UserValidationInfo>();
        }

        var codesList = nationalCodes.Where(nc => !string.IsNullOrWhiteSpace(nc)).ToList();
        if (!codesList.Any())
        {
            _logger.LogWarning("BatchValidateUsers called with no valid national codes");
            return Enumerable.Empty<UserValidationInfo>();
        }

        try
        {
            _logger.LogDebug("Batch validating {Count} users", codesList.Count);

            var members = await _iMemberService.GetMembersByNationalCodesAsync(codesList);
            var validationInfos = members.Select(member => new UserValidationInfo
            {
                UserId = member.Id,
                NationalCode = member.NationalCode,
                FirstName = member.FirstName,
                LastName = member.LastName,
                PhoneNumber = member.PhoneNumber,
                Email = member.Email,
                ExternalId = member.MembershipNumber,
                IsActiveUser = member.IsActive,
                HasValidStatus = member.HasActiveMembership,
                StatusExpiryDate = member.MembershipEndDate,
                UserRole = member.IsActive ? "Member" : "InactiveMember"
            }).ToList();

            _logger.LogDebug("Batch validation completed: {Found}/{Total} users found", 
                validationInfos.Count, codesList.Count);

            return validationInfos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch user validation for {Count} codes", codesList.Count);
            return Enumerable.Empty<UserValidationInfo>();
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Performing external user service health check");
            
            // Try to call a simple method to check if the service is responsive
            // Using a non-existent national code to avoid any side effects
            await _iMemberService.IsMemberExistsByNationalCodeAsync(new NationalId("0000000000"));
            
            _logger.LogDebug("External user service health check passed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "External user service health check failed");
            return false;
        }
    }
}