using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CancelFacilityRequest;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CancelFacilityRequest;

/// <summary>
/// Handler for cancelling facility requests
/// </summary>
public class CancelFacilityRequestCommandHandler : IRequestHandler<CancelFacilityRequestCommand, ApplicationResult<CancelFacilityRequestResult>>
{
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IFacilitiesUnitOfWork _unitOfWork;
    private readonly IValidator<CancelFacilityRequestCommand> _validator;
    private readonly ILogger<CancelFacilityRequestCommandHandler> _logger;

    public CancelFacilityRequestCommandHandler(
        IFacilityRequestRepository requestRepository,
        IFacilitiesUnitOfWork unitOfWork,
        IValidator<CancelFacilityRequestCommand> validator,
        ILogger<CancelFacilityRequestCommandHandler> logger)
    {
        _requestRepository = requestRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<CancelFacilityRequestResult>> Handle(
        CancelFacilityRequestCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<CancelFacilityRequestResult>.Failure(
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
                    return ApplicationResult<CancelFacilityRequestResult>.Failure(
                        "درخواست تسهیلات مورد نظر یافت نشد");
                }

                // بررسی اینکه کاربر فقط می‌تواند درخواست خودش را لغو کند
                if (facilityRequest.MemberId != request.CancelledByUserId)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CancelFacilityRequestResult>.Failure(
                        "شما فقط می‌توانید درخواست خودتان را لغو کنید");
                }

                // Check if request can be cancelled (قبل از تایید)
                if (!facilityRequest.CanBeCancelled())
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CancelFacilityRequestResult>.Failure(
                        $"درخواست در وضعیت فعلی ({facilityRequest.Status}) قابل لغو نیست. فقط درخواست‌های قبل از تایید قابل لغو هستند");
                }

                // Cancel the request using domain method
                facilityRequest.Cancel(request.Reason);

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

            _logger.LogInformation("Cancelled facility request {RequestId} by user {CancelledByUserId}",
                request.RequestId, request.CancelledByUserId);

            return ApplicationResult<CancelFacilityRequestResult>.Success(new CancelFacilityRequestResult
            {
                RequestId = facilityRequest.Id,
                RequestNumber = facilityRequest.RequestNumber!,
                Status = facilityRequest.Status.ToString(),
                Reason = request.Reason,
                CancelledAt = DateTime.UtcNow, // Note: Domain entity doesn't track cancellation time, using current time
                CancelledByUserId = request.CancelledByUserId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling facility request {RequestId} by user {CancelledByUserId}",
                request.RequestId, request.CancelledByUserId);
            return ApplicationResult<CancelFacilityRequestResult>.Failure(
                "خطای داخلی در لغو درخواست تسهیلات");
        }
    }
}
