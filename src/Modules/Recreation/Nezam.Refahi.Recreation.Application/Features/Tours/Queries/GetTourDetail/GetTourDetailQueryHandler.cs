using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Recreation.Application.Services;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetTourDetail;

/// <summary>
/// Handler for getting detailed tour information
/// </summary>
public class GetTourDetailQueryHandler : IRequestHandler<GetTourDetailQuery, ApplicationResult<TourDetailDto>>
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourReservationRepository _tourReservationRepository;
    private readonly ITourCapacityRepository _tourCapacityRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly MemberValidationService _memberValidationService;
    private readonly TourCapacityCalculationService _capacityCalculationService;
    private readonly ILogger<GetTourDetailQueryHandler> _logger;

    public GetTourDetailQueryHandler(
        ITourRepository tourRepository,
        ITourReservationRepository tourReservationRepository,
        ITourCapacityRepository tourCapacityRepository,
        ICurrentUserService currentUserService,
        MemberValidationService memberValidationService,
        ILogger<GetTourDetailQueryHandler> logger)
    {
        _tourRepository = tourRepository;
        _tourReservationRepository = tourReservationRepository;
        _tourCapacityRepository = tourCapacityRepository;
        _currentUserService = currentUserService;
        _memberValidationService = memberValidationService;
        _capacityCalculationService = new TourCapacityCalculationService(_tourReservationRepository);
        _logger = logger;
    }

    public async Task<ApplicationResult<TourDetailDto>> Handle(
        GetTourDetailQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting tour detail for tour {TourId}", request.TourId);

            // Get tour with all related data
            var tour = await GetTourWithIncludesAsync(request.TourId, cancellationToken);
            if (tour == null)
            {
                return ApplicationResult<TourDetailDto>.Failure("تور مورد نظر یافت نشد");
            }

            // Get user information if requested and user is authenticated
            TourReservation? userReservation = null;
            string? userNationalNumber = null;
            
            if (request.IncludeUserInfo && _currentUserService.IsAuthenticated && _currentUserService.UserId.HasValue)
            {
                try
                {
                    var member = await _memberValidationService.GetMemberInfoByExternalIdAsync(_currentUserService.UserId.Value.ToString());
                    if (member != null)
                    {
                        userNationalNumber = member.NationalCode;
                        
                        // Get user's reservation for this tour
                        var userReservations = await _tourReservationRepository.GetByTourIdAndNationalNumberAsync(
                            tour.Id, member.NationalCode, cancellationToken);
                        
                        userReservation = userReservations
                            .Where(r => r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.Expired)
                            .OrderByDescending(r => r.ReservationDate)
                            .FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get user information for tour detail");
                }
            }

            // Map to detailed DTO
            var tourDetailDto = await MapToDetailDtoAsync(tour, userReservation, request, cancellationToken);

            _logger.LogInformation("Successfully retrieved tour detail for tour {TourId}", request.TourId);

            return ApplicationResult<TourDetailDto>.Success(tourDetailDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting tour detail for tour {TourId}", request.TourId);
            return ApplicationResult<TourDetailDto>.Failure(ex, "خطا در دریافت جزئیات تور رخ داده است");
        }
    }

    private async Task<Tour?> GetTourWithIncludesAsync(Guid tourId, CancellationToken cancellationToken)
    {
        // Get tour with all necessary includes
        var tours = await _tourRepository.FindAsync(t => t.Id == tourId);
        return tours.FirstOrDefault();
    }

    private async Task<TourDetailDto> MapToDetailDtoAsync(
        Tour tour,
        TourReservation? userReservation,
        GetTourDetailQuery request,
        CancellationToken cancellationToken)
    {
        var currentDate = DateTime.UtcNow;
        
        // Calculate capacity information
        TourCapacityInfo? tourCapacityInfo = null;
        Dictionary<Guid, CapacityInfo> capacityInfos = new();
        
        if (request.IncludeCapacityDetails)
        {
            var tourCapacityInfos = await _capacityCalculationService.CalculateTourCapacitiesAsync(new[] { tour }, cancellationToken);
            tourCapacityInfo = tourCapacityInfos.GetValueOrDefault(tour.Id);

            var activeCapacities = tour.GetActiveCapacities().ToList();
            if (activeCapacities.Any())
            {
                capacityInfos = await _capacityCalculationService.CalculateCapacitiesAsync(activeCapacities, cancellationToken);
            }
        }

        // Basic tour information
        var dto = new TourDetailDto
        {
            Id = tour.Id,
            Title = tour.Title,
            Description = tour.Description,
            LongDescription = tour.Description, // You can add a separate LongDescription field to Tour entity
            Summary = tour.Description?.Length > 200 ? tour.Description[..200] + "..." : tour.Description,
            
            // Tour dates and timing
            TourStart = tour.TourStart,
            TourEnd = tour.TourEnd,
            Duration = tour.TourEnd - tour.TourStart,
            DurationDays = (int)(tour.TourEnd - tour.TourStart).TotalDays,
            DurationHours = (int)(tour.TourEnd - tour.TourStart).TotalHours,

            // Enhanced capacity information
            MaxCapacity = tourCapacityInfo?.MaxCapacity ?? tour.MaxParticipants,
            RemainingCapacity = tourCapacityInfo?.RemainingCapacity ?? tour.MaxParticipants,
            ReservedCapacity = tourCapacityInfo?.ReservedCapacity ?? 0,
            CapacityUtilizationPercentage = tourCapacityInfo?.UtilizationPercentage ?? 0,
            IsFullyBooked = tourCapacityInfo?.IsFullyBooked ?? false,
            IsNearlyFull = tourCapacityInfo?.IsNearlyFull ?? false,

            IsActive = tour.IsActive,
            IsFeatured = false, // Add this field to Tour entity if needed
            IsPopular = false,  // Add this field to Tour entity if needed

            // Age restrictions
            MinAge = tour.MinAge,
            MaxAge = tour.MaxAge,
            MaxGuestsPerReservation = tour.MaxGuestsPerReservation,
            AgeRestrictionNote = GetAgeRestrictionNote(tour.MinAge, tour.MaxAge),

            // Registration period
            RegistrationStart = tour.GetActiveCapacities().Any() ? tour.GetActiveCapacities().Min(c => c.RegistrationStart) : null,
            RegistrationEnd = tour.GetActiveCapacities().Any() ? tour.GetActiveCapacities().Max(c => c.RegistrationEnd) : null,
            IsRegistrationOpen = tour.IsRegistrationOpen(currentDate),
            RegistrationStatus = GetRegistrationStatus(tour, currentDate),

            CreatedAt = tour.CreatedAt,
            UpdatedAt = tour.LastModifiedAt,
            CreatedBy = tour.CreatedBy,
            UpdatedBy = tour.LastModifiedBy
        };

        // Set capacity status and message
        SetCapacityStatusAndMessage(dto);

        // Include detailed capacity information
        if (request.IncludeCapacityDetails)
        {
            dto.Capacities = await MapCapacityDetailsDtoAsync(tour.GetActiveCapacities(), capacityInfos, currentDate);
        }

        // Include pricing information
        if (request.IncludePricing)
        {
            dto.Pricing = MapPricingDetails(tour.GetActivePricing());
            SetPricingInfo(dto);
        }

        // Include features
        if (request.IncludeFeatures)
        {
            dto.Features = MapFeatureDetails(tour.TourFeatures);
            dto.RequiredCapabilities = tour.MemberCapabilities.Select(x => x.CapabilityId).ToList();
            dto.RequiredFeatures = tour.MemberFeatures.Select(x => x.FeatureId).ToList();
        }

        // Include media
        if (request.IncludeMedia)
        {
            dto.Photos = MapPhotoDetails(tour.Photos);
            dto.MainPhotoUrl = tour.Photos.OrderBy(p => p.DisplayOrder).FirstOrDefault()?.Url;
            dto.GalleryUrls = tour.Photos.OrderBy(p => p.DisplayOrder).Select(p => p.Url).ToList();
        }

        // Include restricted tours
        if (request.IncludeRestrictions)
        {
            dto.RestrictedTours = MapRestrictedTourDetails(tour.TourRestrictedTours);
        }

        // Include statistics
        if (request.IncludeStatistics)
        {
            await SetStatisticsAsync(dto, tour, cancellationToken);
        }

        // Include user-specific information
        if (request.IncludeUserInfo && userReservation != null)
        {
            SetUserReservationInfo(dto, userReservation);
        }

        // Set reservation eligibility
        dto.CanUserReserve = CanUserReserve(tour, userReservation, currentDate);
        dto.ReservationBlockReasons = GetReservationBlockReasons(tour, userReservation, currentDate);

        return dto;
    }

    private Task<List<TourCapacityDetailDto>> MapCapacityDetailsDtoAsync(
        IEnumerable<TourCapacity> capacities,
        Dictionary<Guid, CapacityInfo> capacityInfos,
        DateTime currentDate)
    {
        var result = new List<TourCapacityDetailDto>();
        
        foreach (var capacity in capacities)
        {
            var capacityInfo = capacityInfos.GetValueOrDefault(capacity.Id);
            var (status, message) = GetAvailabilityInfo(
                capacity.IsRegistrationOpen(currentDate),
                capacityInfo?.IsFullyBooked ?? false,
                capacityInfo?.IsNearlyFull ?? false,
                capacityInfo?.RemainingParticipants ?? capacity.MaxParticipants,
                capacity.MaxParticipants);

            result.Add(new TourCapacityDetailDto
            {
                Id = capacity.Id,
                TourId = capacity.TourId,
                Name = capacity.Description,
                Description = capacity.Description,
                MaxParticipants = capacity.MaxParticipants,
                RemainingParticipants = capacityInfo?.RemainingParticipants ?? capacity.MaxParticipants,
                AllocatedParticipants = capacityInfo?.AllocatedParticipants ?? 0,
                UtilizationPercentage = capacityInfo?.UtilizationPercentage ?? 0,
                MinParticipantsPerReservation = capacity.MinParticipantsPerReservation,
                MaxParticipantsPerReservation = capacity.MaxParticipantsPerReservation,
                
                RegistrationStart = capacity.RegistrationStart,
                RegistrationEnd = capacity.RegistrationEnd,
                IsActive = capacity.IsActive,
                IsRegistrationOpen = capacity.IsRegistrationOpen(currentDate),
                IsEffectiveFor = capacity.IsEffectiveFor(currentDate),
                IsFullyBooked = capacityInfo?.IsFullyBooked ?? false,
                IsNearlyFull = capacityInfo?.IsNearlyFull ?? false,
                
                AvailabilityStatus = status,
                AvailabilityMessage = message,
                DisplayOrder = 0 // Add this field to TourCapacity if needed
            });
        }

        return Task.FromResult(result.OrderBy(c => c.DisplayOrder).ToList());
    }

    private List<TourPricingDetailDto> MapPricingDetails(IEnumerable<TourPricing> pricings)
    {
        return pricings.Select(p => new TourPricingDetailDto
        {
            Id = p.Id,
            ParticipantType = p.ParticipantType,
            ParticipantTypeName = GetParticipantTypeName(p.ParticipantType),
            BasePrice = p.Price.AmountRials,
            DiscountPercentage = p.DiscountPercentage,
            EffectivePrice = p.GetEffectivePrice().AmountRials,
            DiscountAmount = p.DiscountPercentage > 0 ? p.Price.AmountRials * (p.DiscountPercentage / 100) : null,
            ValidFrom = p.ValidFrom,
            ValidTo = p.ValidTo,
            IsActive = p.IsActive,
            Currency = "ریال",
            IsEarlyBird = p.DiscountPercentage > 0 && p.ValidTo > DateTime.UtcNow.AddDays(30),
            IsLastMinute = p.DiscountPercentage > 0 && p.ValidTo <= DateTime.UtcNow.AddDays(7)
        }).ToList();
    }

    private List<TourFeatureDetailDto> MapFeatureDetails(IEnumerable<TourFeature> tourFeatures)
    {
        return tourFeatures.Select(tf => new TourFeatureDetailDto
        {
            Id = tf.Id,
            TourId = tf.TourId,
            FeatureId = tf.FeatureId,
            Name = tf.Feature.Name,
            Description = tf.Feature.Description,
            Value = tf.Feature.Name,
            IconClass = tf.Feature.IconClass,
            DisplayOrder = tf.Feature.DisplayOrder,
            IsRequired = tf.Feature.IsRequired,
            IsHighlight = tf.Feature.IsRequired, // You can add IsHighlight field
            Category = tf.Feature.Category?.Name
        }).OrderBy(f => f.DisplayOrder).ToList();
    }

    private List<TourPhotoDetailDto> MapPhotoDetails(IEnumerable<TourPhoto> photos)
    {
        return photos.Select(p => new TourPhotoDetailDto
        {
            Id = p.Id,
            Url = p.Url,
            ThumbnailUrl = p.Url, // Add thumbnail field if needed
            Caption = p.Caption,
            AltText = p.Caption,
            DisplayOrder = p.DisplayOrder,
            IsMain = p.DisplayOrder == 0,
            IsPublic = true,
            ContentType = "image/jpeg" // Add this field if needed
        }).OrderBy(p => p.DisplayOrder).ToList();
    }

    private List<RestrictedTourDetailDto> MapRestrictedTourDetails(IEnumerable<TourRestrictedTour> restrictedTours)
    {
        return restrictedTours.Select(rt => new RestrictedTourDetailDto
        {
            Id = rt.RestrictedTour.Id,
            Title = rt.RestrictedTour.Title,
            Description = rt.RestrictedTour.Description,
            TourStart = rt.RestrictedTour.TourStart,
            TourEnd = rt.RestrictedTour.TourEnd,
            IsActive = rt.RestrictedTour.IsActive,
            Reason = "تداخل زمانی",
            ConflictType = "Schedule"
        }).ToList();
    }

    private Task SetStatisticsAsync(TourDetailDto dto, Tour tour, CancellationToken cancellationToken)
    {
        var confirmedReservations = tour.GetConfirmedReservations();
        
        dto.TotalParticipants = dto.ReservedCapacity;
        dto.MainParticipants = confirmedReservations.SelectMany(r => r.Participants).Count(p => p.ParticipantType == ParticipantType.Member);
        dto.GuestParticipants = confirmedReservations.SelectMany(r => r.Participants).Count(p => p.ParticipantType == ParticipantType.Guest);
        dto.TotalReservations = tour.Reservations.Count;
        dto.ConfirmedReservations = tour.GetConfirmedReservationCount();
        dto.PendingReservations = tour.GetPendingReservationCount();
        dto.AverageRating = 0; // Add rating system if needed
        dto.ReviewCount = 0;   // Add review system if needed
        
        return Task.CompletedTask;
    }

    private void SetUserReservationInfo(TourDetailDto dto, TourReservation userReservation)
    {
        dto.UserReservationStatus = userReservation.Status;
        dto.UserReservationId = userReservation.Id;
        dto.UserReservationTrackingCode = userReservation.TrackingCode;
        dto.UserReservationDate = userReservation.ReservationDate;
        dto.UserReservationExpiryDate = userReservation.ExpiryDate;
    }

    private bool CanUserReserve(Tour tour, TourReservation? userReservation, DateTime currentDate)
    {
        if (!tour.IsActive || !tour.IsRegistrationOpen(currentDate))
            return false;

        if (userReservation != null && userReservation.IsActive())
            return false;

        return tour.MaxParticipants > 0; // Has available capacity
    }

    private List<string> GetReservationBlockReasons(Tour tour, TourReservation? userReservation, DateTime currentDate)
    {
        var reasons = new List<string>();

        if (!tour.IsActive)
            reasons.Add("تور غیرفعال است");

        if (!tour.IsRegistrationOpen(currentDate))
            reasons.Add("زمان ثبت‌نام به پایان رسیده است");

        if (userReservation?.Status == ReservationStatus.Held)
            reasons.Add("شما قبلاً برای این تور رزرو دارید");

        if (userReservation?.Status == ReservationStatus.Confirmed)
            reasons.Add("رزرو شما برای این تور تأیید شده است");

        return reasons;
    }

    // Helper methods
    private static string GetAgeRestrictionNote(int? minAge, int? maxAge)
    {
        if (minAge.HasValue && maxAge.HasValue)
            return $"محدوده سنی: {minAge} تا {maxAge} سال";
        if (minAge.HasValue)
            return $"حداقل سن: {minAge} سال";
        if (maxAge.HasValue)
            return $"حداکثر سن: {maxAge} سال";
        return "محدودیت سنی ندارد";
    }

    private static string GetRegistrationStatus(Tour tour, DateTime currentDate)
    {
        if (!tour.IsActive)
            return "غیرفعال";
        if (!tour.IsRegistrationOpen(currentDate))
            return "بسته";
        return "باز";
    }

    private static void SetCapacityStatusAndMessage(TourDetailDto dto)
    {
        if (dto.IsFullyBooked)
        {
            dto.CapacityStatus = "تکمیل ظرفیت";
            dto.CapacityMessage = "ظرفیت این تور تکمیل شده است";
        }
        else if (dto.IsNearlyFull)
        {
            dto.CapacityStatus = "در حال تکمیل";
            dto.CapacityMessage = $"تنها {dto.RemainingCapacity} جا باقی مانده";
        }
        else
        {
            dto.CapacityStatus = "ظرفیت آزاد";
            dto.CapacityMessage = $"{dto.RemainingCapacity} از {dto.MaxCapacity} جا موجود است";
        }
    }

    private static void SetPricingInfo(TourDetailDto dto)
    {
        if (dto.Pricing.Any())
        {
            dto.LowestPrice = dto.Pricing.Min(p => p.EffectivePrice);
            dto.HighestPrice = dto.Pricing.Max(p => p.EffectivePrice);
            dto.HasDiscount = dto.Pricing.Any(p => p.DiscountPercentage > 0);
            dto.MaxDiscountPercentage = dto.Pricing.Where(p => p.DiscountPercentage > 0).Max(p => p.DiscountPercentage);
        }
    }

    private static string GetParticipantTypeName(ParticipantType type)
    {
        return type switch
        {
            ParticipantType.Member => "شرکت‌کننده اصلی",
            ParticipantType.Guest => "مهمان",
            _ => type.ToString()
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
}
