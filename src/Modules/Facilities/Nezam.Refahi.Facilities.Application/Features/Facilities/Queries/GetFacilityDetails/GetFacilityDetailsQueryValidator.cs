using FluentValidation;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;

/// <summary>
/// Validator for GetFacilityDetailsQuery
/// </summary>
public class GetFacilityDetailsQueryValidator : AbstractValidator<GetFacilityDetailsQuery>
{
    public GetFacilityDetailsQueryValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty()
            .WithMessage("شناسه تسهیلات الزامی است");
    }
}
