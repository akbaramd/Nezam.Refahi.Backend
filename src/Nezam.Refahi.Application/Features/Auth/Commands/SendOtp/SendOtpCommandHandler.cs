using FluentValidation;
using MediatR;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Application.Common.Models;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Enums;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Services;
using Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nezam.Refahi.Application.Features.Auth.Commands.SendOtp;

/// <summary>
/// Handler for the SendOtpCommand
/// </summary>
public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, ApplicationResult<SendOtpResponse>>
{
    private readonly IOtpService _otpService;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly UserDomainService _userDomainService;
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyQuestionRepository _surveyQuestionRepository;
    private readonly IValidator<SendOtpCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    private const int ExpiryMinutes = 5;

    public SendOtpCommandHandler(
        IOtpService otpService,
        INotificationService notificationService,
        IUserRepository userRepository,
        UserDomainService userDomainService,
        ISurveyRepository surveyRepository,
        ISurveyQuestionRepository surveyQuestionRepository,
        IValidator<SendOtpCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
        _surveyRepository = surveyRepository ?? throw new ArgumentNullException(nameof(surveyRepository));
        _surveyQuestionRepository = surveyQuestionRepository ?? throw new ArgumentNullException(nameof(surveyQuestionRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<SendOtpResponse>> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<SendOtpResponse>.Failure(errors, "Validation failed");
            }

            var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
            bool userExists = user != null;

            if (!userExists)
            {
                user = _userDomainService.CreateUser(request.PhoneNumber);
                await _userRepository.AddAsync(user);
            }

            var otpCode = await _otpService.GenerateOtpAsync(request.PhoneNumber, request.Purpose, ExpiryMinutes);

            var message = $"Your verification code for Nezam Refahi is: {otpCode}. This code will expire in {ExpiryMinutes} minutes.";
            await _notificationService.SendSmsAsync(request.PhoneNumber, message);

            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var response = new SendOtpResponse
            {
                ExpiryMinutes = ExpiryMinutes,
                MaskedPhoneNumber = MaskPhoneNumber(request.PhoneNumber),
                IsRegistered = userExists
            };

            var successMessage = userExists
                ? "Verification code sent successfully."
                : "Verification code sent. New account will be created upon verification.";

            return ApplicationResult<SendOtpResponse>.Success(response, successMessage);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);

            var response = new SendOtpResponse
            {
                ExpiryMinutes = ExpiryMinutes,
                MaskedPhoneNumber = MaskPhoneNumber(request.PhoneNumber),
                IsRegistered = false
            };

            return ApplicationResult<SendOtpResponse>.Failure($"Failed to send verification code: {ex.Message}", ex);
        }
    }

    private static string MaskPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 7)
            return phoneNumber;

        return $"{phoneNumber[..2]}*****{phoneNumber[^4..]}";
    }
}
