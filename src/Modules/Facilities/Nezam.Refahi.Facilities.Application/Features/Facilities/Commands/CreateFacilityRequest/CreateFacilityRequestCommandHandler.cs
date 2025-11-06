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
    private readonly IMemberService _memberService;
    private readonly IFacilityDependencyService _dependencyService;

    public CreateFacilityRequestCommandHandler(
        IFacilityRepository facilityRepository,
        IFacilityCycleRepository cycleRepository,
        IFacilityRequestRepository requestRepository,
        IFacilitiesUnitOfWork unitOfWork,
        IValidator<CreateFacilityRequestCommand> validator,
        ILogger<CreateFacilityRequestCommandHandler> logger,
        IMemberService memberService,
        IFacilityDependencyService dependencyService)
    {
        _facilityRepository = facilityRepository;
        _cycleRepository = cycleRepository;
        _requestRepository = requestRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
        _memberService = memberService;
        _dependencyService = dependencyService;
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
                var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
                if (memberDetail == null)
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

                // Check if user already has an active request for this cycle
                var existingUserRequest = await _requestRepository.GetUserLastRequestForCycleAsync(memberDetail.Id, request.FacilityCycleId, cancellationToken);
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
                            memberDetail.Id, request.NationalNumber, request.FacilityCycleId, existingUserRequest.Status);
                        
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<CreateFacilityRequestResult>.Failure(
                            "شما قبلاً برای این دوره تسهیلات درخواست فعال دارید");
                    }
                }

                // Check member eligibility for cycle based on features and capabilities
                // استخراج Features و Capabilities مورد نیاز از دوره
                var requiredFeatures = cycle.Features.Select(f => f.FeatureId).ToList();
                var requiredCapabilities = cycle.Capabilities.Select(c => c.CapabilityId).ToList();

                // بررسی دسترسی عضو با استفاده از MemberService
                // استفاده از nationalId که قبلاً در خط 76 تعریف شده است
                var eligibilityResult = await _memberService.ValidateMemberEligibilityAsync(
                    nationalId,
                    requiredCapabilities: requiredCapabilities.Any() ? requiredCapabilities : null,
                    requiredFeatures: requiredFeatures.Any() ? requiredFeatures : null,
                    requiredAgencies: null);

                if (!eligibilityResult.IsEligible)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    
                    // اگر پیام خطا موجود باشد، از آن استفاده کن
                    if (eligibilityResult.Errors != null && eligibilityResult.Errors.Any())
                    {
                        var errorMessage = string.Join("; ", eligibilityResult.Errors);
                        return ApplicationResult<CreateFacilityRequestResult>.Failure(errorMessage);
                    }
                    
                    // در غیر این صورت، پیام خطای پیش‌فرض
                    var errorParts = new List<string>();
                    if (!eligibilityResult.HasActiveMembership)
                    {
                        errorParts.Add("عضویت فعال برای شما یافت نشد");
                    }
                    if (eligibilityResult.MissingFeatures != null && eligibilityResult.MissingFeatures.Any())
                    {
                        errorParts.Add($"شما باید حداقل یکی از ویژگی‌های مورد نیاز را داشته باشید: {string.Join(", ", eligibilityResult.MissingFeatures)}");
                    }
                    if (eligibilityResult.MissingCapabilities != null && eligibilityResult.MissingCapabilities.Any())
                    {
                        errorParts.Add($"شما باید حداقل یکی از قابلیت‌های مورد نیاز را داشته باشید: {string.Join(", ", eligibilityResult.MissingCapabilities)}");
                    }
                    
                    var finalErrorMessage = errorParts.Any() 
                        ? string.Join("; ", errorParts)
                        : "شما مجاز به درخواست این تسهیلات نیستید";
                    
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(finalErrorMessage);
                }

                // بررسی محدودیت به دوره‌های قبلی همان وام (RestrictToPreviousCycles)
                if (cycle.RestrictToPreviousCycles)
                {
                    // دریافت همه دوره‌های قبلی همین وام (با StartDate قبل از دوره فعلی)
                    var previousCycles = await _cycleRepository.GetByFacilityIdAsync(cycle.FacilityId, cancellationToken);
                    var previousCycleIds = previousCycles
                        .Where(c => c.StartDate < cycle.StartDate)
                        .Select(c => c.Id)
                        .ToList();

                    if (previousCycleIds.Any())
                    {
                        // بررسی اینکه آیا کاربر در دوره‌های قبلی همین وام درخواست تایید شده دارد
                        var userRequestsForFacility = await _requestRepository.GetByFacilityIdAsync(cycle.FacilityId, cancellationToken);
                        var approvedRequestsInPreviousCycles = userRequestsForFacility
                            .Where(r => r.MemberId == memberDetail.Id && 
                                       previousCycleIds.Contains(r.FacilityCycleId) &&
                                       r.Status == Domain.Enums.FacilityRequestStatus.Approved)
                            .ToList();

                        if (approvedRequestsInPreviousCycles.Any())
                        {
                            await _unitOfWork.RollbackAsync(cancellationToken);
                            return ApplicationResult<CreateFacilityRequestResult>.Failure(
                                "شما قبلاً در دوره‌های قبلی این وام درخواست تایید شده دارید و امکان ثبت درخواست مجدد در این دوره را ندارید");
                        }
                    }
                }

                // بررسی وابستگی‌های دوره (Dependencies)
                if (cycle.Dependencies.Any())
                {
                    // Get user's completed facility IDs for dependency validation
                    var allUserRequests = await _requestRepository.GetByUserIdAsync(memberDetail.Id, cancellationToken);
                    var completedFacilityIds = allUserRequests
                        .Where(r => r.Status == Domain.Enums.FacilityRequestStatus.Completed)
                        .Select(r => r.FacilityId)
                        .Distinct()
                        .ToList();

                    // Use domain service to check if dependencies with MustBeCompleted are satisfied
                    var dependenciesSatisfied = _dependencyService.AreDependenciesSatisfied(
                        cycle.Dependencies,
                        completedFacilityIds);

                    if (!dependenciesSatisfied)
                    {
                        var missingDependencies = cycle.Dependencies
                            .Where(d => d.MustBeCompleted && !completedFacilityIds.Contains(d.RequiredFacilityId))
                            .ToList();

                        if (missingDependencies.Any())
                        {
                            var missingNames = string.Join("، ", missingDependencies.Select(d => d.RequiredFacilityName));
                            await _unitOfWork.RollbackAsync(cancellationToken);
                            return ApplicationResult<CreateFacilityRequestResult>.Failure(
                                $"برای ثبت درخواست در این دوره، باید درخواست شما در وام‌های زیر تکمیل شده باشد: {missingNames}");
                        }
                    }

                    // Check for active requests in dependency facilities (application-specific rule)
                    foreach (var dependency in cycle.Dependencies)
                    {
                        var dependencyFacilityRequests = await _requestRepository.GetByFacilityIdAsync(
                            dependency.RequiredFacilityId, 
                            cancellationToken);

                        var activeRequestsInDependencyFacility = dependencyFacilityRequests
                            .Where(r => r.MemberId == memberDetail.Id &&
                                       r.Status != Domain.Enums.FacilityRequestStatus.Rejected &&
                                       r.Status != Domain.Enums.FacilityRequestStatus.Cancelled &&
                                       r.Status != Domain.Enums.FacilityRequestStatus.BankCancelled &&
                                       r.Status != Domain.Enums.FacilityRequestStatus.Expired &&
                                       r.Status != Domain.Enums.FacilityRequestStatus.Completed)
                            .ToList();

                        if (activeRequestsInDependencyFacility.Any())
                        {
                            await _unitOfWork.RollbackAsync(cancellationToken);
                            return ApplicationResult<CreateFacilityRequestResult>.Failure(
                                $"شما درخواست فعال در وام «{dependency.RequiredFacilityName}» دارید. برای ثبت درخواست در این دوره، ابتدا باید درخواست قبلی خود را تکمیل کنید");
                        }
                    }
                }


                // Check quota availability - UsedQuota is computed from Requests.Count
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

                // Find and validate the selected price option
                var priceOption = cycle.PriceOptions.FirstOrDefault(po => po.Id == request.PriceOptionId && po.IsActive);
                if (priceOption == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreateFacilityRequestResult>.Failure(
                        "گزینه قیمت انتخاب شده یافت نشد یا غیرفعال است");
                }

                // Get the amount from the selected price option
                var requestedAmount = priceOption.Amount;

                // Create facility request using domain constructor
                var facilityRequest = new Domain.Entities.FacilityRequest(
                    cycle.FacilityId, // Use facility ID from cycle
                    request.FacilityCycleId,
                    memberDetail.Id, // Use MemberId instead of ExternalUserId
                    request.PriceOptionId, // Selected price option ID
                    requestedAmount, // Amount from price option
                    memberDetail.FullName, // Use member info for user details
                    memberDetail.NationalCode,
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
                    facilityRequest.Id, cycle.FacilityId, memberDetail.Id, request.NationalNumber);

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
