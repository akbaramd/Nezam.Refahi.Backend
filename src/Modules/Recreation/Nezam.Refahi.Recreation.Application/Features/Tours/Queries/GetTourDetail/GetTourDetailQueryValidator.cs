using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetTourDetail;

/// <summary>
/// Validator for GetTourDetailQuery
/// </summary>
public class GetTourDetailQueryValidator : AbstractValidator<GetTourDetailQuery>
{
    public GetTourDetailQueryValidator()
    {
        RuleFor(x => x.TourId)
            .NotEmpty()
            .WithMessage("شناسه تور الزامی است")
            .NotEqual(Guid.Empty)
            .WithMessage("شناسه تور نامعتبر است");
    }
}
