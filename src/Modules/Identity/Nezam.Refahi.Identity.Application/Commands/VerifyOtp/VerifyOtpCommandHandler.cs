// -----------------------------------------------------------------------------
// VerifyOtpCommandHandler.cs - Production-grade DDD + EF Core token system
// -----------------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Application.Common.Services;

namespace Nezam.Refahi.Identity.Application.Commands.VerifyOtp;

/// <summary>
/// Production-grade OTP verification handler implementing DDD + EF Core best practices
/// Uses the new token system with JWT access tokens (5-15 min TTL) and refresh token rotation
/// </summary>
public sealed class VerifyOtpCommandHandler
    : IRequestHandler<VerifyOtpCommand, ApplicationResult<VerifyOtpResponse>>
{
    // ========================================================================
    // Configuration Constants
    // ========================================================================
    
    private const int AccessTokenExpiryMinutes = 15;  // 15 minutes (production-grade)
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

        await _uow.BeginAsync(ct);

        try
        {
            // ========================================================================
            // 2) Load and Validate User
            // ========================================================================
            
            var user = await _userRepository.GetByNationalIdAsync(new NationalId(request.NationalCode));
            if (user is null)
                return await FailAndRollbackAsync($"User {request.NationalCode} not found.");

            if (string.IsNullOrWhiteSpace(user.PhoneNumber?.Value))
                return await FailAndRollbackAsync("User has no phone number.");

            // ========================================================================
            // 3) Verify OTP Challenge
            // ========================================================================
            
            var phoneNumber = user.PhoneNumber;
            var activeChallenges = await _otpChallengeRepository.GetActiveChallengesByPhoneAsync(phoneNumber);
            var challenge = activeChallenges.FirstOrDefault(c => c.CanBeVerified);
            
            if (challenge == null)
                return await FailAndRollbackAsync("No active OTP challenge found. Please request a new OTP.");

            // Verify OTP code against challenge
            var otpHash = await _otpHasherService.HashAsync(
                challenge.ChallengeId, 
                phoneNumber.Value, 
                request.OtpCode, 
                challenge.Nonce);
            
            if (!challenge.AttemptVerification(otpHash, user.Id))
            {
                await _otpChallengeRepository.UpdateAsync(challenge, ct);
                await _uow.SaveAsync(ct);
                return await FailAndRollbackAsync("Invalid or expired OTP code.");
            }

            // Mark challenge as consumed
            challenge.Consume();
            await _otpChallengeRepository.UpdateAsync(challenge, ct);

            // ========================================================================
            // 4) Clean Up Old Challenges
            // ========================================================================
            
            await _otpChallengeRepository.DeleteExpiredChallengesAsync();
            await _otpChallengeRepository.DeleteOldChallengesAsync(7);

            // ========================================================================
            // 5) Revoke All Existing Tokens (Security Best Practice)
            // ========================================================================
            
            // Revoke all existing refresh tokens for this user
            await _tokenService.RevokeAllUserRefreshTokensAsync(user.Id);
            
            // Clean up any existing access tokens
            var existingAccessTokens = await _userTokenRepository.GetActiveTokensForUserAsync(user.Id, "AccessToken");
            foreach (var token in existingAccessTokens)
            {
                token.Revoke();
                await _userTokenRepository.UpdateAsync(token, ct);
            }

            // ========================================================================
            // 6) Update User Domain State
            // ========================================================================
            
            user.LoggedIn();
            
            // Ensure Engineer role is assigned for users with national ID
            if (user.NationalId is not null && !user.HasRole("Engineer"))
            {
                var engineerRole = await _roleRepository.GetByNameAsync("Engineer", ct);
                if (engineerRole != null)
                {
                    user.AssignRole(engineerRole);
                }
            }

            await _userRepository.UpdateAsync(user, ct);

            // ========================================================================
            // 7) Generate New Tokens Using Production-Grade Token Service
            // ========================================================================
            
            // Generate JWT access token (15 minutes TTL)
            var accessToken = _tokenService.GenerateAccessToken(user, out var jwtId, AccessTokenExpiryMinutes);
            
            // Store JWT reference for revocation tracking
            var jwtToken = UserToken.CreateAccessToken(
                user.Id,
                jwtId,
                AccessTokenExpiryMinutes,
                request.DeviceId,
                request.IpAddress,
                request.UserAgent);
            await _userTokenRepository.AddAsync(jwtToken, ct);

            // Generate refresh token (30 days TTL with proper hashing)
            var (refreshToken, hashedRefreshToken, refreshTokenId) = await _tokenService.GenerateRefreshTokenAsync(
                user.Id,
                request.DeviceId,
                request.IpAddress,
                request.UserAgent,
                RefreshTokenExpiryDays);

            // ========================================================================
            // 8) Commit All Changes
            // ========================================================================
            
            await _uow.SaveAsync(ct);
            await _uow.CommitAsync(ct);

            // ========================================================================
            // 9) Build Response
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

            _logger.LogInformation("OTP verified & tokens generated for User {UserId} with JWT {JwtId}", 
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
            _logger.LogError(ex, "VerifyOtp failed for NationalCode {NationalCode}", request.NationalCode);
            return ApplicationResult<VerifyOtpResponse>.Failure($"Verification failed: {ex.Message}", ex);
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
