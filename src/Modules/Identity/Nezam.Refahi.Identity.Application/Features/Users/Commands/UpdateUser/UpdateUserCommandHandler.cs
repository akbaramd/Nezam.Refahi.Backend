using MediatR;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Contracts.IntegrationEvents;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using MassTransit;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApplicationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateUserCommandHandler(
        IUserRepository userRepository, 
        IIdentityUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<ApplicationResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find the user to update
            var user = await _userRepository.FindOneAsync(x=>x.Id==request.Id, cancellationToken:cancellationToken);
            if (user == null)
            {
                return ApplicationResult.Failure("کاربر یافت نشد");
            }

            // Track changes for integration event
            var changedFields = new List<string>();
            var originalFirstName = user.FirstName;
            var originalLastName = user.LastName;
            var originalPhoneNumber = user.PhoneNumber?.Value;
            var originalNationalId = user.NationalId?.Value;

            // Check if phone number is changing and if new phone number is unique
            if (request.PhoneNumber != user.PhoneNumber?.Value)
            {
                var phoneNumberVo = new PhoneNumber(request.PhoneNumber);
                var existingUserByPhone = await _userRepository.GetByPhoneNumberValueObjectAsync(phoneNumberVo);
                if (existingUserByPhone != null && existingUserByPhone.Id != request.Id)
                {
                    return ApplicationResult.Failure("این شماره موبایل توسط کاربر دیگری استفاده شده است");
                }
            }

            // Check if national ID is changing and if new national ID is unique
            var newNationalIdVo = new NationalId(request.NationalId);
            if (user.NationalId == null || request.NationalId != user.NationalId.Value)
            {
                var existingUserByNationalId = await _userRepository.GetByNationalIdAsync(newNationalIdVo);
                if (existingUserByNationalId != null && existingUserByNationalId.Id != request.Id)
                {
                    return ApplicationResult.Failure("این کد ملی توسط کاربر دیگری استفاده شده است");
                }
            }

            // Update basic information
            user.UpdateName(request.FirstName, request.LastName);
            if (request.FirstName != originalFirstName) changedFields.Add("FirstName");
            if (request.LastName != originalLastName) changedFields.Add("LastName");

            // Update profile with national ID
            user.UpdateProfile(request.FirstName, request.LastName, newNationalIdVo);
            if (request.NationalId != originalNationalId) changedFields.Add("NationalId");

            // Update phone number if changed
            if (request.PhoneNumber != user.PhoneNumber?.Value)
            {
                user.UpdatePhoneNumber(request.PhoneNumber);
                changedFields.Add("PhoneNumber");
            }

            // Update in repository
            await _userRepository.UpdateAsync(user, cancellationToken:cancellationToken);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Publish UserUpdatedIntegrationEvent if any fields changed
            if (changedFields.Any())
            {
                var userUpdatedEvent = new UserUpdatedIntegrationEvent
                {
                    UserId = user.Id,
                    PhoneNumber = user.PhoneNumber?.Value,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    NationalCode = user.NationalId?.Value,
                    IsActive = user.IsActive,
                    IsPhoneVerified = user.IsPhoneVerified,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "System", // TODO: Get from current user context
                    Roles = user.UserRoles.Select(ur => ur.Role?.Name ?? string.Empty).Where(r => !string.IsNullOrEmpty(r)).ToList(),
                    Claims = user.UserClaims.Select(uc => uc.Claim.Type).ToList(),
                    Preferences = user.Preferences.ToDictionary(up => up.Key.Value, up => up.Value.RawValue),
                    ChangedFields = changedFields
                };

                await _publishEndpoint.Publish(userUpdatedEvent, cancellationToken);
            }

            return ApplicationResult.Success("اطلاعات کاربر با موفقیت به‌روزرسانی شد");
        }
        catch (ArgumentException ex)
        {
            return ApplicationResult.Failure(ex, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ApplicationResult.Failure(ex, ex.Message);
        }
        catch (Exception ex)
        {
            return ApplicationResult.Failure(ex, "خطا در به‌روزرسانی اطلاعات کاربر");
        }
    }
}