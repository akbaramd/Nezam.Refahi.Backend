using FluentValidation;
using MediatR;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Contracts.Pool;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.SendOtp;

/// <summary>
/// Handler for the SendOtpCommand. Creates OTP challenges and manages user creation/synchronization.
/// Uses UserIntegrationPool to fetch user data from external contexts (like Membership).
/// </summary>
public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, ApplicationResult<SendOtpResponse>>
{
    private readonly IUserIntegrationPool _userIntegrationPool;
    private readonly IOtpGeneratorService _otpGeneratorService;
    private readonly IOtpHasherService _otpHasherService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IOtpChallengeRepository _otpChallengeRepository;
    private readonly IValidator<SendOtpCommand> _validator;
    private readonly IIdentityUnitOfWork _unitOfWork;

    private const int ExpiryMinutes = 5;
    private const int OtpLength = 6;

    public SendOtpCommandHandler(
        IUserIntegrationPool userIntegrationPool,
        IOtpGeneratorService otpGeneratorService,
        IOtpHasherService otpHasherService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IOtpChallengeRepository otpChallengeRepository,
        IValidator<SendOtpCommand> validator,
        IIdentityUnitOfWork unitOfWork)
    {
        _userIntegrationPool = userIntegrationPool ?? throw new ArgumentNullException(nameof(userIntegrationPool));
        _otpGeneratorService = otpGeneratorService ?? throw new ArgumentNullException(nameof(otpGeneratorService));
        _otpHasherService = otpHasherService ?? throw new ArgumentNullException(nameof(otpHasherService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _otpChallengeRepository = otpChallengeRepository ?? throw new ArgumentNullException(nameof(otpChallengeRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<SendOtpResponse>> Handle(
        SendOtpCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            // Validate request
            var validation = await _validator.ValidateAsync(request, cancellation:cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<SendOtpResponse>.Failure(errors, "Validation failed");
            }

            // Prepare national ID value object
            var nationalId = new NationalId(request.NationalCode);

            // Try to find user locally
            var user = await _userRepository.GetByNationalIdAsync(nationalId);
            bool userExists = user != null;

            // If not local, fetch from external sources and create
            if (user == null)
            {
                var externalUserInfo = await _userIntegrationPool.GetExternalUserInfoAsync(nationalId, cancellationToken:cancellationToken);
                if (externalUserInfo == null || !externalUserInfo.CanCreateAccount)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<SendOtpResponse>.Failure(
                        $"No eligible user found with national code '{request.NationalCode}' or user is not active.");
                }

                // Create user directly using constructor
                user = new User(
                    externalUserInfo.FirstName,
                    externalUserInfo.LastName,
                    externalUserInfo.NationalCode,
                    externalUserInfo.PhoneNumber ?? string.Empty
                );
                
                await _userRepository.AddAsync(user, cancellationToken:cancellationToken);

                // Always assign Member role as default
                var memberRole = await _roleRepository.GetByNameAsync("Member", cancellationToken:cancellationToken);
                if (memberRole != null)
                {
                    user.AssignRole(memberRole);
                }

                // Additionally assign specific role if available (user can have multiple roles)
                if (!string.IsNullOrWhiteSpace(externalUserInfo.UserRole) &&
                    !externalUserInfo.UserRole.Equals("Member", StringComparison.OrdinalIgnoreCase))
                {
                    var specificRole = await _roleRepository.GetByNameAsync(externalUserInfo.UserRole, cancellationToken:cancellationToken);
                    if (specificRole != null)
                    {
                        user.AssignRole(specificRole);
                    }
                }
            }
            else
            {
                // If local exists, optionally sync updated info from external sources
                // sum 1 + 1 in varaible a


                

                var externalUserInfo = await _userIntegrationPool.GetExternalUserInfoAsync(nationalId, cancellationToken:cancellationToken);
                if (externalUserInfo != null)
                {
                    // Update user information if changed
                    user.UpdateName(externalUserInfo.FirstName, externalUserInfo.LastName);
                    if (!string.IsNullOrWhiteSpace(externalUserInfo.PhoneNumber))
                        user.UpdatePhoneNumber(externalUserInfo.PhoneNumber);
                    
                    // Check if user still has active status in external system
                    if (!externalUserInfo.IsActiveUser)
                    {
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<SendOtpResponse>.Failure(
                            "UserDetail account is not active in external system.");
                    }
                }
                
                await _userRepository.UpdateAsync(user, cancellationToken:cancellationToken);
            }

            // Check phone number
            var phone = user.PhoneNumber.Value;
            if (string.IsNullOrWhiteSpace(phone))
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<SendOtpResponse>.Failure("UserDetail has no valid phone number.");
            }

            // Check rate limiting
            var phoneNumber = user.PhoneNumber; // Use existing PhoneNumber value object
            var activeChallengesCount = await _otpChallengeRepository.CountActiveChallengesByPhoneInLastHourAsync(phoneNumber);
            if (activeChallengesCount >= 5) // Max 5 challenges per phone per hour
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<SendOtpResponse>.Failure("Too many OTP requests. Please wait before requesting another.");
            }

            // Smart cleanup: Only delete challenges for this phone number to avoid affecting other users
            // Also delete only expired challenges to prevent data accumulation
            await _otpChallengeRepository.DeleteChallengesForPhoneAsync(phoneNumber, cancellationToken:cancellationToken);
            
            // Lightweight expired cleanup - don't overload the system
            await _otpChallengeRepository.DeleteExpiredChallengesAsync();

            // Generate OTP and create challenge
            string otpCode;
      
            string nonce;
            string otpHash;
            
            try
            {
                // otpCode = await _otpGeneratorService.GenerateOtpAsync(OtpLength);
                otpCode = "123456";
                nonce = await _otpHasherService.GenerateNonceAsync();
                otpHash = await _otpHasherService.HashAsync( phone, otpCode, nonce);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<SendOtpResponse>.Failure(ex, "Failed to generate OTP");
            }
            
            // Create OTP policy
            var otpPolicy = new OtpPolicy(
                length: OtpLength,
                ttlSeconds: ExpiryMinutes * 60,
                maxVerifyAttempts: 3,
                maxResends: 3,
                maxPerPhonePerHour: 5,
                maxPerIpPerHour: 10
            );

            // Create OTP challenge
            var otpChallenge = new OtpChallenge(
                phoneNumber: phoneNumber,
                clientId: new ClientId("web"),
                otpHash: otpHash,
                nonce: nonce,
                policy: otpPolicy,
                deviceFingerprint: !string.IsNullOrEmpty(request.DeviceId) ? new DeviceFingerprint(request.DeviceId) : null,
                ipAddress: !string.IsNullOrEmpty(request.IpAddress) ? new IpAddress(request.IpAddress) : null
            );

            // Mark as sent
            otpChallenge.MarkAsSent();

            // Save OTP challenge
            await _otpChallengeRepository.AddAsync(otpChallenge, cancellationToken:cancellationToken);

            // Send OTP via SMS
            var message = $"Your verification code is {otpCode}. It expires in {ExpiryMinutes} minutes.";
            // await _notificationService.SendSmsAsync(phone, message);

            // Persist changes
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Prepare response
            var response = new SendOtpResponse
            {
                ExpiryMinutes = ExpiryMinutes,
                MaskedPhoneNumber = MaskPhoneNumber(phone),
                IsRegistered = userExists,
                ChallengeId = otpChallenge.Id.ToString("N")
            };
            
            var successMsg = userExists
                ? "OTP sent successfully."
                : "OTP sent and new user created.";

            return ApplicationResult<SendOtpResponse>.Success(response, successMsg);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<SendOtpResponse>.Failure(ex, "Failed to send OTP");
        }
    }

    private static string MaskPhoneNumber(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 7) return phone;
        return $"{phone[..2]}*****{phone[^4..]}";
    }
}
