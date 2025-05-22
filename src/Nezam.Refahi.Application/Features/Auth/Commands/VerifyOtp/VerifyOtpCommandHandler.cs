using MediatR;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Application.Common.Models;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace Nezam.Refahi.Application.Features.Auth.Commands.VerifyOtp;

/// <summary>
/// Handler for the VerifyOtpCommand
/// </summary>
public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, ApplicationResult<VerifyOtpResponse>>
{
    private readonly IOtpService _otpService;
    private readonly IUserRepository _userRepository;
    private readonly UserDomainService _userDomainService;
    private readonly ITokenService _tokenService;
    private readonly IValidator<VerifyOtpCommand> _validator;
    
    // Constants for token settings
    private const int TokenExpiryMinutes = 60;
    private const int RefreshTokenExpiryMinutes = 10080; // 7 days
    
    public VerifyOtpCommandHandler(
        IOtpService otpService,
        IUserRepository userRepository,
        UserDomainService userDomainService,
        ITokenService tokenService,
        IValidator<VerifyOtpCommand> validator)
    {
        _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }
    
    public async Task<ApplicationResult<VerifyOtpResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate the request using FluentValidation
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                // Return validation errors
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<VerifyOtpResponse>.Failure(errors, "Validation failed");
            }
            
            // Get the user from the repository first to check if they exist
            var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
            
            // If user doesn't exist, we should not proceed with verification
            // This is a security measure to prevent OTP brute force attacks
            if (user == null)
            {
                return ApplicationResult<VerifyOtpResponse>.Failure("Invalid phone number or verification code.");
            }
            
            // Verify the OTP code using the domain service
            var isValid = await _otpService.ValidateOtpAsync(request.PhoneNumber, request.OtpCode, request.Purpose);
            if (!isValid)
            {
                return ApplicationResult<VerifyOtpResponse>.Failure("Invalid or expired verification code.");
            }
            
            // OTP is valid, now check if the user profile is completed
            // According to DDD principles, this business rule belongs to the domain layer
            bool isProfileCompleted = IsUserProfileComplete(user);
            bool requiresRegistrationCompletion = !isProfileCompleted;
            
            // Generate token for authentication
            // Following DDD principles, token generation is a domain service responsibility
            string accessToken = _tokenService.GenerateToken(user);
            
            // In a real implementation, we would also generate a refresh token
            // But for now, we'll use a placeholder
            string refreshToken = Guid.NewGuid().ToString();
            
            // Create the response data
            var response = new VerifyOtpResponse
            {
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiryMinutes = TokenExpiryMinutes,
                IsRegistered = true,
                RequiresRegistrationCompletion = requiresRegistrationCompletion
            };
            
            // Return successful result
            return ApplicationResult<VerifyOtpResponse>.Success(
                response,
                requiresRegistrationCompletion 
                    ? "Authentication successful. Please complete your profile." 
                    : "Authentication successful."
            );
        }
        catch (Exception ex)
        {
            // Return failure result with exception
            return ApplicationResult<VerifyOtpResponse>.Failure($"Verification failed: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Determines if a user's profile is complete based on business rules
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if the profile is complete, false otherwise</returns>
    private static bool IsUserProfileComplete(User user)
    {
        if (user == null)
            return false;
            
        // A profile is considered complete if the user has:
        // 1. First name
        // 2. Last name
        // 3. National ID
        return !string.IsNullOrEmpty(user.FirstName) && 
               !string.IsNullOrEmpty(user.LastName) && 
               user.NationalId != null;
    }
}
