using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.ApproveFacilityRequest;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.ApproveFacilityRequest;

/// <summary>
/// Handler for approving facility requests
/// </summary>
public class ApproveFacilityRequestCommandHandler : IRequestHandler<ApproveFacilityRequestCommand, ApplicationResult<ApproveFacilityRequestResult>>
{
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IFacilitiesUnitOfWork _unitOfWork;
    private readonly IValidator<ApproveFacilityRequestCommand> _validator;
    private readonly ILogger<ApproveFacilityRequestCommandHandler> _logger;

    public ApproveFacilityRequestCommandHandler(
        IFacilityRequestRepository requestRepository,
        IFacilitiesUnitOfWork unitOfWork,
        IValidator<ApproveFacilityRequestCommand> validator,
        ILogger<ApproveFacilityRequestCommandHandler> logger)
    {
        _requestRepository = requestRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<ApproveFacilityRequestResult>> Handle(
        ApproveFacilityRequestCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<ApproveFacilityRequestResult>.Failure(
                    errors, 
                    "اطلاعات ورودی نامعتبر است");
            }

            // Begin transaction
            await _unitOfWork.BeginAsync(cancellationToken);

            Domain.Entities.FacilityRequest facilityRequest;
            Money approvedAmount;

            try
            {
                // Get the facility request
                facilityRequest = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);
                if (facilityRequest == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<ApproveFacilityRequestResult>.Failure(
                        "درخواست تسهیلات مورد نظر یافت نشد");
                }

                // Check if request can be approved
                if (!facilityRequest.CanBeApproved())
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<ApproveFacilityRequestResult>.Failure(
                        $"درخواست در وضعیت فعلی ({facilityRequest.Status}) قابل تأیید نیست");
                }

                // Create approved amount
                approvedAmount = new Money(request.ApprovedAmountRials);

                // Approve the request using domain method
                facilityRequest.Approve(approvedAmount, request.Notes);

                // Update the request
                await _requestRepository.UpdateAsync(facilityRequest);

                // Commit transaction
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }

            _logger.LogInformation("Approved facility request {RequestId} by user {ApproverUserId} with amount {Amount}",
                request.RequestId, request.ApproverUserId, approvedAmount.AmountRials);

            return ApplicationResult<ApproveFacilityRequestResult>.Success(new ApproveFacilityRequestResult
            {
                RequestId = facilityRequest.Id,
                RequestNumber = facilityRequest.RequestNumber!,
                Status = facilityRequest.Status.ToString(),
                ApprovedAmountRials = facilityRequest.ApprovedAmount!.AmountRials,
                Currency = facilityRequest.ApprovedAmount.Currency,
                ApprovedAt = facilityRequest.ApprovedAt!.Value,
                ApproverUserId = request.ApproverUserId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving facility request {RequestId} by user {ApproverUserId}",
                request.RequestId, request.ApproverUserId);
            return ApplicationResult<ApproveFacilityRequestResult>.Failure(
                "خطای داخلی در تأیید درخواست تسهیلات");
        }
    }
}
