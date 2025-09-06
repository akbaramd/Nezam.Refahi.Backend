using FluentValidation;
using MediatR;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Application.Ports;

namespace Nezam.Refahi.Identity.Application.Commands.CompleteRegistration;

public class CompleteRegistrationCommandHandler 
    : IRequestHandler<CompleteRegistrationCommand, ApplicationResult<CompleteRegistrationResponse>>
{
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CompleteRegistrationCommand> _validator;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly IEngineerHttpClient _engineerClient;

        public CompleteRegistrationCommandHandler(
            IUserRepository userRepository,
            IValidator<CompleteRegistrationCommand> validator,
            IIdentityUnitOfWork unitOfWork,
            IEngineerHttpClient engineerClient)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _engineerClient = engineerClient ?? throw new ArgumentNullException(nameof(engineerClient));
        }

        public async Task<ApplicationResult<CompleteRegistrationResponse>> Handle(
            CompleteRegistrationCommand request, 
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginAsync(cancellationToken);
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return ApplicationResult<CompleteRegistrationResponse>.Failure(errors, "Validation failed");
                }

                // Verify national number via external service
                var engineer = await _engineerClient.GetByNationalCodeAsync(request.NationalId);
                if (engineer is null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CompleteRegistrationResponse>.Failure(
                        $"National ID '{request.NationalId}' not found in engineer registry.");
                }

                var nationalId = new NationalId(request.NationalId);
                var existingUser = await _userRepository.GetByNationalIdAsync(nationalId);
                if (existingUser != null && existingUser.Id != request.UserId)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CompleteRegistrationResponse>.Failure(
                        "This national ID is already registered with another account.");
                }

                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user is null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CompleteRegistrationResponse>.Failure("User not found.");
                }

                // Security check: Ensure the user has a phone number (OTP verified)
                if (string.IsNullOrWhiteSpace(user.PhoneNumber.Value))
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CompleteRegistrationResponse>.Failure(
                        "User must have a verified phone number to complete registration.");
                }

                // Security check: Ensure the user is not already fully registered
                if (!string.IsNullOrWhiteSpace(user.FirstName) && 
                    !string.IsNullOrWhiteSpace(user.LastName) && 
                    user.NationalId != null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CompleteRegistrationResponse>.Failure(
                        "User profile is already complete.");
                }

                user.UpdateProfile(
                    request.FirstName,
                    request.LastName,
                    nationalId
                );

                await _userRepository.UpdateAsync(user, cancellationToken);

                await _unitOfWork.SaveAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                var response = new CompleteRegistrationResponse
                {
                    Id = user.Id,
                    PhoneNumber = user.PhoneNumber.Value,
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
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CompleteRegistrationResponse>.Failure(
                    $"Failed to update profile: {ex.Message}", ex);
            }
        }

        private static string MaskNationalId(string nationalId)
        {
            if (string.IsNullOrEmpty(nationalId) || nationalId.Length < 6)
                return nationalId;

            return $"{nationalId.Substring(0, 2)}******{nationalId.Substring(nationalId.Length - 2)}";
        }
    }
