using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Contracts.IntegrationEvents;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Membership.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Application.EventHandlers;

/// <summary>
/// Event handler for UserCreatedIntegrationEvent from Identity context
/// Creates a corresponding Member when a User is created, using repository directly for better performance
/// </summary>
public class UserCreatedIntegrationEventHandler : INotificationHandler<UserCreatedIntegrationEvent>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMembershipUnitOfWork _unitOfWork;
    private readonly IMemberService _memberService;
    private readonly ILogger<UserCreatedIntegrationEventHandler> _logger;

    public UserCreatedIntegrationEventHandler(
        IMemberRepository memberRepository,
        IMembershipUnitOfWork unitOfWork,
        IMemberService memberService,
        ILogger<UserCreatedIntegrationEventHandler> logger)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(UserCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
          
            _logger.LogInformation("Processing UserCreatedIntegrationEvent for UserId: {UserId}", notification.UserId);

            // Check if member already exists for this user
            var existingMember = await _memberRepository.GetByExternalUserIdAsync(notification.UserId, cancellationToken);
            if (existingMember != null)
            {
                _logger.LogWarning("Member already exists for UserId: {UserId}", notification.UserId);
                return;
            }

            // Use MemberService to handle member creation/sync - it will add member if not exists
            if (!string.IsNullOrWhiteSpace(notification.NationalId))
            {
                try
                {
                    var nationalId = new NationalId(notification.NationalId);
                    _logger.LogInformation("Calling MemberService to get/sync member by NationalId for UserId: {UserId}, NationalId: {NationalId}", 
                        notification.UserId, notification.NationalId);
                    
                    // MemberService will handle everything: get existing member or create new one
                    var memberDto = await _memberService.GetMemberByNationalCodeAsync(nationalId);
                    
                
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calling MemberService for UserId: {UserId}, NationalId: {NationalId}", 
                        notification.UserId, notification.NationalId);
                    throw;
                }
            }
            else
            {
                _logger.LogWarning("No NationalId provided for UserId: {UserId}, cannot sync member", notification.UserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UserCreatedIntegrationEvent for UserId: {UserId}", 
                notification.UserId);
            throw;
        }
    }

}
