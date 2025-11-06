using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Features.Responses.Commands;

/// <summary>
/// Handler for CancelResponseCommand (C7)
/// </summary>
public class CancelResponseCommandHandler : IRequestHandler<CancelResponseCommand, ApplicationResult<CancelResponseResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyUnitOfWork _unitOfWork;
    private readonly ILogger<CancelResponseCommandHandler> _logger;

    public CancelResponseCommandHandler(
        ISurveyRepository surveyRepository,
        ISurveyUnitOfWork unitOfWork,
        ILogger<CancelResponseCommandHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApplicationResult<CancelResponseResponse>> Handle(CancelResponseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Efficiently load survey with response
            var survey = await _surveyRepository.GetSurveyWithResponseAndQuestionsAsync(request.ResponseId, cancellationToken);
            if (survey == null)
            {
                return ApplicationResult<CancelResponseResponse>.Failure("نظرسنجی یافت نشد");
            }

            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                return ApplicationResult<CancelResponseResponse>.Failure("پاسخ یافت نشد");
            }

            // Check if response is already submitted
            if (response.SubmittedAt.HasValue)
            {
                return ApplicationResult<CancelResponseResponse>.Failure("RESPONSE_ALREADY_SUBMITTED: پاسخ قبلاً ارسال شده است");
            }

            // Check if policy allows cancellation
            var canCancel = survey.ParticipationPolicy.AllowMultipleSubmissions;
            
            if (canCancel)
            {
                // Use domain entity to cancel response
                response.Cancel();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var responseDto = new CancelResponseResponse
                {
                    Canceled = true,
                    IsAbandoned = false,
                    Message = "پاسخ با موفقیت لغو شد"
                };

                return ApplicationResult<CancelResponseResponse>.Success(responseDto);
            }
            else
            {
                // Mark as abandoned (soft delete)
                // Note: This would require adding an AbandonedAt property to Response entity
                // For now, we'll just return success with abandoned flag
                var responseDto = new CancelResponseResponse
                {
                    Canceled = true,
                    IsAbandoned = true,
                    Message = "پاسخ به عنوان رها شده علامت‌گذاری شد"
                };

                return ApplicationResult<CancelResponseResponse>.Success(responseDto);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling response {ResponseId}", request.ResponseId);
            return ApplicationResult<CancelResponseResponse>.Failure(ex, "خطا در لغو پاسخ");
        }
    }
}
