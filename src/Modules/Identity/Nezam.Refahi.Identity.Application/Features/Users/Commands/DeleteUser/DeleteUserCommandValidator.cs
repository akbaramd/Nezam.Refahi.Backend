using FluentValidation;
using Nezam.Refahi.Identity.Domain.Repositories;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("شناسه کاربر اجباری است")
            .MustAsync(UserMustExist)
            .WithMessage("کاربر با این شناسه یافت نشد");

        RuleFor(x => x.DeleteReason)
            .MaximumLength(500)
            .WithMessage("دلیل حذف نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.DeleteReason));
    }

    private async Task<bool> UserMustExist(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
            return false;

        var user = await _userRepository.FindOneAsync(x=>x.Id==userId, cancellationToken:cancellationToken);
        return user != null;
    }
}