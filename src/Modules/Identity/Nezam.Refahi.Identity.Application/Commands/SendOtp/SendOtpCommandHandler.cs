using FluentValidation;
using MediatR;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Domain.Entities;

using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Application.Common.Services;
using Nezam.Refahi.Shared.Application.Ports;

namespace Nezam.Refahi.Identity.Application.Commands.SendOtp;

/// <summary>
/// Handler for the SendOtpCommand. Creates OTP challenges and manages user creation/synchronization.
/// Ensures users with a national code are granted the Engineer role.
/// </summary>
public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, ApplicationResult<SendOtpResponse>>
{
    private readonly IEngineerHttpClient _engineerClient;
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
        IEngineerHttpClient engineerClient,
        IOtpGeneratorService otpGeneratorService,
        IOtpHasherService otpHasherService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IOtpChallengeRepository otpChallengeRepository,
        IValidator<SendOtpCommand> validator,
        IIdentityUnitOfWork unitOfWork)
    {
        _engineerClient = engineerClient ?? throw new ArgumentNullException(nameof(engineerClient));
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
            var validation = await _validator.ValidateAsync(request, cancellationToken);
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

            // If not local, fetch from external and create
            if (user == null)
            {
                var engineer = await _engineerClient.GetByNationalCodeAsync(request.NationalCode);
                if (engineer == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<SendOtpResponse>.Failure(
                        $"No engineer found with national code '{request.NationalCode}'.");
                }

                var dto = engineer.User!;
                // Create user directly using constructor
                user = new User(
                    dto.FirstName,
                    dto.LastName,
                    dto.NationalNumber,
                    dto.PhoneNumber ?? string.Empty
                );
                
                await _userRepository.AddAsync(user, cancellationToken);
            }
            else
            {
                // If local exists, optionally sync updated info from external
                var engineer = await _engineerClient.GetByNationalCodeAsync(request.NationalCode);
                if (engineer != null)
                {
                    var dto = engineer.User!;
                    user.UpdateName(dto.FirstName, dto.LastName);
                    if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                        user.UpdatePhoneNumber(dto.PhoneNumber);
                }
                // Ensure Engineer role assigned
                if (!user.HasRole("Engineer"))
                {
                    // Get Engineer role from repository and assign it
                    var engineerRole = await _roleRepository.GetByNameAsync("Engineer", cancellationToken);
                    if (engineerRole != null)
                    {
                        user.AssignRole(engineerRole);
                    }
                }
                await _userRepository.UpdateAsync(user, cancellationToken);
            }

            // Check phone number
            var phone = user.PhoneNumber.Value;
            if (string.IsNullOrWhiteSpace(phone))
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<SendOtpResponse>.Failure("User has no valid phone number.");
            }

            // Check rate limiting
            var phoneNumber = user.PhoneNumber; // Use existing PhoneNumber value object
            var activeChallengesCount = await _otpChallengeRepository.CountActiveChallengesByPhoneInLastHourAsync(phoneNumber);
            if (activeChallengesCount >= 5) // Max 5 challenges per phone per hour
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<SendOtpResponse>.Failure("Too many OTP requests. Please wait before requesting another.");
            }

            // Clean up old challenges to prevent data redundancy
            // This will remove expired and old challenges automatically
            await _otpChallengeRepository.DeleteExpiredChallengesAsync();
            await _otpChallengeRepository.DeleteOldChallengesAsync(7); // Remove challenges older than 7 days

            // Generate OTP and create challenge
            string otpCode;
            string challengeId;
            string nonce;
            string otpHash;
            
            try
            {
                otpCode = await _otpGeneratorService.GenerateOtpAsync(OtpLength);
                challengeId = Guid.NewGuid().ToString("N");
                nonce = await _otpHasherService.GenerateNonceAsync();
                otpHash = await _otpHasherService.HashAsync(challengeId, phone, otpCode, nonce);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<SendOtpResponse>.Failure($"Failed to generate OTP: {ex.Message}", ex);
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
                challengeId: challengeId,
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
            await _otpChallengeRepository.AddAsync(otpChallenge, cancellationToken);

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
                ChallengeId = challengeId
            };
              var successMsg = userExists
                ? "OTP sent successfully."
                : "OTP sent and new user created.";

            return ApplicationResult<SendOtpResponse>.Success(response, successMsg);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<SendOtpResponse>.Failure($"Failed to send OTP: {ex.Message}", ex);
        }
    }

    private static string MaskPhoneNumber(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 7) return phone;
        return $"{phone[..2]}*****{phone[^4..]}";
    }
}