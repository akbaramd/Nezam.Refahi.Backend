using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Commands.Bills;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using Nezam.Refahi.Shared.Application;
using MassTransit;

namespace Nezam.Refahi.Finance.Application.Consumers;

/// <summary>
/// Consumer that creates a bill upon receiving CreateBillIntegrationEvent and publishes BillCreatedIntegrationEvent
/// </summary>
public class IssueBillMessageConsumer : IConsumer<IssueBillCommandMessage>
{
    private readonly IMediator _mediator;
    private readonly IBus _publishEndpoint;
    private readonly ILogger<IssueBillMessageConsumer> _logger;

    public IssueBillMessageConsumer(
        IMediator mediator,
        IBus publishEndpoint,
        ILogger<IssueBillMessageConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<IssueBillCommandMessage> context)
    {
        var notification = context.Message;
        var cancellationToken = context.CancellationToken;
        _logger.LogInformation(
            "AwaitingBill CreateBillIntegrationEvent for ReferenceType: {ReferenceType}, ReferenceId: {ReferenceId}, TrackingCode: {TrackingCode}",
            notification.ReferenceType, notification.ReferenceId, notification.TrackingCode);

        // Map integration event to CreateBillCommand
        var createCommand = new CreateBillCommand
        {
            Title = string.IsNullOrWhiteSpace(notification.BillTitle)
                ? $"فاکتور رزرو {notification.TrackingCode}"
                : notification.BillTitle,
            ReferenceTrackingCode = notification.TrackingCode,
            ReferenceId = notification.ReferenceId.ToString(),
            BillType = notification.ReferenceType, // internal classification; using entity name
            ExternalUserId = notification.ExternalUserId,
            UserFullName = notification.UserFullName,
            Description = notification.Description,
            DueDate = null,
            Metadata = notification.Metadata,
            Items = notification.Items?.Select(i => new CreateBillItemRequest
            {
                Title = i.Title,
                Description = i.Description,
                UnitPriceRials = i.UnitPriceRials,
                Quantity = i.Quantity,
                DiscountPercentage = i.DiscountPercentage
            }).ToList()
        };

        var createResult = await _mediator.Send(createCommand, cancellationToken);

        if (createResult.IsSuccess)
        {
          await _mediator.Send(new IssueBillCommand() { BillId = createResult.Data!.BillId }, cancellationToken);
        }
    
       
     
    }
}


