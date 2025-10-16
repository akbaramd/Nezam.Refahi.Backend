using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.ReadModels;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Membership.Contracts.IntegrationEvents;

namespace Nezam.Refahi.Facilities.Application.EventHandlers;

/// <summary>
/// Event handler for MemberCreatedIntegrationEvent in Facilities context
/// Creates a member snapshot when a member is created in Membership context
/// </summary>
public class MemberCreatedIntegrationEventHandler : INotificationHandler<MemberCreatedIntegrationEvent>
{
    private readonly IMemberSnapshotService _memberSnapshotService;
    private readonly ILogger<MemberCreatedIntegrationEventHandler> _logger;

    public MemberCreatedIntegrationEventHandler(
        IMemberSnapshotService memberSnapshotService,
        ILogger<MemberCreatedIntegrationEventHandler> logger)
    {
        _memberSnapshotService = memberSnapshotService;
        _logger = logger;
    }

    public async Task Handle(MemberCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling MemberCreatedIntegrationEvent for MemberId: {MemberId}", notification.MemberId);

        try
        {
            var memberSnapshot = new MemberSnapshot
            {
              
                ExternalUserId = notification.ExternalUserId,
                MemberId = notification.MemberId,
                MembershipNumber = notification.MembershipNumber,
                NationalId = notification.NationalId,
                FullName = notification.FullName,
                Email = notification.Email,
                PhoneNumber = notification.PhoneNumber,
                BirthDate = notification.BirthDate,
                Features = notification.Features,
                Capabilities = notification.Capabilities,
                Roles = notification.Roles,
                IsActive = notification.IsActive,
                SnapshotCreatedAt = DateTime.UtcNow,
                LastUpdatedAt = notification.CreatedAt
            };

            await _memberSnapshotService.CreateOrUpdateSnapshotAsync(memberSnapshot, cancellationToken);

            _logger.LogInformation("Member snapshot created successfully for MemberId: {MemberId}", notification.MemberId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating member snapshot for MemberId: {MemberId}", notification.MemberId);
            throw;
        }
    }
}