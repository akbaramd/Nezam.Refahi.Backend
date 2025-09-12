// -----------------------------------------------------------------------------
// VerifyOtpCommandHandler.cs - Production-grade DDD + EF Core token system
// -----------------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.VerifyOtp;

/// <summary>
/// Production-grade OTP verification handler implementing DDD + EF Core best practices
/// Uses the new token system with JWT access tokens (5-15 min TTL) and refresh token rotation
/// </summary>
public class VerifyOtpCommandHandler
    : IRequestHandler<VerifyOtpCommand, ApplicationResult<VerifyOtpResponse>>
{
    // ========================================================================
    // Configuration Constants
    // ========================================================================
    
    private const int AccessTokenExpiryMinutes = 30;  // 15 minutes (production-grade)
    private const int RefreshTokenExpiryDays = 30;    // 30 days (production-grade)

    // ========================================================================
    // Dependencies
    // ========================================================================
    
    private readonly IOtpHasherService _otpHasherService;
    private readonly IOtpChallengeRepository _otpChallengeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IScopeAuthorizationService _scopeAuthorizationService;
    private readonly IValidator<VerifyOtpCommand> _validator;
    private readonly IIdentityUnitOfWork _uow;
    private readonly ILogger<VerifyOtpCommandHandler> _logger;

    public VerifyOtpCommandHandler(
        IOtpHasherService otpHasherService,
        IOtpChallengeRepository otpChallengeRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserTokenRepository userTokenRepository,
        ITokenService tokenService,
        IScopeAuthorizationService scopeAuthorizationService,
        IValidator<VerifyOtpCommand> validator,
        IIdentityUnitOfWork uow,
        ILogger<VerifyOtpCommandHandler> logger)
    {
        _otpHasherService = otpHasherService ?? throw new ArgumentNullException(nameof(otpHasherService));
        _otpChallengeRepository = otpChallengeRepository ?? throw new ArgumentNullException(nameof(otpChallengeRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _userTokenRepository = userTokenRepository ?? throw new ArgumentNullException(nameof(userTokenRepository));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _scopeAuthorizationService = scopeAuthorizationService ?? throw new ArgumentNullException(nameof(scopeAuthorizationService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<VerifyOtpResponse>> Handle(
        VerifyOtpCommand request,
        CancellationToken ct)
    {
        // ========================================================================
        // 1) Validate Request
        // ========================================================================
        
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ApplicationResult<VerifyOtpResponse>
                   .Failure(validation.Errors.Select(e => e.ErrorMessage));

        // ========================================================================
        // 2) Cleanup Expired Data Before Starting Transaction
        // ========================================================================
        
        try
        {
            // Clean up expired OTP challenges to keep database clean
            var expiredChallengesDeleted = await _otpChallengeRepository.DeleteExpiredChallengesAsync();
            if (expiredChallengesDeleted > 0)
            {
                _logger.LogDebug("Cleaned up {Count} expired OTP challenges before authentication", expiredChallengesDeleted);
            }
            
            // Clean up expired and trash tokens to optimize database performance
            var expiredTokensDeleted = await _tokenService.CleanupExpiredTokensAsync();
            if (expiredTokensDeleted > 0)
            {
                _logger.LogDebug("Cleaned up {Count} expired tokens before authentication", expiredTokensDeleted);
            }
        }
        catch (Exception ex)
        {
            // Log cleanup failures but don't fail the authentication process
            _logger.LogWarning(ex, "Failed to cleanup expired data before authentication - continuing with authentication flow");
        }

        await _uow.BeginAsync(ct);

        try
        {
            // ========================================================================
            // 3) Get OTP Challenge First
            // ========================================================================
            
            var challenge = await _otpChallengeRepository.GetByIdAsync(request.ChallengeId, ct);
            
            if (challenge == null || !challenge.CanBeVerified)
                return await FailAndRollbackAsync("No active OTP challenge found. Please request a new OTP.");

            // ========================================================================
            // 3) Load and Validate UserDetail from Phone Number
            // ========================================================================
            
            var user = await _userRepository.GetByPhoneNumberAsync(challenge.PhoneNumber);
            if (user is null)
                return await FailAndRollbackAsync("UserDetail not found for this OTP challenge.");

            // Verify OTP code against challenge
            var otpHash = await _otpHasherService.HashAsync(
                challenge.PhoneNumber.Value, 
                request.OtpCode, 
                challenge.Nonce);
            
            if (!challenge.AttemptVerification(otpHash, user.Id))
            {
                // Handle concurrency exception for failed verification attempts
                try
                {
                    await _otpChallengeRepository.UpdateAsync(challenge, ct);
                    await _uow.SaveAsync(ct);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(ex, "Concurrency conflict updating OTP challenge {ChallengeId}, reloading to check status", request.ChallengeId);
                    
                    // Another request already updated this challenge, reload and check current status
                    challenge = await _otpChallengeRepository.GetByIdAsync(request.ChallengeId, ct);
                    if (challenge?.Status == Identity.Domain.Enums.ChallengeStatus.Locked)
                    {
                        return await FailAndRollbackAsync("Challenge has been locked due to too many attempts.");
                    }
                    if (challenge?.Status == Identity.Domain.Enums.ChallengeStatus.Verified)
                    {
                        return await FailAndRollbackAsync("Challenge has already been verified by another request.");
                    }
                    // If not locked or verified, treat as invalid attempt
                }
                return await FailAndRollbackAsync("Invalid or expired OTP code.");
            }

            // Mark challenge as consumed and delete it immediately (no need to keep successful challenges)
            try
            {
                // CRITICAL FIX: Use original challenge entity, not reloaded one from concurrency handling
                var originalChallenge = await _otpChallengeRepository.GetByIdAsync(request.ChallengeId, ct);
                if (originalChallenge != null)
                {
                    originalChallenge.Consume();
                    await _otpChallengeRepository.DeleteAsync(originalChallenge, ct);
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict when consuming/deleting OTP challenge {ChallengeId}. Another request may have already processed it.", request.ChallengeId);
                // This is acceptable - another request succeeded in parallel, we can continue
                // The challenge was already consumed/deleted by the other request
            }

            // // ========================================================================
            // // 4) Clean Up Related Challenges (Optimized for Success Flow)
            // // ========================================================================
            //
            // // Since user authenticated successfully, clean up all challenges for this phone
            // // This prevents trash data accumulation and ensures clean state
            // await _otpChallengeRepository.DeleteChallengesForPhoneAsync(challenge.PhoneNumber, ct);
            //
            // // Lightweight expired cleanup - avoid heavy operations during success flow
            // // Only clean expired challenges, consumed ones are already handled above
            // await _otpChallengeRepository.DeleteExpiredChallengesAsync();

            // ========================================================================
            // 5) Revoke All Existing Tokens (Security Best Practice)
            // ========================================================================
            
            // CRITICAL FIX: Revoke tokens without creating tracking conflicts
            // Revoke all existing refresh tokens for this user
            user.RevokeAllUserRefreshTokens();
            
            // Clean up any existing access tokens - use repository to avoid tracking conflicts
            var existingAccessTokens = await _userTokenRepository.GetActiveTokensForUserAsync(user.Id, "AccessToken");
            foreach (var token in existingAccessTokens)
            {
                token.Revoke();
                await _userTokenRepository.UpdateAsync(token, ct);
            }

            // ========================================================================
            // 6) Update UserDetail Domain State
            // ========================================================================
            
            user.LoggedIn();
            
            // CRITICAL FIX: Ensure Engineer role is assigned for users with national ID
            if (user.NationalId is not null && !user.HasRole("Engineer"))
            {
                var engineerRole = await _roleRepository.GetByNameAsync("Engineer", ct);
                if (engineerRole != null)
                {
                    // Use role ID to avoid tracking conflicts with potentially loaded Role entities
                    user.AssignRole(engineerRole.Id);
                }
            }

     

            // ========================================================================
            // 7) Validate Scope Authorization
            // ========================================================================
            
            // Validate that the user has permission to access the requested scope
            var scopeValidation = user.ValidateScope( request.Scope);
            if (!scopeValidation)
            {
                _logger.LogWarning("UserDetail {UserId} denied access to scope '{Scope}'", user.Id, request.Scope);
                return await FailAndRollbackAsync($"Access denied. Users with your role cannot access the '{request.Scope}' scope.");
            }
            
            _logger.LogInformation("UserDetail {UserId} authorized for scope '{Scope}'", user.Id, request.Scope);

            // ========================================================================
            // 8) Generate New Tokens Using Production-Grade Token Service
            // ========================================================================
            
            // Generate JWT access token (15 minutes TTL)
            // The token service now handles storage internally
            var (accessToken, expiryMinutes, jwtId) = await _tokenService.GenerateAccessTokenAsync(
                user, 
                request.DeviceId, 
                request.IpAddress, 
                request.UserAgent, 
                AccessTokenExpiryMinutes);
            // Generate refresh token (30 days TTL with proper hashing)
            var (refreshToken, hashedRefreshToken, refreshTokenId) = await _tokenService.GenerateRefreshTokenAsync(
              user.Id,
              request.DeviceId,
              request.IpAddress,
              request.UserAgent,
              RefreshTokenExpiryDays);


            // ========================================================================
            // 9) Commit All Changes with Optimized Approach
            // ========================================================================
            try
            {
                // CRITICAL FIX: Save in specific order to minimize tracking conflicts
                // 1. Save user changes first (most important entity)
                await _userRepository.UpdateAsync(user, ct);
                
                // 2. Save all changes atomically
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);
                
                _logger.LogDebug("Successfully saved all authentication changes for user {UserId}", user.Id);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict detected during save operation for user {UserId}. Entity states: UserDetail tracked, Challenge processed, Tokens updated.", user.Id);
                
                // Detailed logging for debugging
                _logger.LogError("Concurrency conflict details - ChallengeId: {ChallengeId}, UserId: {UserId}, Scope: {Scope}", 
                    request.ChallengeId, user.Id, request.Scope);
                
                await _uow.RollbackAsync(ct);
                return ApplicationResult<VerifyOtpResponse>.Failure("Authentication conflict detected. Please try verification again.");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error during authentication save for user {UserId}", user.Id);
                await _uow.RollbackAsync(ct);
                return ApplicationResult<VerifyOtpResponse>.Failure("Database error occurred. Please try again.");
            }

            // ========================================================================
            // 10) Build Response
            // ========================================================================
            
            var profileDone = IsProfileComplete(user);
            var response = new VerifyOtpResponse
            {
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiryMinutes = AccessTokenExpiryMinutes,
                IsRegistered = true,
                RequiresRegistrationCompletion = !profileDone
            };

            _logger.LogInformation("OTP verified & tokens generated for UserDetail {UserId} with JWT {JwtId}", 
                user.Id, jwtId);

            return ApplicationResult<VerifyOtpResponse>
                   .Success(response,
                     profileDone ? "Authentication successful."
                                  : "Authentication successful. Please complete your profile.");

            // ========================================================================
            // Local Helper Methods
            // ========================================================================
            
            async Task<ApplicationResult<VerifyOtpResponse>> FailAndRollbackAsync(string msg)
            {
                await _uow.RollbackAsync(ct);
                return ApplicationResult<VerifyOtpResponse>.Failure(msg);
            }
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync(ct);
            _logger.LogError(ex, "VerifyOtp failed for ChallengeId {ChallengeId}", request.ChallengeId);
            return ApplicationResult<VerifyOtpResponse>.Failure($"Verification failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if user profile is complete
    /// </summary>
    private static bool IsProfileComplete(User user) =>
        !string.IsNullOrWhiteSpace(user.FirstName) &&
        !string.IsNullOrWhiteSpace(user.LastName) &&
        user.NationalId is not null;
}
