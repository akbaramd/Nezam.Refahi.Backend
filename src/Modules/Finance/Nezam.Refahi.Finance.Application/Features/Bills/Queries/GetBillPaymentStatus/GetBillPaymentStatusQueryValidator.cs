using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Queries.Bills;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Queries.GetBillPaymentStatus;

/// <summary>
/// Validator for GetBillPaymentStatusQuery
/// </summary>
public class GetBillPaymentStatusQueryValidator : AbstractValidator<GetBillPaymentStatusQuery>
{
    public GetBillPaymentStatusQueryValidator()
    {
        RuleFor(x => x.BillId)
            .NotEmpty()
            .WithMessage("Bill ID is required")
            .NotEqual(Guid.Empty)
            .WithMessage("Bill ID cannot be empty");
    }
}

/// <summary>
/// Validator for GetBillPaymentStatusByNumberQuery
/// </summary>
public class GetBillPaymentStatusByNumberQueryValidator : AbstractValidator<GetBillPaymentStatusByNumberQuery>
{
    public GetBillPaymentStatusByNumberQueryValidator()
    {
        RuleFor(x => x.BillNumber)
            .NotEmpty()
            .WithMessage("Bill number is required")
            .MaximumLength(50)
            .WithMessage("Bill number cannot exceed 50 characters");
    }
}

/// <summary>
/// Validator for GetBillPaymentStatusByTrackingCodeQuery
/// </summary>
public class GetBillPaymentStatusByTrackingCodeQueryValidator : AbstractValidator<GetBillPaymentStatusByTrackingCodeQuery>
{
    public GetBillPaymentStatusByTrackingCodeQueryValidator()
    {
        RuleFor(x => x.TrackingCode)
            .NotEmpty()
            .WithMessage("Tracking code is required")
            .MaximumLength(20)
            .WithMessage("Tracking code cannot exceed 20 characters");

        RuleFor(x => x.BillType)
            .NotEmpty()
            .WithMessage("Bill type is required")
            .MaximumLength(50)
            .WithMessage("Bill type cannot exceed 50 characters");
    }
}