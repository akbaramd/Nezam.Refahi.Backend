using System.Runtime.CompilerServices;
using MediatR;
using Nezam.Refahi.Application.Common.Exceptions;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Application.Common.Models;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;

namespace Nezam.Refahi.Application.Features.Auth.Queries.GetCurrentUser;

/// <summary>
/// Handler for the GetCurrentUserQuery
/// </summary>
public sealed class GetCurrentUserQueryHandler 
    : IRequestHandler<GetCurrentUserQuery, ApplicationResult<CurrentUserResponse>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(ICurrentUserService currentUser, IUserRepository userRepository)
    {
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<ApplicationResult<CurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            return ApplicationResult<CurrentUserResponse>.Failure("User is not authenticated.");

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId.Value);

        if (user is null)
            return ApplicationResult<CurrentUserResponse>.Failure("User not found.");

        var response = new CurrentUserResponse
        {
            Id = user.Id,
            PhoneNumber = user.PhoneNumber,
            FullName = string.Concat(user.FirstName, " ", user.LastName),
            Roles = _currentUser.Roles,
            IsAuthenticated = true,
            MaskedNationalId = Mask(user.NationalId?.Value),
            IsProfileComplete = HasProfile(user)
        };

        return ApplicationResult<CurrentUserResponse>.Success(response);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string Mask(string? nid)
    {
        if (string.IsNullOrWhiteSpace(nid) || nid.Length < 6)
            return nid ?? string.Empty;

        return $"{nid[..2]}******{nid[^2..]}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasProfile(User user) =>
        user.NationalId != null &&
        !string.IsNullOrWhiteSpace(user.FirstName) &&
        !string.IsNullOrWhiteSpace(user.LastName) &&
        !string.IsNullOrWhiteSpace(user.NationalId);
}
