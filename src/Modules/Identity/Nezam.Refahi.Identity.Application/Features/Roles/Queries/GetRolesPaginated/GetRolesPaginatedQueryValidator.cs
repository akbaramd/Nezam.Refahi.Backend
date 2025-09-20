using FluentValidation;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetRolesPaginated;

public class GetRolesPaginatedQueryValidator : AbstractValidator<GetRolesPaginatedQuery>
{
    public GetRolesPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm))
            .WithMessage("Search term cannot exceed 200 characters");

        RuleFor(x => x.SortBy)
            .Must(sortBy => string.IsNullOrEmpty(sortBy) || 
                          new[] { "Name", "DisplayOrder", "CreatedAt", "UpdatedAt" }
                              .Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort field must be one of: Name, DisplayOrder, CreatedAt, UpdatedAt");

        RuleFor(x => x.SortDirection)
            .Must(direction => string.IsNullOrEmpty(direction) || 
                             new[] { "asc", "desc" }
                                 .Contains(direction, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort direction must be 'asc' or 'desc'");
    }
}