using MCA.SharedKernel.Application.Contracts;
using MCA.SharedKernel.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Domain.Dtos;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQueryHandler 
    : IRequestHandler<GetUsersPaginatedQuery, ApplicationResult<PaginatedResult<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper<User,UserDto> _userMapper;

    public GetUsersPaginatedQueryHandler(IUserRepository userRepository, IMapper<User,UserDto> userMapper) => (_userRepository, _userMapper) = (userRepository, userMapper);

    public async Task<ApplicationResult<PaginatedResult<UserDto>>> Handle(
        GetUsersPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PageSize <= 0 || request.PageNumber <= 0)
            return ApplicationResult<PaginatedResult<UserDto>>.Failure("Invalid pagination parameters.");

        // Base Query
        var query = await _userRepository.GetPaginatedAsync(new UserPagiantedSpec(request.PageNumber,request.PageSize,request.Search), cancellationToken:cancellationToken);

        // Map → UserDto
        var items = new List<UserDto>();
        foreach (var u in query.Items)
        {
            items.Add(await _userMapper.MapAsync(u, cancellationToken));
        }

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