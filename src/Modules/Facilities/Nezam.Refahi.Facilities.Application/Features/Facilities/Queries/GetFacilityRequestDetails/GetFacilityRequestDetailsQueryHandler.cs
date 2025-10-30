using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Facilities.Domain.Enums;
using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequestDetails;

/// <summary>
/// Handler for getting facility request details
/// </summary>
public class GetFacilityRequestDetailsQueryHandler : IRequestHandler<GetFacilityRequestDetailsQuery, ApplicationResult<FacilityRequestDetailsDto>>
{
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IMapper<Domain.Entities.FacilityRequest, FacilityRequestDetailsDto> _requestDetailsMapper;
    private readonly IValidator<GetFacilityRequestDetailsQuery> _validator;
    private readonly ILogger<GetFacilityRequestDetailsQueryHandler> _logger;

    public GetFacilityRequestDetailsQueryHandler(
        IFacilityRequestRepository requestRepository,
        IValidator<GetFacilityRequestDetailsQuery> validator,
        ILogger<GetFacilityRequestDetailsQueryHandler> logger,
        IMapper<Domain.Entities.FacilityRequest, FacilityRequestDetailsDto> requestDetailsMapper)
    {
        _requestRepository = requestRepository;
        _validator = validator;
        _logger = logger;
        _requestDetailsMapper = requestDetailsMapper;
    }

    public async Task<ApplicationResult<FacilityRequestDetailsDto>> Handle(
        GetFacilityRequestDetailsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<FacilityRequestDetailsDto>.Failure(
                    errors, 
                    "اطلاعات ورودی نامعتبر است");
            }

            // Get facility request with related data
            var facilityRequest = await _requestRepository.GetByIdWithDetailsAsync(
                request.RequestId,
                request.IncludeFacility,
                request.IncludeCycle,
                cancellationToken);

            if (facilityRequest == null)
            {
                return ApplicationResult<FacilityRequestDetailsDto>.Failure(
                    "درخواست تسهیلات مورد نظر یافت نشد");
            }

            // Map to DTO via mapper
            var requestDto = await _requestDetailsMapper.MapAsync(facilityRequest, cancellationToken);

            _logger.LogInformation("Retrieved facility request details for {RequestId}",
                request.RequestId);

            return ApplicationResult<FacilityRequestDetailsDto>.Success(requestDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facility request details for {RequestId}",
                request.RequestId);
            return ApplicationResult<FacilityRequestDetailsDto>.Failure(
                "خطای داخلی در دریافت جزئیات درخواست تسهیلات");
        }
    }

}
