using MediatR;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApplicationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUserRepository userRepository, IIdentityUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
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

            // Check if phone number is changing and if new phone number is unique
            if (request.PhoneNumber != user.PhoneNumber.Value)
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

            // Update profile with national ID
            user.UpdateProfile(request.FirstName, request.LastName, newNationalIdVo);

            // Update phone number if changed
            if (request.PhoneNumber != user.PhoneNumber.Value)
            {
                user.UpdatePhoneNumber(request.PhoneNumber);
            }

            // Update in repository
            await _userRepository.UpdateAsync(user, cancellationToken:cancellationToken);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ApplicationResult.Success("اطلاعات کاربر با موفقیت به‌روزرسانی شد");
        }
        catch (ArgumentException ex)
        {
            return ApplicationResult.Failure(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ApplicationResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return ApplicationResult.Failure($"خطا در به‌روزرسانی اطلاعات کاربر: {ex.Message}");
        }
    }
}