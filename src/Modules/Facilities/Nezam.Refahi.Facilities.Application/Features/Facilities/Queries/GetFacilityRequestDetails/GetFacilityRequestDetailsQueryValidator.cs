using FluentValidation;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequestDetails;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequestDetails;

/// <summary>
/// Validator for GetFacilityRequestDetailsQuery
/// </summary>
public class GetFacilityRequestDetailsQueryValidator : AbstractValidator<GetFacilityRequestDetailsQuery>
{
    public GetFacilityRequestDetailsQueryValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("شناسه درخواست الزامی است");
    }
}
