using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Queries.Bills;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Queries.GetUserBills;

/// <summary>
/// Validator for GetUserBillsQuery
/// </summary>
public class GetUserBillsQueryValidator : AbstractValidator<GetUserBillsQuery>
{
    private readonly string[] _validSortFields = { "issuedate", "duedate", "totalamount", "status" };
    private readonly string[] _validSortDirections = { "asc", "desc" };

    public GetUserBillsQueryValidator()
    {
        RuleFor(x => x.ExternalUserId)  
            .NotEmpty()
            .WithMessage("User external user ID is required")
            .NotEqual(Guid.Empty)
            .WithMessage("User external user ID cannot be empty");

        RuleFor(x => x.Status)
            .Must(BeValidBillStatus)
            .WithMessage("Invalid bill status. Valid values are: Draft, Issued, PartiallyPaid, FullyPaid, Overdue, Cancelled, Refunded")
            .When(x => !string.IsNullOrEmpty(x.Status));

        RuleFor(x => x.BillType)
            .MaximumLength(50)
            .WithMessage("Bill type cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.BillType));

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .WithMessage($"Invalid sort field. Valid values are: {string.Join(", ", _validSortFields)}");

        RuleFor(x => x.SortDirection)
            .Must(BeValidSortDirection)
            .WithMessage($"Invalid sort direction. Valid values are: {string.Join(", ", _validSortDirections)}");
    }

    private static bool BeValidBillStatus(string? status)
    {
        if (string.IsNullOrEmpty(status))
            return true;

        return Enum.TryParse<Nezam.Refahi.Finance.Domain.Enums.BillStatus>(status, true, out _);
    }

    private bool BeValidSortField(string sortBy)
    {
        return _validSortFields.Contains(sortBy.ToLowerInvariant());
    }

    private bool BeValidSortDirection(string sortDirection)
    {
        return _validSortDirections.Contains(sortDirection.ToLowerInvariant());
    }
}