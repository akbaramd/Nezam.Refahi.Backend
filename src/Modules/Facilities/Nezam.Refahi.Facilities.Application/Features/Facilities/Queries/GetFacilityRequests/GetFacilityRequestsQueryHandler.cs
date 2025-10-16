using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using FacilityInfoDto = Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails.FacilityInfoDto;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;

/// <summary>
/// Handler for getting facility requests
/// </summary>
public class GetFacilityRequestsQueryHandler : IRequestHandler<GetFacilityRequestsQuery, ApplicationResult<GetFacilityRequestsResult>>
{
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IValidator<GetFacilityRequestsQuery> _validator;
    private readonly ILogger<GetFacilityRequestsQueryHandler> _logger;
    private readonly IMemberInfoService _memberInfoService;

    public GetFacilityRequestsQueryHandler(
        IFacilityRequestRepository requestRepository,
        IValidator<GetFacilityRequestsQuery> validator,
        ILogger<GetFacilityRequestsQueryHandler> logger,
        IMemberInfoService memberInfoService)
    {
        _requestRepository = requestRepository;
        _validator = validator;
        _logger = logger;
        _memberInfoService = memberInfoService;
    }

    public async Task<ApplicationResult<GetFacilityRequestsResult>> Handle(
        GetFacilityRequestsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<GetFacilityRequestsResult>.Failure(
                    errors, 
                    "اطلاعات ورودی نامعتبر است");
            }

            // Build query parameters
            var queryParams = new FacilityRequestQueryParameters
            {
                Page = request.Page,
                PageSize = request.PageSize,
                FacilityId = request.FacilityId,
                FacilityCycleId = request.FacilityCycleId,
                MemberId = null, // Will be set below if NationalNumber is provided
                Status = request.Status,
                SearchTerm = request.SearchTerm,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo
            };

            // If NationalNumber is provided, get the MemberId
            if (!string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                var nationalId = new NationalId(request.NationalNumber);
                var memberInfo = await _memberInfoService.GetMemberInfoAsync(nationalId);
                if (memberInfo == null)
                {
                    return ApplicationResult<GetFacilityRequestsResult>.Failure(
                        "عضو یافت نشد");
                }
                queryParams.MemberId = memberInfo.Id;
            }

            // Get requests
            var requests = await _requestRepository.GetFacilityRequestsAsync(queryParams, cancellationToken);
            var totalCount = await _requestRepository.GetFacilityRequestsCountAsync(queryParams, cancellationToken);

            // Map to DTOs
            var requestDtos = requests.Select(MapToDto).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            _logger.LogInformation("Retrieved {Count} facility requests for page {Page}",
                requestDtos.Count, request.Page);

            return ApplicationResult<GetFacilityRequestsResult>.Success(new GetFacilityRequestsResult
            {
                Requests = requestDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facility requests");
            return ApplicationResult<GetFacilityRequestsResult>.Failure(
                "خطای داخلی در دریافت لیست درخواست‌های تسهیلات");
        }
    }

    private static FacilityRequestDto MapToDto(Domain.Entities.FacilityRequest request)
    {
        var now = DateTime.UtcNow;
        var daysSinceCreated = (now - request.CreatedAt).Days;
        var daysUntilBankAppointment = request.BankAppointmentScheduledAt.HasValue 
            ? (int?)(request.BankAppointmentScheduledAt.Value - now).Days 
            : null;
        var isBankAppointmentOverdue = request.BankAppointmentScheduledAt.HasValue 
            && request.BankAppointmentScheduledAt.Value < now 
            && !request.BankAppointmentDate.HasValue;

        return new FacilityRequestDto
        {
            Id = request.Id,
            Facility = new FacilityInfoDto
            {
                Id = request.FacilityId,
                Name = request.Facility.Name,
                Code = request.Facility.Code,
                Type = request.Facility.Type.ToString(),
                TypeText = EnumTextMappingService.GetFacilityTypeDescription(request.Facility.Type),
                Status = request.Facility.Status.ToString(),
                StatusText = EnumTextMappingService.GetFacilityStatusDescription(request.Facility.Status),
                Description = request.Facility.Description,
                BankInfo = !string.IsNullOrWhiteSpace(request.Facility.BankName) ? new BankInfoDto
                {
                    BankName = request.Facility.BankName,
                    BankCode = request.Facility.BankCode,
                    BankAccountNumber = request.Facility.BankAccountNumber
                } : null,
                IsActive = EnumTextMappingService.IsFacilityActive(request.Facility.Status)
            },
            Cycle = new FacilityCycleDto
            {
                Id = request.FacilityCycleId,
                Name = request.FacilityCycle.Name,
                StartDate = request.FacilityCycle.StartDate,
                EndDate = request.FacilityCycle.EndDate,
                DaysUntilEnd = Math.Max(0, (request.FacilityCycle.EndDate - now).Days),
                IsActive = EnumTextMappingService.IsCycleActive(request.FacilityCycle.Status),
                Quota = request.FacilityCycle.Quota,
                UsedQuota = request.FacilityCycle.UsedQuota,
                AvailableQuota = request.FacilityCycle.Quota - request.FacilityCycle.UsedQuota,
                Status = request.FacilityCycle.Status.ToString(),
                StatusText = EnumTextMappingService.GetFacilityCycleStatusDescription(request.FacilityCycle.Status),
                MinAmountRials = request.FacilityCycle.MinAmount?.AmountRials ?? 0,
                MaxAmountRials = request.FacilityCycle.MaxAmount?.AmountRials ?? 0,
                PaymentMonths = request.FacilityCycle.PaymentMonths,
                CooldownDays = request.FacilityCycle.CooldownDays,
                CreatedAt = request.FacilityCycle.CreatedAt
            },
            Applicant = new ApplicantInfoDto
            {
                MemberId = request.MemberId,
                FullName = request.UserFullName,
                NationalId = request.UserNationalId
            },
            RequestedAmountRials = request.RequestedAmount.AmountRials,
            ApprovedAmountRials = request.ApprovedAmount?.AmountRials,
            Currency = request.RequestedAmount.Currency,
            Status = request.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityRequestStatusDescription(request.Status),
            CreatedAt = request.CreatedAt,
            DaysSinceCreated = daysSinceCreated,
            IsInProgress = EnumTextMappingService.IsRequestInProgress(request.Status),
            IsCompleted = EnumTextMappingService.IsRequestCompleted(request.Status),
            IsRejected = EnumTextMappingService.IsRequestRejected(request.Status),
            IsCancelled = EnumTextMappingService.IsRequestCancelled(request.Status)
        };
    }
}

