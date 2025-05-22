using FluentValidation;
using MediatR;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Application.Common.Models;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Services;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nezam.Refahi.Application.Features.Auth.Commands.CompleteRegistration;

/// <summary>
/// Handler for the CompleteRegistrationCommand
/// </summary>
public class CompleteRegistrationCommandHandler : IRequestHandler<CompleteRegistrationCommand, ApplicationResult<CompleteRegistrationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly UserDomainService _userDomainService;
    private readonly IValidator<CompleteRegistrationCommand> _validator;

    public CompleteRegistrationCommandHandler(
        IUserRepository userRepository,
        UserDomainService userDomainService,
        IValidator<CompleteRegistrationCommand> validator)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ApplicationResult<CompleteRegistrationResponse>> Handle(CompleteRegistrationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<CompleteRegistrationResponse>.Failure(errors, "Validation failed");
            }

            // Check if the national ID is already in use by another user
            var nationalId = new NationalId(request.NationalId);
            var existingUserWithNationalId = await _userRepository.GetByNationalIdAsync(nationalId);
            
            if (existingUserWithNationalId != null && existingUserWithNationalId.Id != request.UserId)
            {
                return ApplicationResult<CompleteRegistrationResponse>.Failure("This national ID is already registered with another account.");
            }

            // Get the user from the repository
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return ApplicationResult<CompleteRegistrationResponse>.Failure("User not found.");
            }

            // Update user profile information
            // Following DDD principles, we use the domain service to update the user
            user.UpdateProfile(
                request.FirstName,
                request.LastName,
                nationalId
            );

            // Save the updated user
            await _userRepository.UpdateAsync(user);

            // Create the response
            var response = new CompleteRegistrationResponse
            {
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                FullName = $"{user.FirstName} {user.LastName}",
                Roles = user.GetRoles().Select(r => r.ToString()),
                IsAuthenticated = true,
                MaskedNationalId = MaskNationalId(user.NationalId?.Value ?? string.Empty),
                IsProfileComplete = true
            };

            return ApplicationResult<CompleteRegistrationResponse>.Success(
                response,
                "Profile information updated successfully."
            );
        }
        catch (Exception ex)
        {
            return ApplicationResult<CompleteRegistrationResponse>.Failure($"Failed to update profile: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Masks a national ID for security purposes
    /// </summary>
    private static string MaskNationalId(string nationalId)
    {
        if (string.IsNullOrEmpty(nationalId) || nationalId.Length < 6)
            return nationalId;

        // Show only first 2 and last 2 digits
        return $"{nationalId.Substring(0, 2)}******{nationalId.Substring(nationalId.Length - 2)}";
    }
}
