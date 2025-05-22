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
    
    // Constants for OTP settings
    private const int ExpiryMinutes = 5;
    
    public SendOtpCommandHandler(
        IOtpService otpService,
        INotificationService notificationService,
        IUserRepository userRepository,
        UserDomainService userDomainService,
        ISurveyRepository surveyRepository,
        ISurveyQuestionRepository surveyQuestionRepository,
        IValidator<SendOtpCommand> validator)
    {
        _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
        _surveyRepository = surveyRepository ?? throw new ArgumentNullException(nameof(surveyRepository));
        _surveyQuestionRepository = surveyQuestionRepository ?? throw new ArgumentNullException(nameof(surveyQuestionRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }
    
    public async Task<ApplicationResult<SendOtpResponse>> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate the request using FluentValidation
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                // Return validation errors
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<SendOtpResponse>.Failure(errors, "Validation failed");
            }
            
            // Check if the user exists
            var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
            bool userExists = user != null;
            string otpCode;
            Guid userId;
            
            if (!userExists)
            {
                // Create a new user with just the phone number and set as not registered
                // Following DDD principles, we use the domain service to create the user
                user = _userDomainService.CreateUser(request.PhoneNumber);
                await _userRepository.AddAsync(user);
                
                userId = user.Id;
            }
            else
            {
                // We've checked that user exists, so it's safe to use non-null assertion
                userId = user!.Id;
            }
            
            // Generate OTP for the user
            otpCode = await _otpService.GenerateOtpAsync(request.PhoneNumber, request.Purpose, ExpiryMinutes);
            
            // Send the OTP via SMS
            var message = $"Your verification code for Nezam Refahi is: {otpCode}. This code will expire in {ExpiryMinutes} minutes.";
            await _notificationService.SendSmsAsync(request.PhoneNumber, message);
            
            // Mask the phone number for the response
            var maskedPhoneNumber = MaskPhoneNumber(request.PhoneNumber);
            
            // Create response data
            var response = new SendOtpResponse
            {
                ExpiryMinutes = ExpiryMinutes,
                MaskedPhoneNumber = maskedPhoneNumber,
                IsRegistered = userExists
            };
            
            // Return successful result
            return ApplicationResult<SendOtpResponse>.Success(
                response,
                userExists ? "Verification code sent successfully." : "Verification code sent. New account will be created upon verification."
            );
        }
        catch (Exception ex)
        {
            // Create response data for error case
            var response = new SendOtpResponse
            {
                ExpiryMinutes = ExpiryMinutes,
                MaskedPhoneNumber = MaskPhoneNumber(request.PhoneNumber),
                IsRegistered = false
            };
            
            // Return failure result with exception
            return ApplicationResult<SendOtpResponse>.Failure($"Failed to send verification code: {ex.Message}", ex);
        }
    }
    
    private static string MaskPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 7)
            return phoneNumber;
            
        // Keep the first 2 and last 4 digits visible, mask the rest
        return $"{phoneNumber[..2]}*****{phoneNumber[^4..]}";
    }
}
