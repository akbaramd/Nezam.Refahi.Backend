using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationDetail;

/// <summary>
/// Validator for GetReservationDetailQuery
/// </summary>
public class GetReservationDetailQueryValidator : AbstractValidator<GetReservationDetailQuery>
{
    public GetReservationDetailQueryValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("شناسه رزرو الزامی است");

     
    }
}