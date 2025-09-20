using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;

/// <summary>
/// Handler for getting paginated list of tours
/// </summary>
public class GetToursPaginatedQueryHandler
    : IRequestHandler<GetToursPaginatedQuery, ApplicationResult<PaginatedResult<TourDto>>>
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourReservationRepository _tourReservationRepository;
    private readonly ITourCapacityRepository _tourCapacityRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemberService _memberService;
    private readonly TourCapacityCalculationService _capacityCalculationService;
    private readonly ILogger<GetToursPaginatedQueryHandler> _logger;

    public GetToursPaginatedQueryHandler(
        ITourRepository tourRepository,
        ITourReservationRepository tourReservationRepository,
        ITourCapacityRepository tourCapacityRepository,
        ICurrentUserService currentUserService,
        IMemberService memberService,
        ILogger<GetToursPaginatedQueryHandler> logger)
    {
        _tourRepository = tourRepository;
        _tourReservationRepository = tourReservationRepository;
        _tourCapacityRepository = tourCapacityRepository;
        _currentUserService = currentUserService;
        _memberService = memberService;
        _capacityCalculationService = new TourCapacityCalculationService(_tourReservationRepository);
        _logger = logger;
    }

    public async Task<ApplicationResult<PaginatedResult<TourDto>>> Handle(
        GetToursPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting paginated tours - Page: {PageNumber}, Size: {PageSize}, Search: {Search}",
                request.PageNumber, request.PageSize, request.Search);

            // Get paginated tours from repository
            var (tours, totalCount) = await GetFilteredToursAsync(request, cancellationToken:cancellationToken);

            // Get user reservations based on current user or provided national number
            Dictionary<Guid, TourReservation> userReservations = new();
            string? nationalNumber = await GetUserNationalNumberAsync( cancellationToken);

            if (!string.IsNullOrWhiteSpace(nationalNumber))
            {
                var tourIds = tours.Select(t => t.Id).ToList();
                var reservations = await _tourReservationRepository.GetByTourIdsAndNationalNumberAsync(
                    tourIds, nationalNumber, cancellationToken:cancellationToken);

                // Keep only the latest reservation per tour (based on ReservationDate)
                userReservations = reservations
                    .GroupBy(r => r.TourId)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.ReservationDate).First());
            }

            // Batch calculate capacity information for better performance
            var tourCapacityInfos = await _capacityCalculationService.CalculateTourCapacitiesAsync(tours, cancellationToken);

            // Get all active capacities for batch processing
            var allActiveCapacities = tours.SelectMany(t => t.GetActiveCapacities()).ToList();
            var capacityInfos = await _capacityCalculationService.CalculateCapacitiesAsync(allActiveCapacities, cancellationToken);

            // Convert to DTOs with pre-calculated capacity information
            var tourDtos = tours.Select(tour =>
            {
                var userReservation = userReservations.GetValueOrDefault(tour.Id);
                var tourCapacityInfo = tourCapacityInfos.GetValueOrDefault(tour.Id);
                return MapToDtoWithCapacityInfo(tour, userReservation, tourCapacityInfo, capacityInfos);
            }).ToList();

            var result = new PaginatedResult<TourDto>
            {
                Items = tourDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
            };

            _logger.LogInformation("Successfully retrieved {Count} tours out of {Total}",
                tourDtos.Count, totalCount);

            return ApplicationResult<PaginatedResult<TourDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting paginated tours");
            return ApplicationResult<PaginatedResult<TourDto>>.Failure(
                "خطا در دریافت لیست تورها رخ داده است");
        }
    }

    private async Task<(IEnumerable<Domain.Entities.Tour> Tours, int TotalCount)> GetFilteredToursAsync(
        GetToursPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        // For now, use the existing repository method and apply filtering in memory
        // This should be optimized by adding proper filtering methods to the repository
        var allTours = await _tourRepository.FindAsync(x=>true);

        // Apply filters
        var query = allTours.AsEnumerable();

        // Filter by active status
        if (request.IsActive.HasValue)
        {
            query = query.Where(t => t.IsActive == request.IsActive.Value);
        }

        // Filter by search term (title)
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLowerInvariant();
            query = query.Where(t =>
                t.Title.ToLowerInvariant().Contains(searchTerm) ||
                t.Description.ToLowerInvariant().Contains(searchTerm));
        }

       

        var enumerable = query as Tour[] ?? query.ToArray();
        var totalCount = enumerable.Length;

        // Apply pagination
        var tours = enumerable
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        return (tours, totalCount);
    }

    private TourDto MapToDtoWithCapacityInfo(
        Domain.Entities.Tour tour, 
        TourReservation? userReservation, 
        TourCapacityInfo? tourCapacityInfo,
        Dictionary<Guid, CapacityInfo> capacityInfos)
    {
        var currentDate = DateTime.UtcNow;

        // Get active capacities
        var activeCapacities = tour.GetActiveCapacities().ToList();
        
        // Use pre-calculated capacity information or fallback
        var maxCapacity = tourCapacityInfo?.MaxCapacity ?? tour.MaxParticipants;
        var remainingCapacity = tourCapacityInfo?.RemainingCapacity ?? maxCapacity;
        var reservedCapacity = tourCapacityInfo?.ReservedCapacity ?? 0;
        var utilizationPercentage = tourCapacityInfo?.UtilizationPercentage ?? 0;
        var isFullyBooked = tourCapacityInfo?.IsFullyBooked ?? false;
        var isNearlyFull = tourCapacityInfo?.IsNearlyFull ?? false;

        // Registration period (calculated from active capacities)
        var registrationStart = activeCapacities.Any() ? activeCapacities.Min(c => c.RegistrationStart) : (DateTime?)null;
        var registrationEnd = activeCapacities.Any() ? activeCapacities.Max(c => c.RegistrationEnd) : (DateTime?)null;

        // Map capacities with pre-calculated information
        var capacityDtos = activeCapacities.Select(capacity =>
        {
            var capacityInfo = capacityInfos.GetValueOrDefault(capacity.Id);
            return MapTourCapacityToDtoWithInfo(capacity, capacityInfo, currentDate);
        }).ToList();

        return new TourDto
        {
            Id = tour.Id,
            Title = tour.Title,
            Description = tour.Description,
            TourStart = tour.TourStart,
            TourEnd = tour.TourEnd,

            // Enhanced capacity information
            MaxCapacity = maxCapacity,
            RemainingCapacity = remainingCapacity,
            ReservedCapacity = reservedCapacity,
            CapacityUtilizationPercentage = utilizationPercentage,
            IsFullyBooked = isFullyBooked,
            IsNearlyFull = isNearlyFull,

            IsActive = tour.IsActive,

            // Age restrictions
            MinAge = tour.MinAge,
            MaxAge = tour.MaxAge,

            // Registration period (calculated from active capacities)
            RegistrationStart = registrationStart,
            RegistrationEnd = registrationEnd,
            IsRegistrationOpen = tour.IsRegistrationOpen(currentDate),

            // Tour capacities collection with detailed info
            Capacities = capacityDtos,

            RequiredCapabilities = tour.MemberCapabilities.Select(x => x.CapabilityId).ToList(),
            RequiredFeatures = tour.MemberFeatures.Select(x => x.FeatureId).ToList(),
            Features = tour.TourFeatures.Select(MapTourFeatureToDto).ToList(),
            RestrictedTours = tour.TourRestrictedTours.Select(trt => MapRestrictedTourToDto(trt.RestrictedTour)).ToList(),
            Pricing = tour.GetActivePricing().Select(MapPricingToDto).ToList(),
            Photos = tour.Photos.Select(MapPhotoToDto).ToList(),
            TotalParticipants = reservedCapacity,
            MainParticipants = tour.GetConfirmedReservations().SelectMany(r => r.Participants).Count(p => p.ParticipantType == Domain.Enums.ParticipantType.Member),
            GuestParticipants = tour.GetConfirmedReservations().SelectMany(r => r.Participants).Count(p => p.ParticipantType == Domain.Enums.ParticipantType.Guest),
            UserReservationStatus = ShouldIncludeReservationInfo(userReservation) ? userReservation?.Status : null,
            UserReservationId = ShouldIncludeReservationInfo(userReservation) ? userReservation?.Id : null,
            CreatedAt = tour.CreatedAt,
            UpdatedAt = tour.LastModifiedAt,
            CreatedBy = tour.CreatedBy,
            UpdatedBy = tour.LastModifiedBy
        };
    }

    private TourCapacityDto MapTourCapacityToDtoWithInfo(
        Domain.Entities.TourCapacity capacity, 
        CapacityInfo? capacityInfo, 
        DateTime currentDate)
    {
        var remainingParticipants = capacityInfo?.RemainingParticipants ?? capacity.MaxParticipants;
        var allocatedParticipants = capacityInfo?.AllocatedParticipants ?? 0;
        var utilizationPercentage = capacityInfo?.UtilizationPercentage ?? 0;
        var isFullyBooked = capacityInfo?.IsFullyBooked ?? false;
        var isNearlyFull = capacityInfo?.IsNearlyFull ?? false;
        var isRegistrationOpen = capacity.IsRegistrationOpen(currentDate);

        // Determine availability status and message
        var (availabilityStatus, availabilityMessage) = GetAvailabilityInfo(
            isRegistrationOpen, isFullyBooked, isNearlyFull, remainingParticipants, capacity.MaxParticipants);

        return new TourCapacityDto
        {
            Id = capacity.Id,
            TourId = capacity.TourId,
            MaxParticipants = capacity.MaxParticipants,
            RemainingParticipants = remainingParticipants,
            AllocatedParticipants = allocatedParticipants,
            UtilizationPercentage = utilizationPercentage,
            MinParticipantsPerReservation = capacity.MinParticipantsPerReservation,
            MaxParticipantsPerReservation = capacity.MaxParticipantsPerReservation,
            
            RegistrationStart = capacity.RegistrationStart,
            RegistrationEnd = capacity.RegistrationEnd,
            IsActive = capacity.IsActive,
            Description = capacity.Description,
            IsRegistrationOpen = isRegistrationOpen,
            IsEffectiveFor = capacity.IsEffectiveFor(currentDate),
            IsFullyBooked = isFullyBooked,
            IsNearlyFull = isNearlyFull,
            
            AvailabilityStatus = availabilityStatus,
            AvailabilityMessage = availabilityMessage
        };
    }

    private async Task<TourDto> MapToDtoAsync(Domain.Entities.Tour tour, TourReservation? userReservation, CancellationToken cancellationToken)
    {
        var currentDate = DateTime.UtcNow;

        // Get active capacities
        var activeCapacities = tour.GetActiveCapacities().ToList();
        
        // Calculate total capacity information
        var maxCapacity = tour.MaxParticipants;
        var totalUtilization = await _tourReservationRepository.GetTourUtilizationAsync(tour.Id, cancellationToken);
        var remainingCapacity = Math.Max(0, maxCapacity - totalUtilization);
        var utilizationPercentage = maxCapacity > 0 ? (double)totalUtilization / maxCapacity * 100 : 0;
        var isFullyBooked = remainingCapacity == 0;
        var isNearlyFull = utilizationPercentage > 80;

        // Registration period (calculated from active capacities)
        var registrationStart = activeCapacities.Any() ? activeCapacities.Min(c => c.RegistrationStart) : (DateTime?)null;
        var registrationEnd = activeCapacities.Any() ? activeCapacities.Max(c => c.RegistrationEnd) : (DateTime?)null;

        // Map capacities with detailed information
        var capacityDtos = new List<TourCapacityDto>();
        foreach (var capacity in activeCapacities)
        {
            var capacityDto = await MapTourCapacityToDtoAsync(capacity, currentDate, cancellationToken);
            capacityDtos.Add(capacityDto);
        }

        return new TourDto
        {
            Id = tour.Id,
            Title = tour.Title,
            Description = tour.Description,
            TourStart = tour.TourStart,
            TourEnd = tour.TourEnd,

            // Enhanced capacity information
            MaxCapacity = maxCapacity,
            RemainingCapacity = remainingCapacity,
            ReservedCapacity = totalUtilization,
            CapacityUtilizationPercentage = Math.Round(utilizationPercentage, 2),
            IsFullyBooked = isFullyBooked,
            IsNearlyFull = isNearlyFull,

            IsActive = tour.IsActive,

            // Age restrictions
            MinAge = tour.MinAge,
            MaxAge = tour.MaxAge,

            // Registration period (calculated from active capacities)
            RegistrationStart = registrationStart,
            RegistrationEnd = registrationEnd,
            IsRegistrationOpen = tour.IsRegistrationOpen(currentDate),

            // Tour capacities collection with detailed info
            Capacities = capacityDtos,

            RequiredCapabilities = tour.MemberCapabilities.Select(x => x.CapabilityId).ToList(),
            RequiredFeatures = tour.MemberFeatures.Select(x => x.FeatureId).ToList(),
            Features = tour.TourFeatures.Select(MapTourFeatureToDto).ToList(),
            RestrictedTours = tour.TourRestrictedTours.Select(trt => MapRestrictedTourToDto(trt.RestrictedTour)).ToList(),
            Pricing = tour.GetActivePricing().Select(MapPricingToDto).ToList(),
            Photos = tour.Photos.Select(MapPhotoToDto).ToList(),
            TotalParticipants = totalUtilization,
            MainParticipants = tour.GetConfirmedReservations().SelectMany(r => r.Participants).Count(p => p.ParticipantType == Domain.Enums.ParticipantType.Member),
            GuestParticipants = tour.GetConfirmedReservations().SelectMany(r => r.Participants).Count(p => p.ParticipantType == Domain.Enums.ParticipantType.Guest),
            UserReservationStatus = ShouldIncludeReservationInfo(userReservation) ? userReservation?.Status : null,
            UserReservationId = ShouldIncludeReservationInfo(userReservation) ? userReservation?.Id : null,
            CreatedAt = tour.CreatedAt,
            UpdatedAt = tour.LastModifiedAt,
            CreatedBy = tour.CreatedBy,
            UpdatedBy = tour.LastModifiedBy
        };
    }

    private static TourPricingDto MapPricingToDto(Domain.Entities.TourPricing pricing)
    {
        return new TourPricingDto
        {
            Id = pricing.Id,
            ParticipantType = pricing.ParticipantType,
            BasePrice = pricing.Price.AmountRials,
            DiscountPercentage = pricing.DiscountPercentage,
            EffectivePrice = pricing.GetEffectivePrice().AmountRials,
            ValidFrom = pricing.ValidFrom,
            ValidTo = pricing.ValidTo,
            IsActive = pricing.IsActive
        };
    }

    private static TourPhotoDto MapPhotoToDto(Domain.Entities.TourPhoto photo)
    {
        return new TourPhotoDto
        {
            Id = photo.Id,
            Url = photo.Url,
            Caption = photo.Caption,
            DisplayOrder = photo.DisplayOrder
        };
    }

    private static TourFeatureDto MapTourFeatureToDto(Domain.Entities.TourFeature tourFeature)
    {
        return new TourFeatureDto
        {
            Id = tourFeature.Id,
            TourId = tourFeature.TourId,
            FeatureId = tourFeature.FeatureId,
            Value = tourFeature.Feature.Name,
            Feature = MapFeatureToDto(tourFeature.Feature)
        };
    }

    private static FeatureDto MapFeatureToDto(Domain.Entities.Feature feature)
    {
        return new FeatureDto
        {
            Id = feature.Id,
            Name = feature.Name,
            Description = feature.Description,
            IconClass = feature.IconClass,
            CategoryId = feature.CategoryId,
            DisplayOrder = feature.DisplayOrder,
            IsActive = feature.IsActive,
            IsRequired = feature.IsRequired,
            DefaultValue = feature.DefaultValue,
            ValidationRules = feature.ValidationRules
        };
    }

    private static RestrictedTourDto MapRestrictedTourToDto(Domain.Entities.Tour restrictedTour)
    {
        return new RestrictedTourDto
        {
            Id = restrictedTour.Id,
            Title = restrictedTour.Title,
            Description = restrictedTour.Description,
            TourStart = restrictedTour.TourStart,
            TourEnd = restrictedTour.TourEnd,
            IsActive = restrictedTour.IsActive
        };
    }

    private async Task<TourCapacityDto> MapTourCapacityToDtoAsync(Domain.Entities.TourCapacity capacity, DateTime currentDate, CancellationToken cancellationToken)
    {
        // Get current utilization for this specific capacity
        var capacityUtilization = await _tourReservationRepository.GetCapacityUtilizationAsync(capacity.Id, cancellationToken);
        var remainingParticipants = Math.Max(0, capacity.MaxParticipants - capacityUtilization);
        var utilizationPercentage = capacity.MaxParticipants > 0 ? (double)capacityUtilization / capacity.MaxParticipants * 100 : 0;
        var isFullyBooked = remainingParticipants == 0;
        var isNearlyFull = utilizationPercentage > 80;
        var isRegistrationOpen = capacity.IsRegistrationOpen(currentDate);

        // Determine availability status and message
        var (availabilityStatus, availabilityMessage) = GetAvailabilityInfo(
            isRegistrationOpen, isFullyBooked, isNearlyFull, remainingParticipants, capacity.MaxParticipants);

        return new TourCapacityDto
        {
            Id = capacity.Id,
            TourId = capacity.TourId,
            MaxParticipants = capacity.MaxParticipants,
            RemainingParticipants = remainingParticipants,
            AllocatedParticipants = capacityUtilization,
            UtilizationPercentage = Math.Round(utilizationPercentage, 2),
            MinParticipantsPerReservation = capacity.MinParticipantsPerReservation,
            MaxParticipantsPerReservation = capacity.MaxParticipantsPerReservation,
            
            RegistrationStart = capacity.RegistrationStart,
            RegistrationEnd = capacity.RegistrationEnd,
            IsActive = capacity.IsActive,
            Description = capacity.Description,
            IsRegistrationOpen = isRegistrationOpen,
            IsEffectiveFor = capacity.IsEffectiveFor(currentDate),
            IsFullyBooked = isFullyBooked,
            IsNearlyFull = isNearlyFull,
            
            AvailabilityStatus = availabilityStatus,
            AvailabilityMessage = availabilityMessage
        };
    }

    private static (string Status, string Message) GetAvailabilityInfo(
        bool isRegistrationOpen, bool isFullyBooked, bool isNearlyFull, int remainingParticipants, int maxParticipants)
    {
        if (!isRegistrationOpen)
            return ("Closed", "ثبت‌نام بسته است");
        
        if (isFullyBooked)
            return ("Full", "ظرفیت تکمیل است");
        
        if (isNearlyFull)
            return ("Nearly Full", $"تنها {remainingParticipants} جا باقی مانده");
        
        return ("Available", $"{remainingParticipants} از {maxParticipants} جا موجود است");
    }
    
    
    
    /// <summary>
    /// Gets the national number for the user - either from request parameter or current authenticated user
    /// </summary>
    /// <param name="requestNationalNumber">National number provided in the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The national number to use for reservation lookup, or null if not found</returns>
    private async Task<string?> GetUserNationalNumberAsync( CancellationToken cancellationToken)
    {


        // If user is authenticated, get their member information
        if (_currentUserService.IsAuthenticated && _currentUserService.UserId.HasValue)
        {
            try
            {
                var member = await _memberService.GetMemberByExternalIdAsync(_currentUserService.UserId.Value.ToString());
                return member?.NationalCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve member information for user {UserId}", _currentUserService.UserId);
            }
        }

        return null;
    }

    /// <summary>
    /// Determines whether to include reservation information based on reservation status.
    /// Only includes info if reservation exists and is not null, cancelled, or expired.
    /// </summary>
    /// <param name="userReservation">The user's reservation for the tour</param>
    /// <returns>True if reservation info should be included, false otherwise</returns>
    private static bool ShouldIncludeReservationInfo(TourReservation? userReservation)
    {
        if (userReservation == null)
            return false;

        // Don't include info for cancelled or expired reservations
        return userReservation.Status != ReservationStatus.Cancelled &&
               userReservation.Status != ReservationStatus.Expired;
    }
}