using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;

/// <summary>
/// Handler for getting facilities list
/// </summary>
public class GetFacilitiesQueryHandler : IRequestHandler<GetFacilitiesQuery, ApplicationResult<GetFacilitiesResult>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IValidator<GetFacilitiesQuery> _validator;
    private readonly ILogger<GetFacilitiesQueryHandler> _logger;

    public GetFacilitiesQueryHandler(
        IFacilityRepository facilityRepository,
        IValidator<GetFacilitiesQuery> validator,
        ILogger<GetFacilitiesQueryHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<GetFacilitiesResult>> Handle(
        GetFacilitiesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<GetFacilitiesResult>.Failure(
                    errors, 
                    "اطلاعات ورودی نامعتبر است");
            }

            // Build query parameters
            var queryParams = new FacilityQueryParameters
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Type = request.Type,
                Status = request.Status,
                SearchTerm = request.SearchTerm,
                OnlyActive = request.OnlyActive
            };

            // Get facilities
            var facilities = await _facilityRepository.GetFacilitiesAsync(queryParams, cancellationToken);
            var totalCount = await _facilityRepository.GetFacilitiesCountAsync(queryParams, cancellationToken);

            // Map to DTOs
            var facilityDtos = facilities.Select(MapToDto).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            _logger.LogInformation("Retrieved {Count} facilities for page {Page}",
                facilityDtos.Count, request.Page);

            return ApplicationResult<GetFacilitiesResult>.Success(new GetFacilitiesResult
            {
                Facilities = facilityDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facilities");
            return ApplicationResult<GetFacilitiesResult>.Failure(
                "خطای داخلی در دریافت لیست تسهیلات");
        }
    }

    private static FacilityDto MapToDto(Domain.Entities.Facility facility)
    {
        var activeCycles = facility.Cycles.Where(c => c.Status == FacilityCycleStatus.Active).ToList();
        var totalActiveQuota = activeCycles.Sum(c => c.Quota);
        var totalUsedQuota = activeCycles.Sum(c => c.UsedQuota);
        var totalAvailableQuota = totalActiveQuota - totalUsedQuota;
        var quotaUtilizationPercentage = totalActiveQuota > 0 ? (decimal)totalUsedQuota / totalActiveQuota * 100 : 0;

        return new FacilityDto
        {
            Id = facility.Id,
            Name = facility.Name,
            Code = facility.Code,
            Type = facility.Type.ToString(),
            TypeText = EnumTextMappingService.GetFacilityTypeDescription(facility.Type),
            Status = facility.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityStatusDescription(facility.Status),
            IsActive = EnumTextMappingService.IsFacilityActive(facility.Status),
            Description = facility.Description,
            BankInfo = new BankInfoDto
            {
                BankName = facility.BankName,
                BankCode = facility.BankCode,
                BankAccountNumber = facility.BankAccountNumber
            },
            CycleStatistics = new FacilityCycleStatisticsDto
            {
                ActiveCyclesCount = facility.Cycles.Count(c => c.Status == FacilityCycleStatus.Active),
                TotalCyclesCount = facility.Cycles.Count,
                DraftCyclesCount = facility.Cycles.Count(c => c.Status == FacilityCycleStatus.Draft),
                ClosedCyclesCount = facility.Cycles.Count(c => c.Status == FacilityCycleStatus.Closed),
                CompletedCyclesCount = facility.Cycles.Count(c => c.Status == FacilityCycleStatus.Completed),
                CancelledCyclesCount = facility.Cycles.Count(c => c.Status == FacilityCycleStatus.Cancelled),
                TotalActiveQuota = totalActiveQuota,
                TotalUsedQuota = totalUsedQuota,
                TotalAvailableQuota = totalAvailableQuota,
                QuotaUtilizationPercentage = quotaUtilizationPercentage
            },
            Metadata = facility.Metadata,
            CreatedAt = facility.CreatedAt,
            LastModifiedAt = facility.LastModifiedAt
        };
    }
}

