using MediatR;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Contracts.Dtos;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQueryHandler 
    : IRequestHandler<GetUsersPaginatedQuery, ApplicationResult<PaginatedResult<UserDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersPaginatedQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApplicationResult<PaginatedResult<UserDto>>> Handle(
        GetUsersPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PageSize <= 0 || request.PageNumber <= 0)
            return ApplicationResult<PaginatedResult<UserDto>>.Failure("Invalid pagination parameters.");

        // Base Query
        var query = await _userRepository.GetPaginatedAsync(new UserPagiantedSpec(request.PageNumber,request.PageSize,request.Search), cancellationToken:cancellationToken);

       
   

        // Map → UserDto
        var items = query.Items.Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            NationalId = u.NationalId?.Value,
            PhoneNumber = u.PhoneNumber.Value,
            IsActive = u.IsActive,
            CreatedAtUtc = u.CreatedAt,
            UpdatedAtUtc = u.LastModifiedAt
        }).ToList();

        var result = new PaginatedResult<UserDto>
        {
            Items = items,
            TotalCount = query.TotalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        return ApplicationResult<PaginatedResult<UserDto>>.Success(result);
    }
}