using MediatR;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

namespace Nezam.Refahi.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Handler for the LogoutCommand
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ISurveyResponseRepository _surveyResponseRepository;
    
    public LogoutCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        ISurveyResponseRepository surveyResponseRepository)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _surveyResponseRepository = surveyResponseRepository;
    }
    
    public async Task<LogoutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // In a real implementation, we would add the token to a blacklist or revoke it
        // For simplicity, we'll just return a success response
        
        // Verify that the user exists
        var user = await _userRepository.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return new LogoutResponse
            {
                IsSuccess = false,
                Message = "User not found."
            };
        }
        
        return new LogoutResponse
        {
            IsSuccess = true,
            Message = "Logged out successfully."
        };
    }
}
