using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Domain.Events;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Application.EventHandlers;

/// <summary>
/// Handles CapabilityUpdatedEvent to update cached capability information in MemberCapability entities
/// </summary>
public class CapabilityUpdatedEventHandler : INotificationHandler<CapabilityUpdatedEvent>
{
    private readonly IMemberCapabilityRepository _memberCapabilityRepository;
    private readonly IMembershipUnitOfWork _unitOfWork;
    private readonly ILogger<CapabilityUpdatedEventHandler> _logger;

    public CapabilityUpdatedEventHandler(
        IMemberCapabilityRepository memberCapabilityRepository,
        IMembershipUnitOfWork unitOfWork,
        ILogger<CapabilityUpdatedEventHandler> logger)
    {
        _memberCapabilityRepository = memberCapabilityRepository ?? throw new ArgumentNullException(nameof(memberCapabilityRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(CapabilityUpdatedEvent notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Handling CapabilityUpdatedEvent for capability {CapabilityId} with name {Name}", 
                notification.CapabilityId, notification.Name);

            // Get all MemberCapability entities that reference this capability
            var memberCapabilities = await _memberCapabilityRepository.GetByCapabilityIdAsync(notification.CapabilityId, cancellationToken);

            if (!memberCapabilities.Any())
            {
                _logger.LogDebug("No MemberCapability entities found for capability {CapabilityId}", notification.CapabilityId);
                return;
            }

            // Update cached capability information in all related MemberCapability entities
            foreach (var memberCapability in memberCapabilities)
            {
                memberCapability.UpdateCapabilityInformation(notification.Name);
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated {Count} MemberCapability entities for capability {CapabilityId}", 
                memberCapabilities.Count(), notification.CapabilityId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling CapabilityUpdatedEvent for capability {CapabilityId}", notification.CapabilityId);
            throw;
        }
    }
}
