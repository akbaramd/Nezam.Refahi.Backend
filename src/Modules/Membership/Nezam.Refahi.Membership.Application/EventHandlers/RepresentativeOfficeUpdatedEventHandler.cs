using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Domain.Events;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Application.EventHandlers;

/// <summary>
/// Handles RepresentativeOfficeUpdatedEvent to update cached office information in MemberAgency entities
/// </summary>
public class RepresentativeOfficeUpdatedEventHandler : INotificationHandler<RepresentativeOfficeUpdatedEvent>
{
    private readonly IMemberAgencyRepository _memberAgencyRepository;
    private readonly IMembershipUnitOfWork _unitOfWork;
    private readonly ILogger<RepresentativeOfficeUpdatedEventHandler> _logger;

    public RepresentativeOfficeUpdatedEventHandler(
        IMemberAgencyRepository memberAgencyRepository,
        IMembershipUnitOfWork unitOfWork,
        ILogger<RepresentativeOfficeUpdatedEventHandler> logger)
    {
        _memberAgencyRepository = memberAgencyRepository ?? throw new ArgumentNullException(nameof(memberAgencyRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(RepresentativeOfficeUpdatedEvent notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Handling RepresentativeOfficeUpdatedEvent for office {OfficeId} with code {Code} and name {Name}", 
                notification.OfficeId, notification.Code, notification.Name);

            // Get all MemberAgency entities that reference this office
            var memberAgencies = await _memberAgencyRepository.GetByRepresentativeOfficeIdAsync(notification.OfficeId, cancellationToken);

            if (!memberAgencies.Any())
            {
                _logger.LogDebug("No MemberAgency entities found for office {OfficeId}", notification.OfficeId);
                return;
            }

            // Update cached office information in all related MemberAgency entities
            foreach (var memberAgency in memberAgencies)
            {
                memberAgency.UpdateOfficeInformation(notification.Code, notification.Name);
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated {Count} MemberAgency entities for office {OfficeId}", 
                memberAgencies.Count(), notification.OfficeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling RepresentativeOfficeUpdatedEvent for office {OfficeId}", notification.OfficeId);
            throw;
        }
    }
}
