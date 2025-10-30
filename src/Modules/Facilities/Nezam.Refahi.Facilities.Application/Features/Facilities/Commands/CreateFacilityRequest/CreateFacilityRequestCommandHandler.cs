using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CreateFacilityRequest;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Domain.Services;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CreateFacilityRequest;

/// <summary>
/// Handler for creating facility requests
/// </summary>
public class CreateFacilityRequestCommandHandler : IRequestHandler<CreateFacilityRequestCommand, ApplicationResult<CreateFacilityRequestResult>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityCycleRepository _cycleRepository;
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IFacilitiesUnitOfWork _unitOfWork;
    private readonly IValidator<CreateFacilityRequestCommand> _validator;
    private readonly ILogger<CreateFacilityRequestCommandHandler> _logger;
    private readonly IMemberInfoService _memberInfoService;
    private readonly FacilityEligibilityDomainService _eligibilityService;

    public CreateFacilityRequestCommandHandler(
        IFacilityRepository facilityRepository,
        IFacilityCycleRepository cycleRepository,
        IFacilityRequestRepository requestRepository,
        IFacilitiesUnitOfWork unitOfWork,
        IValidator<CreateFacilityRequestCommand> validator,
        ILogger<CreateFacilityRequestCommandHandler> logger,
        IMemberInfoService memberInfoService,
        FacilityEligibilityDomainService eligibilityService)
    {
        _facilityRepository = facilityRepository;
        _cycleRepository = cycleRepository;
        _requestRepository = requestRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
        _memberInfoService = memberInfoService;
        _eligibilityService = eligibilityService;
    }

    public async Task<ApplicationResult<CreateFacilityRequestResult>> Handle(
        CreateFacilityRequestCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<CreateFacilityRequestResult>.Failure(
                    errors, 
                    "اطلاعات ورودی نامعتبر است");
            }

            // Begin transaction
            await _unitOfWork.BeginAsync(cancellationToken);

            try
            {
                // Get member info using national number from command
                if (string.IsNullOrWhiteSpace(request.NationalNumber))
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        "شماره ملی الزامی است");
                }

                var nationalId = new NationalId(request.NationalNumber);
                var memberInfo = await _memberInfoService.GetMemberInfoAsync(nationalId);
                if (memberInfo == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        "عضو یافت نشد");
                }

                // Get cycle first to find facility ID
                var cycle = await _cycleRepository.GetWithAllDetailsAsync(request.FacilityCycleId, cancellationToken);
                if (cycle == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        "دوره تسهیلات مورد نظر یافت نشد");
                }

                if (cycle.Status != Domain.Enums.FacilityCycleStatus.Active)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        "دوره تسهیلات مورد نظر فعال نیست");
                }

                // Get facility from cycle
                var facility = await _facilityRepository.GetByIdAsync(cycle.FacilityId, cancellationToken);
                if (facility == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        "تسهیلات مورد نظر یافت نشد");
                }

                if (facility.Status != Domain.Enums.FacilityStatus.Active)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        "تسهیلات مورد نظر فعال نیست");
                }

                // Check if user already has an active request for this cycle
                var existingUserRequest = await _requestRepository.GetUserLastRequestForCycleAsync(memberInfo.Id, request.FacilityCycleId, cancellationToken);
                if (existingUserRequest != null)
                {
                    // Check if the existing request is still active (not completed/rejected/cancelled)
                    var isRequestActive = existingUserRequest.Status != Domain.Enums.FacilityRequestStatus.Rejected &&
                                        existingUserRequest.Status != Domain.Enums.FacilityRequestStatus.Cancelled &&
                                        existingUserRequest.Status != Domain.Enums.FacilityRequestStatus.BankCancelled &&
                                        existingUserRequest.Status != Domain.Enums.FacilityRequestStatus.Expired &&
                                        existingUserRequest.Status != Domain.Enums.FacilityRequestStatus.Completed;

                    if (isRequestActive)
                    {
                        _logger.LogWarning("User {MemberId} (NationalNumber: {NationalNumber}) attempted to create duplicate request for cycle {CycleId}. Existing request status: {Status}",
                            memberInfo.Id, request.NationalNumber, request.FacilityCycleId, existingUserRequest.Status);
                        
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<CreateFacilityRequestResult>.Failure(
                            "شما قبلاً برای این دوره تسهیلات درخواست فعال دارید");
                    }
                }

                // Check member eligibility for facility based on features and capabilities
                var eligibilityResult = _eligibilityService.ValidateMemberEligibilityWithDetails(
                    cycle, 
                    memberInfo.Features ?? new List<string>(), 
                    memberInfo.Capabilities ?? new List<string>());
                if (!eligibilityResult.IsEligible)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        eligibilityResult.ErrorMessage ?? "شما مجاز به درخواست این تسهیلات نیستید");
                }


                // Check quota availability
                if (cycle.UsedQuota >= cycle.Quota)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        "ظرفیت دوره تسهیلات تکمیل شده است");
                }

                // Check for duplicate requests (if idempotency key provided)
                if (!string.IsNullOrEmpty(request.IdempotencyKey))
                {
                    var existingRequest = await _requestRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey!, cancellationToken);
                    if (existingRequest != null)
                    {
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<CreateFacilityRequestResult>.Success(new CreateFacilityRequestResult
                        {
                            RequestId = existingRequest.Id,
                            RequestNumber = existingRequest.RequestNumber!,
                            Status = existingRequest.Status.ToString(),
                            RequestedAmountRials = existingRequest.RequestedAmount.AmountRials,
                            Currency = existingRequest.RequestedAmount.Currency,
                            CreatedAt = existingRequest.CreatedAt
                        });
                    }
                }

                // Create money value object with default currency (IRR)
                var requestedAmount = new Money(request.RequestedAmountRials);

                // Validate amount constraints using cycle limits
                if (cycle.MinAmount != null && requestedAmount.AmountRials < cycle.MinAmount.AmountRials)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        $"مبلغ درخواستی کمتر از حداقل مجاز ({cycle.MinAmount.AmountRials:N0} {cycle.MinAmount.Currency}) است");
                }

                if (cycle.MaxAmount != null && requestedAmount.AmountRials > cycle.MaxAmount.AmountRials)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        $"مبلغ درخواستی بیشتر از حداکثر مجاز ({cycle.MaxAmount.AmountRials:N0} {cycle.MaxAmount.Currency}) است");
                }

                // Create facility request using domain constructor
                var facilityRequest = new Domain.Entities.FacilityRequest(
                    cycle.FacilityId, // Use facility ID from cycle
                    request.FacilityCycleId,
                    memberInfo.Id, // Use MemberId instead of ExternalUserId
                    requestedAmount,
                    memberInfo.FullName, // Use member info for user details
                    memberInfo.NationalCode,
                    request.Description,
                    request.Metadata);

                // Set idempotency key if provided
                if (!string.IsNullOrEmpty(request.IdempotencyKey))
                {
                    facilityRequest.Metadata["IdempotencyKey"] = request.IdempotencyKey;
                }

                // Save the request
                await _requestRepository.AddAsync(facilityRequest, cancellationToken: cancellationToken);

                await _unitOfWork.SaveChangesAsync();
                // Commit transaction
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Created facility request {RequestId} for facility {FacilityId} by member {MemberId} (NationalNumber: {NationalNumber})",
                    facilityRequest.Id, cycle.FacilityId, memberInfo.Id, request.NationalNumber);

                return ApplicationResult<CreateFacilityRequestResult>.Success(new CreateFacilityRequestResult
                {
                    RequestId = facilityRequest.Id,
                    RequestNumber = facilityRequest.RequestNumber!,
                    Status = facilityRequest.Status.ToString(),
                    RequestedAmountRials = facilityRequest.RequestedAmount.AmountRials,
                    Currency = facilityRequest.RequestedAmount.Currency,
                    CreatedAt = facilityRequest.CreatedAt
                });
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating facility request for cycle {CycleId}",
                request.FacilityCycleId);
            return ApplicationResult<CreateFacilityRequestResult>.Failure(
                "خطای داخلی در ایجاد درخواست تسهیلات");
        }
    }
}
