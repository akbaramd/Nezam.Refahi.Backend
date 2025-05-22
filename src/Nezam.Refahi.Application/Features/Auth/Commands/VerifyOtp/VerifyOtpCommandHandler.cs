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
    private readonly IUnitOfWork _unitOfWork;

    private const int TokenExpiryMinutes = 60;
    private const int RefreshTokenExpiryMinutes = 10080; // 7 days

    public VerifyOtpCommandHandler(
        IOtpService otpService,
        IUserRepository userRepository,
        UserDomainService userDomainService,
        ITokenService tokenService,
        IValidator<VerifyOtpCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<VerifyOtpResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<VerifyOtpResponse>.Failure(errors, "Validation failed");
            }

            var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
            if (user is null)
            {
                return ApplicationResult<VerifyOtpResponse>.Failure("Invalid phone number or verification code.");
            }

            var isValid = await _otpService.ValidateOtpAsync(request.PhoneNumber, request.OtpCode, request.Purpose);
            if (!isValid)
            {
                return ApplicationResult<VerifyOtpResponse>.Failure("Invalid or expired verification code.");
            }

            user.LoggedIn();
            await _userRepository.UpdateAsync(user); // Don't call SaveChanges, unit handles it

            // Commit data changes
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            bool isProfileCompleted = IsUserProfileComplete(user);
            string accessToken = _tokenService.GenerateToken(user);
            string refreshToken = Guid.NewGuid().ToString(); // Placeholder

            var response = new VerifyOtpResponse
            {
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiryMinutes = TokenExpiryMinutes,
                IsRegistered = true,
                RequiresRegistrationCompletion = !isProfileCompleted
            };

            return ApplicationResult<VerifyOtpResponse>.Success(
                response,
                !isProfileCompleted
                    ? "Authentication successful. Please complete your profile."
                    : "Authentication successful."
            );
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);

            return ApplicationResult<VerifyOtpResponse>.Failure($"Verification failed: {ex.Message}", ex);
        }
    }

    private static bool IsUserProfileComplete(User user)
    {
        return user is not null &&
               !string.IsNullOrWhiteSpace(user.FirstName) &&
               !string.IsNullOrWhiteSpace(user.LastName) &&
               user.NationalId != null;
    }
}
