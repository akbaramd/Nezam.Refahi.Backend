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
public class CompleteRegistrationCommandHandler 
    : IRequestHandler<CompleteRegistrationCommand, ApplicationResult<CompleteRegistrationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly UserDomainService _userDomainService;
    private readonly IValidator<CompleteRegistrationCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteRegistrationCommandHandler(
        IUserRepository userRepository,
        UserDomainService userDomainService,
        IValidator<CompleteRegistrationCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

            var nationalId = new NationalId(request.NationalId);
            var existingUser = await _userRepository.GetByNationalIdAsync(nationalId);
            if (existingUser != null && existingUser.Id != request.UserId)
            {
                return ApplicationResult<CompleteRegistrationResponse>.Failure("This national ID is already registered with another account.");
            }

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                return ApplicationResult<CompleteRegistrationResponse>.Failure("User not found.");
            }

            user.UpdateProfile(
                request.FirstName,
                request.LastName,
                nationalId
            );

            await _userRepository.UpdateAsync(user); // SaveChanges inside UoW

            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

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
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CompleteRegistrationResponse>.Failure($"Failed to update profile: {ex.Message}", ex);
        }
    }

    private static string MaskNationalId(string nationalId)
    {
        if (string.IsNullOrEmpty(nationalId) || nationalId.Length < 6)
            return nationalId;

        return $"{nationalId.Substring(0, 2)}******{nationalId.Substring(nationalId.Length - 2)}";
    }
}

