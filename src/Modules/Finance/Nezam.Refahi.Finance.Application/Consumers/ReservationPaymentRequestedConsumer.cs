using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Commands.Bills;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Recreation.Contracts.IntegrationEvents;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Application.Consumers;

/// <summary>
/// Consumer for handling reservation payment requests from Recreation module
/// </summary>
public class ReservationPaymentRequestedConsumer : INotificationHandler<ReservationPaymentRequestedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReservationPaymentRequestedConsumer> _logger;
    private readonly IBillRepository _billRepository;

    public ReservationPaymentRequestedConsumer(
        IMediator mediator,
        ILogger<ReservationPaymentRequestedConsumer> logger,
        IBillRepository billRepository)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
    }

    public async Task Handle(ReservationPaymentRequestedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing reservation payment request for reservation {ReservationId}", 
            notification.ReservationId);

        try
        {
            // Check if bill already exists for this tracking code
            var existingBill = await _billRepository.GetByReferenceAsync(
                notification.TrackingCode, 
                notification.BillType, 
                cancellationToken);

            if (existingBill != null)
            {
                _logger.LogInformation("Found existing bill {BillId} for tracking code {TrackingCode}, reusing instead of creating new bill",
                    existingBill.Id, notification.TrackingCode);

                // Check if the existing bill can be reused
                if (CanReuseExistingBill(existingBill, notification))
                {
                    // Issue the existing bill if it's in draft status
                    if (existingBill.Status == BillStatus.Draft)
                    {
                        var issueExistingBillCommand = new IssueBillCommand
                        {
                            BillId = existingBill.Id
                        };

                        var issueExistingResult = await _mediator.Send(issueExistingBillCommand, cancellationToken);

                        if (issueExistingResult.Data == null || !issueExistingResult.IsSuccess)
                        {
                            _logger.LogError("Failed to issue existing bill {BillId} for reservation {ReservationId}: {Errors}", 
                                existingBill.Id, notification.ReservationId, string.Join(", ", issueExistingResult.Errors));
                            return;
                        }

                        _logger.LogInformation("Successfully issued existing bill {BillId} for reservation {ReservationId}", 
                            existingBill.Id, notification.ReservationId);
                    }
                    else
                    {
                        _logger.LogInformation("Existing bill {BillId} is already in {Status} status, no action needed",
                            existingBill.Id, existingBill.Status);
                    }

                    return;
                }
                else
                {
                    _logger.LogWarning("Existing bill {BillId} cannot be reused for reservation {ReservationId}, creating new bill",
                        existingBill.Id, notification.ReservationId);
                }
            }

            // Create new bill if no existing bill found or existing bill cannot be reused
            _logger.LogInformation("Creating new bill for reservation {ReservationId} with tracking code {TrackingCode}",
                notification.ReservationId, notification.TrackingCode);

            var createBillCommand = new CreateBillCommand
            {
                Title = notification.BillTitle,
                ReferenceId = notification.TrackingCode,
                BillType = notification.BillType,
                ExternalUserId = notification.ExternalUserId,
                UserFullName = notification.UserFullName,
                Description = notification.Description,
                DueDate = notification.ExpiryDate,
                Metadata = notification.Metadata,
                Items = notification.BillItems.Select(item => new CreateBillItemRequest
                {
                    Title = item.Title,
                    Description = item.Description,
                    UnitPriceRials = item.UnitPriceRials,
                    Quantity = item.Quantity,
                    DiscountPercentage = item.DiscountPercentage
                }).ToList()
            };

            // Create the bill
            var billResult = await _mediator.Send(createBillCommand, cancellationToken);

            if (billResult.Data == null || !billResult.IsSuccess)
            {
                _logger.LogError("Failed to create bill for reservation {ReservationId}: {Errors}", 
                    notification.ReservationId, string.Join(", ", billResult.Errors));
                return;
            }

            // Issue the bill
            var issueBillCommand = new IssueBillCommand
            {
                BillId = billResult.Data.BillId
            };

            var issueResult = await _mediator.Send(issueBillCommand, cancellationToken);

            if (issueResult.Data == null || !issueResult.IsSuccess)
            {
                _logger.LogError("Failed to issue bill {BillId} for reservation {ReservationId}: {Errors}", 
                    billResult.Data.BillId, notification.ReservationId, string.Join(", ", issueResult.Errors));
                return;
            }

            _logger.LogInformation("Successfully created and issued bill {BillId} for reservation {ReservationId}", 
                billResult.Data.BillId, notification.ReservationId);

            // TODO: Publish BillCreatedForReservationIntegrationEvent to notify Recreation module
            // This would be done through the outbox publisher
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing reservation payment request for reservation {ReservationId}", 
                notification.ReservationId);
            throw;
        }
    }

    /// <summary>
    /// Determines if an existing bill can be reused for the current reservation
    /// </summary>
    private bool CanReuseExistingBill(Domain.Entities.Bill existingBill, ReservationPaymentRequestedIntegrationEvent notification)
    {
        // Check if the bill belongs to the same user
        if (existingBill.ExternalUserId != notification.ExternalUserId)
        {
            _logger.LogWarning("Existing bill {BillId} belongs to different user {ExistingUserId}, expected {ExpectedUserId}",
                existingBill.Id, existingBill.ExternalUserId, notification.ExternalUserId);
            return false;
        }

        // Check if the bill type matches
        if (!string.Equals(existingBill.BillType, notification.BillType, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Existing bill {BillId} has different bill type {ExistingType}, expected {ExpectedType}",
                existingBill.Id, existingBill.BillType, notification.BillType);
            return false;
        }

        // Check if the bill can be reused based on its status
        switch (existingBill.Status)
        {
            case BillStatus.Draft:
                // Draft bills can always be reused
                _logger.LogInformation("Existing bill {BillId} is in Draft status, can be reused", existingBill.Id);
                return true;

            case BillStatus.Issued:
                // Issued bills can be reused if not expired
                if (existingBill.DueDate.HasValue && existingBill.DueDate.Value < DateTime.UtcNow)
                {
                    _logger.LogWarning("Existing bill {BillId} is expired (Due: {DueDate}), cannot be reused",
                        existingBill.Id, existingBill.DueDate.Value);
                    return false;
                }
                _logger.LogInformation("Existing bill {BillId} is in Issued status and not expired, can be reused", existingBill.Id);
                return true;

            case BillStatus.PartiallyPaid:
                // Partially paid bills can be reused for additional payments
                _logger.LogInformation("Existing bill {BillId} is partially paid, can be reused for additional payments", existingBill.Id);
                return true;

            case BillStatus.FullyPaid:
                _logger.LogWarning("Existing bill {BillId} is fully paid, cannot be reused", existingBill.Id);
                return false;

            case BillStatus.Cancelled:
            case BillStatus.Voided:
            case BillStatus.WrittenOff:
            case BillStatus.Credited:
            case BillStatus.Disputed:
            case BillStatus.Refunded:
                _logger.LogWarning("Existing bill {BillId} is in terminal status {Status}, cannot be reused",
                    existingBill.Id, existingBill.Status);
                return false;

            case BillStatus.Overdue:
                // Overdue bills can be reused for payment
                _logger.LogInformation("Existing bill {BillId} is overdue, can be reused for payment", existingBill.Id);
                return true;

            default:
                _logger.LogWarning("Unknown bill status {Status} for bill {BillId}, cannot be reused",
                    existingBill.Status, existingBill.Id);
                return false;
        }
    }
}
