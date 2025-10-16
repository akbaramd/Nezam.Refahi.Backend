using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Membership.Contracts.IntegrationEvents;

namespace Nezam.Refahi.Facilities.Application.EventHandlers;

/// <summary>
/// Event handler for MemberDeactivatedIntegrationEvent in Facilities context
/// Marks member snapshot as inactive when a member is deactivated in Membership context
/// </summary>
public class MemberDeactivatedIntegrationEventHandler : INotificationHandler<MemberDeactivatedIntegrationEvent>
{
  private readonly IMemberSnapshotService _memberSnapshotService;
  private readonly ILogger<MemberDeactivatedIntegrationEventHandler> _logger;

  public MemberDeactivatedIntegrationEventHandler(
    IMemberSnapshotService memberSnapshotService,
    ILogger<MemberDeactivatedIntegrationEventHandler> logger)
  {
    _memberSnapshotService = memberSnapshotService;
    _logger = logger;
  }

  public async Task Handle(MemberDeactivatedIntegrationEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Handling MemberDeactivatedIntegrationEvent for MemberId: {MemberId}", notification.MemberId);

    try
    {
      var existingSnapshot = await _memberSnapshotService.GetSnapshotByMemberIdAsync(notification.MemberId, cancellationToken);
      if (existingSnapshot != null)
      {
        existingSnapshot.IsActive = false;
        existingSnapshot.LastUpdatedAt = notification.DeactivatedAt;
                
        await _memberSnapshotService.CreateOrUpdateSnapshotAsync(existingSnapshot, cancellationToken);
      }

      _logger.LogInformation("Member snapshot deactivated successfully for MemberId: {MemberId}", notification.MemberId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deactivating member snapshot for MemberId: {MemberId}", notification.MemberId);
      throw;
    }
  }
}