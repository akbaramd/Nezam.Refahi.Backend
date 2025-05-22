using MediatR;
using Nezam.Refahi.Application.Common.Exceptions;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;

namespace Nezam.Refahi.Application.Features.Auth.Queries.GetCurrentUser;

/// <summary>
/// Handler for the GetCurrentUserQuery
/// </summary>
public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    
    public GetCurrentUserQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }
    
    public async Task<CurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
        {
            throw new AuthenticationException("User is not authenticated.");
        }
        
        var userId = _currentUserService.UserId.Value;
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            throw new AuthenticationException("User not found.");
        }
        
        return new CurrentUserResponse
        {
            Id = user.Id,
            PhoneNumber = user.PhoneNumber,
            FullName = $"{user.FirstName}  {user.LastName}",
            Roles = _currentUserService.Roles,
            IsAuthenticated = true,
            MaskedNationalId = MaskNationalId(user.NationalId?.Value ?? string.Empty),
            IsProfileComplete = user.FirstName != null && user.LastName != null && user.NationalId != null
        };
    }

    private static string MaskNationalId(string nationalId)
    {
        if (string.IsNullOrEmpty(nationalId) || nationalId.Length < 6)
            return nationalId;  

        // Show only first 2 and last 2 digits
        return $"{nationalId.Substring(0, 2)}******{nationalId.Substring(nationalId.Length - 2)}";
    }
}
