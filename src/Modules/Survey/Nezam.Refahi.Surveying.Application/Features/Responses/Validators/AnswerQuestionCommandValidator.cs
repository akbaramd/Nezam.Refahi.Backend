using FluentValidation;
using Nezam.Refahi.Surveying.Contracts.Commands;

namespace Nezam.Refahi.Surveying.Application.Features.Responses.Validators;

/// <summary>
/// Validator for AnswerQuestionCommand
/// </summary>
public class AnswerQuestionCommandValidator : AbstractValidator<AnswerQuestionCommand>
{
    public AnswerQuestionCommandValidator()
    {
        RuleFor(x => x.ResponseId)
            .NotEmpty()
            .WithMessage("شناسه پاسخ الزامی است");

    

        RuleFor(x => x.TextAnswer)
            .MaximumLength(2000)
            .WithMessage("پاسخ متنی نمی‌تواند بیش از 2000 کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.TextAnswer));

        RuleFor(x => x.SelectedOptionIds)
            .Must(ids => ids == null || ids.Count <= 10)
            .WithMessage("حداکثر 10 گزینه می‌توانید انتخاب کنید")
            .When(x => x.SelectedOptionIds != null);

        RuleForEach(x => x.SelectedOptionIds)
            .NotEmpty()
            .WithMessage("شناسه گزینه نمی‌تواند خالی باشد")
            .When(x => x.SelectedOptionIds != null);

        // At least one answer must be provided
        RuleFor(x => x)
            .Must(command => !string.IsNullOrWhiteSpace(command.TextAnswer) || 
                            (command.SelectedOptionIds != null && command.SelectedOptionIds.Any()))
            .WithMessage("حداقل یک پاسخ (متنی یا گزینه) باید ارائه شود");
    }
}
