using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Contracts.IntegrationEvents;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Membership.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Application.EventHandlers;

/// <summary>
/// Event handler for UserLoggedInIntegrationEvent from Identity context
/// Syncs user data with external member storage and updates capabilities/features
/// </summary>
public class UserLoggedInIntegrationEventHandler : INotificationHandler<UserLoggedInIntegrationEvent>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMembershipUnitOfWork _unitOfWork;
    private readonly IMemberService _memberService;
    private readonly ILogger<UserLoggedInIntegrationEventHandler> _logger;

    public UserLoggedInIntegrationEventHandler(
        IMemberRepository memberRepository,
        IMembershipUnitOfWork unitOfWork,
        IMemberService memberService,
        ILogger<UserLoggedInIntegrationEventHandler> logger)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(UserLoggedInIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing UserLoggedInIntegrationEvent for UserId: {UserId}, NationalId: {NationalId}", 
                notification.UserId, notification.NationalId);

            // Skip if no NationalId provided
            if (string.IsNullOrWhiteSpace(notification.NationalId))
            {
                _logger.LogWarning("No NationalId provided for UserId: {UserId}, cannot sync member", notification.UserId);
                return;
            }

            var nationalId = new NationalId(notification.NationalId);

            // Check if member already exists for this user
            var existingMember = await _memberRepository.GetByExternalUserIdAsync(notification.UserId, cancellationToken);
            
            if (existingMember != null)
            {
                _logger.LogInformation("Member already exists for UserId: {UserId}, capabilities and features will be synced via MemberService", notification.UserId);
                
                // For existing members, we can trigger a sync by calling GetMemberByNationalCodeAsync
                // This will update capabilities and features from external storage
                var memberDto = await _memberService.GetMemberByNationalCodeAsync(nationalId);
                
                if (memberDto != null)
                {
                    _logger.LogInformation("Successfully synced capabilities and features for existing member UserId: {UserId}", notification.UserId);
                }
                else
                {
                    _logger.LogWarning("Could not sync member data from external storage for UserId: {UserId}, NationalId: {NationalId}", 
                        notification.UserId, notification.NationalId);
                }
            }
            else
            {
                _logger.LogInformation("No existing member found for UserId: {UserId}, creating new member and syncing capabilities/features", notification.UserId);
                
                // Use MemberService to get/create member and sync capabilities/features
                // This will handle the complete sync process including capabilities and features
                var memberDto = await _memberService.GetMemberByNationalCodeAsync(nationalId);
                
                if (memberDto != null)
                {
                    // Update the member with the external user ID
                    var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalId.Value);
                    if (member != null)
                    {
                        member.UpdateExternalUserId(notification.UserId);
                        await _memberRepository.UpdateAsync(member);
                        await _unitOfWork.SaveChangesAsync();
                        
                        _logger.LogInformation("Successfully created and synced member for UserId: {UserId}, MemberId: {MemberId}", 
                            notification.UserId, member.Id);
                    }
                }
                else
                {
                    _logger.LogWarning("Could not create member from external storage for UserId: {UserId}, NationalId: {NationalId}", 
                        notification.UserId, notification.NationalId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UserLoggedInIntegrationEvent for UserId: {UserId}", 
                notification.UserId);
            throw;
        }
    }

}
