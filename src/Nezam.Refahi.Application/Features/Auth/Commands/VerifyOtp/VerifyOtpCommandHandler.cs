using FluentValidation;
using MediatR;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Application.Common.Models;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Enums;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Services;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nezam.Refahi.Application.Features.Auth.Commands.VerifyOtp
{
    /// <summary>
    /// Handler for the VerifyOtpCommand that verifies OTP by looking up users in local repository via national code.
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
                // Validate command
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return ApplicationResult<VerifyOtpResponse>.Failure(errors, "Validation failed");
                }

                // Lookup user by national code in local repository
                var nationalId = new NationalId(request.NationalCode);
                var user = await _userRepository.GetByNationalIdAsync(nationalId);
                if (user is null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<VerifyOtpResponse>.Failure(
                        $"No user found with national code '{request.NationalCode}'.");
                }

                var phoneNumber = user.PhoneNumber;
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<VerifyOtpResponse>.Failure(
                        "User record does not contain a valid phone number.");
                }

                // Validate OTP against stored code
                var isValid = await _otpService.ValidateOtpAsync(phoneNumber, request.OtpCode, request.Purpose);
                if (!isValid)
                {
                    return ApplicationResult<VerifyOtpResponse>.Failure("Invalid or expired verification code.");
                }

                // Mark user as logged in
                user.LoggedIn();

                // Assign Engineer role if NationalId present and not already assigned
                if (user.NationalId != null && !user.Role.HasFlag(Role.Engineer))
                {
                    user.AddRole(Role.Engineer);
                }

                await _userRepository.UpdateAsync(user);

                await _unitOfWork.SaveAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                // Generate tokens
                bool isProfileCompleted = IsUserProfileComplete(user);
                string accessToken = _tokenService.GenerateToken(user);
                string refreshToken = Guid.NewGuid().ToString();

                var response = new VerifyOtpResponse
                {
                    UserId = user.Id,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiryMinutes = TokenExpiryMinutes,
                    IsRegistered = true,
                    RequiresRegistrationCompletion = !isProfileCompleted
                };

                var successMessage = !isProfileCompleted
                    ? "Authentication successful. Please complete your profile."
                    : "Authentication successful.";

                return ApplicationResult<VerifyOtpResponse>.Success(response, successMessage);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<VerifyOtpResponse>.Failure($"Verification failed: {ex.Message}", ex);
            }
        }

        private static bool IsUserProfileComplete(User user)
        {
            return user != null &&
                   !string.IsNullOrWhiteSpace(user.FirstName) &&
                   !string.IsNullOrWhiteSpace(user.LastName) &&
                   user.NationalId != null;
        }
    }
}
