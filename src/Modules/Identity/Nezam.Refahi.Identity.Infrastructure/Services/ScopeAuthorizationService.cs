using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

public class ScopeAuthorizationService : IScopeAuthorizationService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ScopeAuthorizationService> _logger;

    public ScopeAuthorizationService(
        IUserRepository userRepository,
        ILogger<ScopeAuthorizationService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<bool>> ValidateScopeAsync(Guid userId, string requestedScope)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(requestedScope))
                return ApplicationResult<bool>.Failure("Scope is required");

            var user = await _userRepository.FindOneAsync(x => x.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("UserDetail {UserId} not found", userId);
                return ApplicationResult<bool>.Failure("UserDetail not found");
            }

            // Use the UserDetail entity's scope validation method
            var hasScope = user.ValidateScope(requestedScope);
            _logger.LogDebug("UserDetail {UserId} scope validation for '{Scope}': {HasScope}", 
                userId, requestedScope, hasScope);

            return ApplicationResult<bool>.Success(hasScope);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating scope {Scope} for user {UserId}", requestedScope, userId);
            return ApplicationResult<bool>.Failure("Failed to validate scope");
        }
    }

    public async Task<ApplicationResult<IEnumerable<string>>> GetUserScopesAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.FindOneAsync(x => x.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("UserDetail {UserId} not found", userId);
                return ApplicationResult<IEnumerable<string>>.Failure("UserDetail not found");
            }

            // Use the UserDetail entity's scope retrieval method
            var scopes = user.GetUserScopes();

            _logger.LogDebug("UserDetail {UserId} has scopes: {Scopes}", userId, string.Join(", ", scopes));
            return ApplicationResult<IEnumerable<string>>.Success(scopes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scopes for user {UserId}", userId);
            return ApplicationResult<IEnumerable<string>>.Failure("Failed to get user scopes");
        }
    }

    public async Task<ApplicationResult<bool>> HasPanelAccessAsync(Guid userId)
    {
        return await ValidateScopeAsync(userId, "panel");
    }

    public async Task<ApplicationResult<bool>> HasAppAccessAsync(Guid userId)
    {
        return await ValidateScopeAsync(userId, "app");
    }

    public async Task<ApplicationResult<bool>> ValidateAnyScopeAsync(Guid userId, IEnumerable<string> requestedScopes)
    {
        try
        {
            if (requestedScopes == null || !requestedScopes.Any())
                return ApplicationResult<bool>.Failure("At least one scope is required");

            var user = await _userRepository.FindOneAsync(x => x.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("UserDetail {UserId} not found", userId);
                return ApplicationResult<bool>.Failure("UserDetail not found");
            }

            // Use the UserDetail entity's scope validation method
            var hasAnyScope = user.ValidateAnyScope(requestedScopes);
            _logger.LogDebug("UserDetail {UserId} any scope validation for '{Scopes}': {HasAnyScope}", 
                userId, string.Join(", ", requestedScopes), hasAnyScope);

            return ApplicationResult<bool>.Success(hasAnyScope);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating any scope for user {UserId}", userId);
            return ApplicationResult<bool>.Failure("Failed to validate scopes");
        }
    }

    public async Task<ApplicationResult<bool>> ValidateAllScopesAsync(Guid userId, IEnumerable<string> requestedScopes)
    {
        try
        {
            if (requestedScopes == null || !requestedScopes.Any())
                return ApplicationResult<bool>.Failure("At least one scope is required");

            var user = await _userRepository.FindOneAsync(x => x.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("UserDetail {UserId} not found", userId);
                return ApplicationResult<bool>.Failure("UserDetail not found");
            }

            // Use the UserDetail entity's scope validation method
            var hasAllScopes = user.ValidateAllScopes(requestedScopes);
            _logger.LogDebug("UserDetail {UserId} all scopes validation for '{Scopes}': {HasAllScopes}", 
                userId, string.Join(", ", requestedScopes), hasAllScopes);

            return ApplicationResult<bool>.Success(hasAllScopes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating all scopes for user {UserId}", userId);
            return ApplicationResult<bool>.Failure("Failed to validate scopes");
        }
    }

    public async Task<ApplicationResult<IEnumerable<string>>> GetMissingScopesAsync(Guid userId, IEnumerable<string> requestedScopes)
    {
        try
        {
            if (requestedScopes == null || !requestedScopes.Any())
                return ApplicationResult<IEnumerable<string>>.Success(Enumerable.Empty<string>());

            var user = await _userRepository.FindOneAsync(x => x.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("UserDetail {UserId} not found", userId);
                return ApplicationResult<IEnumerable<string>>.Failure("UserDetail not found");
            }

            // Use the UserDetail entity's missing scopes method
            var missingScopes = user.GetMissingScopes(requestedScopes);
            _logger.LogDebug("UserDetail {UserId} missing scopes: {MissingScopes}", 
                userId, string.Join(", ", missingScopes));

            return ApplicationResult<IEnumerable<string>>.Success(missingScopes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting missing scopes for user {UserId}", userId);
            return ApplicationResult<IEnumerable<string>>.Failure("Failed to get missing scopes");
        }
    }

    public async Task<ApplicationResult<bool>> HasAnyScopesAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.FindOneAsync(x => x.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("UserDetail {UserId} not found", userId);
                return ApplicationResult<bool>.Failure("UserDetail not found");
            }

            // Use the UserDetail entity's scope checking method
            var hasAnyScopes = user.HasAnyScopes();
            _logger.LogDebug("UserDetail {UserId} has any scopes: {HasAnyScopes}", userId, hasAnyScopes);

            return ApplicationResult<bool>.Success(hasAnyScopes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has any scopes", userId);
            return ApplicationResult<bool>.Failure("Failed to check user scopes");
        }
    }

    public async Task<ApplicationResult<int>> GetScopeCountAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.FindOneAsync(x => x.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("UserDetail {UserId} not found", userId);
                return ApplicationResult<int>.Failure("UserDetail not found");
            }

            // Use the UserDetail entity's scope count method
            var scopeCount = user.GetScopeCount();
            _logger.LogDebug("UserDetail {UserId} has {ScopeCount} scopes", userId, scopeCount);

            return ApplicationResult<int>.Success(scopeCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scope count for user {UserId}", userId);
            return ApplicationResult<int>.Failure("Failed to get scope count");
        }
    }
}