using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Application.Commands.Bills;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using MassTransit;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Commands.CreateBill;

/// <summary>
/// Handler for CreateBillCommand - Creates a new bill in draft status
/// </summary>
public class CreateBillCommandHandler : IRequestHandler<CreateBillCommand, ApplicationResult<CreateBillResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IValidator<CreateBillCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IBus _publishEndpoint;

    public CreateBillCommandHandler(
        IBillRepository billRepository,
        IValidator<CreateBillCommand> validator,
        IFinanceUnitOfWork unitOfWork,
        IBus publishEndpoint)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<ApplicationResult<CreateBillResponse>> Handle(
        CreateBillCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            // Validate request
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<CreateBillResponse>.Failure(errors, "Validation failed");
            }

            //add bill items to ctor and add items from this
            var items = request.Items?.Select(item => new BillItem(
                billId: Guid.NewGuid(),
                title: item.Title,
                description: item.Description,
                unitPrice: Money.FromRials((decimal)item.UnitPriceRials),
                quantity: item.Quantity,
                discountPercentage: item.DiscountPercentage
            )).ToList();

            // Create new bill
            var bill = new Bill(
                referenceTrackingCode:request.ReferenceTrackingCode,
                title: request.Title,
                referenceId: request.ReferenceId,
                billType: request.BillType,
                externalUserId: request.ExternalUserId,
                userFullName: request.UserFullName,
                description: request.Description,
                dueDate: request.DueDate,
                metadata: request.Metadata,
                items: items ?? new List<BillItem>()
            );


            // Save bill
            await _billRepository.AddAsync(bill, cancellationToken: cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            // Publish BillCreatedIntegrationEvent inside unit of work (captured by outbox if configured)
            var billCreatedEvent = new BillCreatedEventMessage()
            {
                BillId = bill.Id,
                BillNumber = bill.BillNumber,
                Status = bill.Status.ToString(),
                IssueDate = bill.IssueDate,
                TrackingCode = request.ReferenceTrackingCode,
                ReferenceId = Guid.TryParse(request.ReferenceId, out var refId) ? refId : Guid.Empty,
                ReferenceType = request.BillType,
                TotalAmountRials = bill.TotalAmount.AmountRials,
                Currency = bill.TotalAmount.Currency,
                ExternalUserId = bill.ExternalUserId,
                UserFullName = bill.UserFullName ?? string.Empty,
                Metadata = bill.Metadata ?? new Dictionary<string, string>()
            };

            await _publishEndpoint.Publish(billCreatedEvent, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
            
            // Prepare response
            var response = new CreateBillResponse
            {
                BillId = bill.Id,
                BillNumber = bill.BillNumber,
                Status = bill.Status.ToString(),
                IssueDate = bill.IssueDate
            };

            return ApplicationResult<CreateBillResponse>.Success(response, "Bill created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CreateBillResponse>.Failure(ex, "Failed to create bill");
        }
    }
}
