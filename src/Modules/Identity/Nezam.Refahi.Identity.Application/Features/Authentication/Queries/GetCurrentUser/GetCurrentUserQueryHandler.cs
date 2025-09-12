using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Contracts.Dtos;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Queries.GetCurrentUser;

/// <summary>
/// Enhanced handler for the GetCurrentUserQuery with comprehensive user profile data
/// Fetches all roles, claims, and preferences for the current user
/// </summary>
public class GetCurrentUserQueryHandler 
    : IRequestHandler<GetCurrentUserQuery, ApplicationResult<CurrentUserResponse>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IUserPreferenceRepository _userPreferenceRepository;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;

    public GetCurrentUserQueryHandler(
        ICurrentUserService currentUser, 
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IUserClaimRepository userClaimRepository,
        IRoleClaimRepository roleClaimRepository,
        IUserPreferenceRepository userPreferenceRepository,
        ILogger<GetCurrentUserQueryHandler> logger)
    {
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _userClaimRepository = userClaimRepository ?? throw new ArgumentNullException(nameof(userClaimRepository));
        _roleClaimRepository = roleClaimRepository ?? throw new ArgumentNullException(nameof(roleClaimRepository));
        _userPreferenceRepository = userPreferenceRepository ?? throw new ArgumentNullException(nameof(userPreferenceRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<CurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        try
        {
            // ========================================================================
            // 1) Validate Authentication
            // ========================================================================
            
            if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
                return ApplicationResult<CurrentUserResponse>.Failure("UserDetail is not authenticated.");

            var userId = _currentUser.UserId.Value;

            // ========================================================================
            // 2) Fetch UserDetail Data
            // ========================================================================
            
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user is null)
                return ApplicationResult<CurrentUserResponse>.Failure("UserDetail not found.");

            // ========================================================================
            // 3) Fetch UserDetail Roles, Claims, and Preferences Sequentially
            // ========================================================================
            // Note: Running sequentially to avoid DbContext concurrency issues
            
            var userRoles = await _userRoleRepository.GetActiveByUserIdAsync(userId, ct);
            var userClaims = await _userClaimRepository.GetActiveByUserIdAsync(userId, ct);
            var userPreferences = await _userPreferenceRepository.GetActiveByUserIdAsync(userId);

            // ========================================================================
            // 4) Fetch Role Claims for Each Role Sequentially
            // ========================================================================
            
            var allRoleClaims = new List<RoleClaim>();
            foreach (var userRole in userRoles)
            {
                var roleClaims = await _roleClaimRepository.GetByRoleIdAsync(userRole.RoleId, ct);
                allRoleClaims.AddRange(roleClaims);
            }

            // ========================================================================
            // 5) Build Response
            // ========================================================================
            
            var response = BuildUserResponse(user, userRoles, userClaims, allRoleClaims, userPreferences, ct);

            _logger.LogDebug("Successfully built comprehensive user profile for user {UserId}", userId);

            return ApplicationResult<CurrentUserResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current user profile for user {UserId}", _currentUser.UserId);
            return ApplicationResult<CurrentUserResponse>.Failure("Failed to fetch user profile. Please try again.");
        }
    }

    private CurrentUserResponse BuildUserResponse(
        User user, 
        IEnumerable<UserRole> userRoles, 
        IEnumerable<UserClaim> userClaims, 
        IEnumerable<RoleClaim> allRoleClaims,
        IEnumerable<UserPreference> userPreferences,
        CancellationToken ct)
    {
        // ========================================================================
        // Simple, Professional Response - Like Google/Facebook APIs
        // ========================================================================
        
        var response = new CurrentUserResponse
        {
            Id = user.Id,
            Name = $"{user.FirstName} {user.LastName}".Trim(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            NationalId = user.NationalId?.Value ?? string.Empty,
            Phone = user.PhoneNumber?.Value ?? string.Empty
        };

        // ========================================================================
        // Roles - Simple List
        // ========================================================================
        
        response.Roles = userRoles
            .Where(ur => ur.IsActive && !ur.IsExpired())
            .Select(ur => ur.Role?.Name ?? "Unknown")
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        // ========================================================================
        // Claims - Simple DTOs (All Active Claims)
        // ========================================================================
        
        var directClaims = userClaims
            .Where(uc => uc.IsActive && !uc.IsExpired())
            .Select(uc => new ClaimDto 
            { 
                Value = uc.Claim.Value,
                Type = uc.Claim.Type,
                ValueType = uc.Claim.ValueType
            });

        var roleClaims = allRoleClaims
            .Select(rc => new ClaimDto 
            { 
                Value = rc.Claim.Value,
                Type = rc.Claim.Type,
                ValueType = rc.Claim.ValueType,
            });

        response.Claims = directClaims
            .Concat(roleClaims)
            .GroupBy(c => c.Value)
            .Select(g => g.First())
            .OrderBy(c => c.Value)
            .ToList();

        // ========================================================================
        // Preferences - Simple DTOs
        // ========================================================================
        
        response.Preferences = userPreferences
            .Where(up => up.IsActive)
            .Select(up => new UserPreferenceDto()
            {
                Key = up.Key.Value,
                Value = new PreferenceValueDto(){RawValue = up.Value.RawValue, Type = (int)up.Value.Type},
                Category = up.Category.ToString(),
                IsActive = up.IsActive
            })
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Key)
            .ToList();

        return response;
    }


}
