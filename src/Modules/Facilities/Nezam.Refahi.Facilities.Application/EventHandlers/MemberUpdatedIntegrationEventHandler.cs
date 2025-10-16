using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.ReadModels;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Membership.Contracts.IntegrationEvents;

namespace Nezam.Refahi.Facilities.Application.EventHandlers;

/// <summary>
/// Event handler for MemberUpdatedIntegrationEvent in Facilities context
/// Updates member snapshot when a member is updated in Membership context
/// </summary>
public class MemberUpdatedIntegrationEventHandler : INotificationHandler<MemberUpdatedIntegrationEvent>
{
  private readonly IMemberSnapshotService _memberSnapshotService;
  private readonly ILogger<MemberUpdatedIntegrationEventHandler> _logger;

  public MemberUpdatedIntegrationEventHandler(
    IMemberSnapshotService memberSnapshotService,
    ILogger<MemberUpdatedIntegrationEventHandler> logger)
  {
    _memberSnapshotService = memberSnapshotService;
    _logger = logger;
  }

  public async Task Handle(MemberUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Handling MemberUpdatedIntegrationEvent for MemberId: {MemberId}", notification.MemberId);

    try
    {
      var memberSnapshot = new MemberSnapshot
      {
        Id = Guid.NewGuid(), // Generate new ID for snapshot
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
        LastUpdatedAt = notification.UpdatedAt
      };

      await _memberSnapshotService.CreateOrUpdateSnapshotAsync(memberSnapshot, cancellationToken);

      _logger.LogInformation("Member snapshot updated successfully for MemberId: {MemberId}", notification.MemberId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating member snapshot for MemberId: {MemberId}", notification.MemberId);
      throw;
    }
  }
}