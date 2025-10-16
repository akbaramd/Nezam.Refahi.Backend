using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.RejectFacilityRequest;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.RejectFacilityRequest;

/// <summary>
/// Handler for rejecting facility requests
/// </summary>
public class RejectFacilityRequestCommandHandler : IRequestHandler<RejectFacilityRequestCommand, ApplicationResult<RejectFacilityRequestResult>>
{
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IFacilitiesUnitOfWork _unitOfWork;
    private readonly IValidator<RejectFacilityRequestCommand> _validator;
    private readonly ILogger<RejectFacilityRequestCommandHandler> _logger;

    public RejectFacilityRequestCommandHandler(
        IFacilityRequestRepository requestRepository,
        IFacilitiesUnitOfWork unitOfWork,
        IValidator<RejectFacilityRequestCommand> validator,
        ILogger<RejectFacilityRequestCommandHandler> logger)
    {
        _requestRepository = requestRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<RejectFacilityRequestResult>> Handle(
        RejectFacilityRequestCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<RejectFacilityRequestResult>.Failure(
                    errors, 
                    "اطلاعات ورودی نامعتبر است");
            }

            // Begin transaction
            await _unitOfWork.BeginAsync(cancellationToken);

            Domain.Entities.FacilityRequest facilityRequest;

            try
            {
                // Get the facility request
                facilityRequest = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);
                if (facilityRequest == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<RejectFacilityRequestResult>.Failure(
                        "درخواست تسهیلات مورد نظر یافت نشد");
                }

                // Check if request can be rejected
                if (!facilityRequest.CanBeRejected())
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<RejectFacilityRequestResult>.Failure(
                        $"درخواست در وضعیت فعلی ({facilityRequest.Status}) قابل رد نیست");
                }

                // Reject the request using domain method
                facilityRequest.Reject(request.Reason);

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

            _logger.LogInformation("Rejected facility request {RequestId} by user {RejectorUserId} with reason: {Reason}",
                request.RequestId, request.RejectorUserId, request.Reason);

            return ApplicationResult<RejectFacilityRequestResult>.Success(new RejectFacilityRequestResult
            {
                RequestId = facilityRequest.Id,
                RequestNumber = facilityRequest.RequestNumber!,
                Status = facilityRequest.Status.ToString(),
                Reason = facilityRequest.RejectionReason!,
                RejectedAt = facilityRequest.RejectedAt!.Value,
                RejectorUserId = request.RejectorUserId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting facility request {RequestId} by user {RejectorUserId}",
                request.RequestId, request.RejectorUserId);
            return ApplicationResult<RejectFacilityRequestResult>.Failure(
                "خطای داخلی در رد درخواست تسهیلات");
        }
    }
}
