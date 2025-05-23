using FluentValidation;
using MediatR;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Application.Common.Models;
using Nezam.Refahi.Application.Ports;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Enums;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Services;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nezam.Refahi.Application.Features.Auth.Commands.SendOtp
{
    /// <summary>
    /// Handler for the SendOtpCommand. Checks local repo first; falls back to external client and synchronizes.
    /// Ensures users with a national code are granted the Engineer role.
    /// </summary>
    public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, ApplicationResult<SendOtpResponse>>
    {
        private readonly IEngineerHttpClient _engineerClient;
        private readonly IOtpService _otpService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly UserDomainService _userDomainService;
        private readonly IValidator<SendOtpCommand> _validator;
        private readonly IUnitOfWork _unitOfWork;

        private const int ExpiryMinutes = 5;

        public SendOtpCommandHandler(
            IEngineerHttpClient engineerClient,
            IOtpService otpService,
            INotificationService notificationService,
            IUserRepository userRepository,
            UserDomainService userDomainService,
            IValidator<SendOtpCommand> validator,
            IUnitOfWork unitOfWork)
        {
            _engineerClient = engineerClient ?? throw new ArgumentNullException(nameof(engineerClient));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
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
                    user = await _userDomainService.RegisterUserAsync(
                        dto.FirstName,
                        dto.LastName,
                        dto.NationalNumber,
                        dto.PhoneNumber ?? string.Empty
                    );
                    // Grant Engineer role
                    user.AddRole(Role.Engineer);
                    await _userRepository.AddAsync(user);
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
                    if (!user.Role.HasFlag(Role.Engineer))
                    {
                        user.AddRole(Role.Engineer);
                    }
                    await _userRepository.UpdateAsync(user);
                }

                // Generate and send OTP
                var phone = user.PhoneNumber;
                if (string.IsNullOrWhiteSpace(phone))
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<SendOtpResponse>.Failure("User has no valid phone number.");
                }

                var otp = await _otpService.GenerateOtpAsync(phone, request.Purpose, ExpiryMinutes);
                var message = $"Your verification code is {otp}. It expires in {ExpiryMinutes} minutes.";
                await _notificationService.SendSmsAsync(phone, message);

                // Persist changes
                await _unitOfWork.SaveAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                // Prepare response
                var response = new SendOtpResponse
                {
                    ExpiryMinutes = ExpiryMinutes,
                    MaskedPhoneNumber = MaskPhoneNumber(phone),
                    IsRegistered = userExists
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
}