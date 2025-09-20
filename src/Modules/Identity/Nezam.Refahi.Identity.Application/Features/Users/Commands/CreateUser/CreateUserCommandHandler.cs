using MediatR;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApplicationResult<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository, IIdentityUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Double-check uniqueness before creating user
            var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
            if (existingUserByPhone != null)
            {
                return ApplicationResult<Guid>.Failure("این شماره موبایل قبلاً ثبت شده است");
            }

            var nationalIdVo = new NationalId(request.NationalId);
            var existingUserByNationalId = await _userRepository.GetByNationalIdAsync(nationalIdVo);
            if (existingUserByNationalId != null)
            {
                return ApplicationResult<Guid>.Failure("این کد ملی قبلاً ثبت شده است");
            }

            // Create new user
            var user = new User(
                firstName: request.FirstName,
                lastName: request.LastName,
                nationalId: request.NationalId,
                phoneNumber: request.PhoneNumber
            );

            // Add user to repository
            await _userRepository.AddAsync(user, cancellationToken:cancellationToken);

            // Assign default Member role to all new users
            var memberRole = await _roleRepository.GetByNameAsync("Member", cancellationToken:cancellationToken);
            if (memberRole != null)
            {
                user.AssignRole(memberRole);
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ApplicationResult<Guid>.Success(
                data: user.Id,
                message: "کاربر با موفقیت ایجاد شد"
            );
        }
        catch (ArgumentException ex)
        {
            return ApplicationResult<Guid>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return ApplicationResult<Guid>.Failure($"خطا در ایجاد کاربر: {ex.Message}");
        }
    }
}