using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Recreation.Application.Services;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationDetail;

/// <summary>
/// Handler for getting detailed reservation information
/// </summary>
public class GetReservationDetailQueryHandler
    : IRequestHandler<GetReservationDetailQuery, ApplicationResult<ReservationDetailDto>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourCapacityRepository _capacityRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly MemberValidationService _memberValidationService;
    private readonly ILogger<GetReservationDetailQueryHandler> _logger;

    public GetReservationDetailQueryHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        ITourCapacityRepository capacityRepository,
        ICurrentUserService currentUserService,
        MemberValidationService memberValidationService,
        ILogger<GetReservationDetailQueryHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _tourRepository = tourRepository;
        _capacityRepository = capacityRepository;
        _currentUserService = currentUserService;
        _memberValidationService = memberValidationService;
        _logger = logger;
    }

    public async Task<ApplicationResult<ReservationDetailDto>> Handle(
        GetReservationDetailQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get user national number from current user or request parameter
            string? nationalNumber = await GetUserNationalNumberAsync( cancellationToken);

            _logger.LogInformation("Getting reservation detail - ReservationId: {ReservationId}, NationalNumber: {NationalNumber}",
                request.ReservationId, nationalNumber);

            // Get the reservation with participants
            var reservation = await _reservationRepository.FindOneAsync(x=>x.Id==
                request.ReservationId, cancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation not found - ReservationId: {ReservationId}", request.ReservationId);
                return ApplicationResult<ReservationDetailDto>.Failure("رزرو مورد نظر یافت نشد");
            }

            // Check authorization if national number is available
            if (!string.IsNullOrWhiteSpace(nationalNumber))
            {
                var hasAccess = reservation.Participants.Any(p =>
                    p.NationalNumber == nationalNumber);

                if (!hasAccess)
                {
                    _logger.LogWarning("User does not have access to reservation - ReservationId: {ReservationId}, NationalNumber: {NationalNumber}",
                        request.ReservationId, nationalNumber);
                    return ApplicationResult<ReservationDetailDto>.Failure("شما به این رزرو دسترسی ندارید");
                }
            }
            else
            {
                // If no national number is available and user is not authenticated, deny access
                if (!_currentUserService.IsAuthenticated)
                {
                    _logger.LogWarning("Unauthorized access attempt to reservation - ReservationId: {ReservationId}",
                        request.ReservationId);
                    return ApplicationResult<ReservationDetailDto>.Failure("شما به این رزرو دسترسی ندارید");
                }
            }

            // Get tour information
            var tour = await _tourRepository.GetByIdAsync(reservation.TourId, cancellationToken);
            if (tour == null)
            {
                _logger.LogError("Tour not found for reservation - TourId: {TourId}, ReservationId: {ReservationId}",
                    reservation.TourId, request.ReservationId);
                return ApplicationResult<ReservationDetailDto>.Failure("اطلاعات تور یافت نشد");
            }

            // Get capacity details if reservation has a specific capacity
            TourCapacity? reservationCapacity = null;
            if (reservation.CapacityId.HasValue)
            {
                reservationCapacity = await _capacityRepository.GetWithTourAsync(reservation.CapacityId.Value, cancellationToken);
            }

            // Get all tour capacities for complete information
            var tourCapacities = await _capacityRepository.GetByTourIdAsync(tour.Id, cancellationToken);

            // Map to DTO
            var reservationDto = await MapToDetailDtoAsync(reservation, tour, reservationCapacity, tourCapacities, cancellationToken);

            _logger.LogInformation("Successfully retrieved reservation detail - ReservationId: {ReservationId}",
                request.ReservationId);

            return ApplicationResult<ReservationDetailDto>.Success(reservationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting reservation detail - ReservationId: {ReservationId}",
                request.ReservationId);
            return ApplicationResult<ReservationDetailDto>.Failure(ex, "خطا در دریافت جزئیات رزرو رخ داده است");
        }
    }

    private async Task<ReservationDetailDto> MapToDetailDtoAsync(
        TourReservation reservation, 
        Tour tour, 
        TourCapacity? reservationCapacity,
        IEnumerable<TourCapacity> tourCapacities,
        CancellationToken cancellationToken)
    {
        var currentDate = DateTime.UtcNow;

        // Map capacity details
        TourCapacityDetailDto? capacityDto = null;
        if (reservationCapacity != null)
        {
            capacityDto = await MapToCapacityDetailDtoAsync(reservationCapacity, currentDate, cancellationToken);
        }

        return new ReservationDetailDto
        {
            Id = reservation.Id,
            TourId = reservation.TourId,
            TrackingCode = reservation.TrackingCode,
            Status = reservation.Status,
            ReservationDate = reservation.ReservationDate,
            ExpiryDate = reservation.ExpiryDate,
            ConfirmationDate = reservation.ConfirmationDate,
            TotalAmountRials = reservation.TotalAmount?.AmountRials,
            Notes = reservation.Notes,
            CapacityId = reservation.CapacityId,
            BillId = reservation.BillId,
            Capacity = capacityDto,
            Tour = await MapToTourSummaryDtoAsync(tour, tourCapacities, currentDate, cancellationToken),
            Participants = reservation.Participants.Select(MapToParticipantDto).ToList(),
            ParticipantCount = reservation.GetParticipantCount(),
            MainParticipantCount = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Member),
            GuestParticipantCount = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Guest),
            IsExpired = reservation.IsExpired(),
            IsConfirmed = reservation.IsConfirmed(),
            IsPending = reservation.IsPending(),

            // Additional domain behavior properties
            IsActive = reservation.IsActive(),
            IsCancelled = reservation.IsCancelled(),
            IsTerminal = reservation.IsTerminal(),
            IsDraft = reservation.IsDraft(),
            IsPaying = reservation.IsPaying(),
            IsSystemCancelled = reservation.Status == ReservationStatus.SystemCancelled,
            CreatedAt = reservation.CreatedAt,
            UpdatedAt = reservation.LastModifiedAt,
            CreatedBy = reservation.CreatedBy,
            UpdatedBy = reservation.LastModifiedBy
        };
    }

    private static ParticipantDto MapToParticipantDto(Participant participant)
    {
        return new ParticipantDto
        {
            Id = participant.Id,
            ReservationId = participant.ReservationId,
            FirstName = participant.FirstName,
            LastName = participant.LastName,
            FullName = participant.FullName,
            NationalNumber = participant.NationalNumber,
            PhoneNumber = participant.PhoneNumber,
            Email = participant.Email,
            ParticipantType = participant.ParticipantType,
            BirthDate = participant.BirthDate,
            EmergencyContactName = participant.EmergencyContactName,
            EmergencyContactPhone = participant.EmergencyContactPhone,
            Notes = participant.Notes,
            RequiredAmountRials = participant.RequiredAmount.AmountRials,
            PaidAmountRials = participant.PaidAmount?.AmountRials,
            PaymentDate = participant.PaymentDate,
            RegistrationDate = participant.RegistrationDate,
            HasPaid = participant.HasPaid,
            IsFullyPaid = participant.IsFullyPaid,
            RemainingAmountRials = participant.RemainingAmount.AmountRials,
            IsMainParticipant = participant.IsMainParticipant,
            IsGuest = participant.IsGuest,

            // Domain behavior properties
            IsPaymentPending = !participant.HasPaid && participant.RequiredAmount.AmountRials > 0,
            IsPaymentOverdue = !participant.HasPaid && participant.RequiredAmount.AmountRials > 0,
            CanMakePayment = !participant.HasPaid && participant.RequiredAmount.AmountRials > 0,
            IsPaymentRequired = participant.RequiredAmount.AmountRials > 0
        };
    }

    /// <summary>
    /// Gets the national number for the user - either from request parameter or current authenticated user
    /// </summary>
    /// <param name="requestExternalUserId">External user ID provided in the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The national number to use for authorization, or null if not found</returns>
    private async Task<string?> GetUserNationalNumberAsync( CancellationToken cancellationToken)
    {
       

        // If user is authenticated, get their member information
        if (_currentUserService.IsAuthenticated && _currentUserService.UserId.HasValue)
        {
            try
            {
                var member = await _memberValidationService.GetMemberInfoByExternalIdAsync(_currentUserService.UserId.Value.ToString());
                return member?.NationalCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve member information for user {UserId}", _currentUserService.UserId);
            }
        }

        return null;
    }

    private async Task<TourSummaryDto> MapToTourSummaryDtoAsync(
        Tour tour, 
        IEnumerable<TourCapacity> tourCapacities, 
        DateTime currentDate,
        CancellationToken cancellationToken)
    {
        var capacityList = tourCapacities.ToList();
        var activeCapacities = capacityList.Where(c => c.IsActive).ToList();
        
        // Calculate overall statistics
        var maxCapacity = activeCapacities.Sum(c => c.MaxParticipants);
        var currentReservations = tour.GetConfirmedReservationCount() + tour.GetPendingReservationCount();
        var availableSpots = Math.Max(0, maxCapacity - currentReservations);
        
        // Registration period from active capacities
        var registrationStart = activeCapacities.Any() ? activeCapacities.Min(c => c.RegistrationStart) : (DateTime?)null;
        var registrationEnd = activeCapacities.Any() ? activeCapacities.Max(c => c.RegistrationEnd) : (DateTime?)null;

        // Map capacity details
        var capacityDtos = new List<TourCapacityDetailDto>();
        foreach (var capacity in capacityList)
        {
            var capacityDto = await MapToCapacityDetailDtoAsync(capacity, currentDate, cancellationToken);
            capacityDtos.Add(capacityDto);
        }

        // Map pricing
        var pricingDtos = tour.Pricing.Select(p => MapToPricingDto(p, currentDate)).ToList();

        return new TourSummaryDto
        {
            Id = tour.Id,
            Title = tour.Title,
            Description = tour.Description,
            TourStart = tour.TourStart,
            TourEnd = tour.TourEnd,

            // Capacity information
            MaxCapacity = maxCapacity,
            CurrentReservations = currentReservations,
            AvailableSpots = availableSpots,
            IsAtCapacity = availableSpots == 0,

            // Registration period
            RegistrationStart = registrationStart,
            RegistrationEnd = registrationEnd,
            IsRegistrationOpen = tour.IsRegistrationOpen(currentDate),

            // Age restrictions
            MinAge = tour.MinAge,
            MaxAge = tour.MaxAge,
            MaxGuestsPerReservation = tour.MaxGuestsPerReservation,

            IsActive = tour.IsActive,
            Capacities = capacityDtos,
            Pricing = pricingDtos
        };
    }

    private async Task<TourCapacityDetailDto> MapToCapacityDetailDtoAsync(
        TourCapacity capacity, 
        DateTime currentDate,
        CancellationToken cancellationToken)
    {
        var currentUtilization = await _reservationRepository.GetCapacityUtilizationAsync(capacity.Id, cancellationToken);
        var availableSpots = Math.Max(0, capacity.MaxParticipants - currentUtilization);
        var utilizationPercentage = capacity.MaxParticipants > 0 
            ? (double)currentUtilization / capacity.MaxParticipants * 100 
            : 0;

        var timeUntilStart = capacity.RegistrationStart > currentDate 
            ? capacity.RegistrationStart - currentDate 
            : (TimeSpan?)null;
        
        var timeUntilEnd = capacity.RegistrationEnd > currentDate 
            ? capacity.RegistrationEnd - currentDate 
            : (TimeSpan?)null;

        return new TourCapacityDetailDto
        {
            Id = capacity.Id,
            TourId = capacity.TourId,
            MaxParticipants = capacity.MaxParticipants,
            RegistrationStart = capacity.RegistrationStart,
            RegistrationEnd = capacity.RegistrationEnd,
            IsActive = capacity.IsActive,
            Description = capacity.Description,
            
            // Calculated properties
            IsRegistrationOpen = capacity.IsRegistrationOpen(currentDate),
            IsEffectiveFor = capacity.IsEffectiveFor(currentDate),
            CurrentUtilization = currentUtilization,
            AvailableSpots = availableSpots,
            UtilizationPercentage = utilizationPercentage,
            IsAtCapacity = availableSpots == 0,
            
            // Time-related properties
            TimeUntilRegistrationStart = timeUntilStart,
            TimeUntilRegistrationEnd = timeUntilEnd,
            IsExpired = capacity.RegistrationEnd < currentDate
        };
    }

    private static TourPricingDto MapToPricingDto(TourPricing pricing, DateTime currentDate)
    {
        var isCurrentlyValid = pricing.IsValidFor(currentDate, 1);
        var effectivePrice = pricing.GetEffectivePrice();
        var discountAmount = pricing.Price.AmountRials - effectivePrice.AmountRials;

        return new TourPricingDto
        {
            Id = pricing.Id,
            ParticipantType = pricing.ParticipantType,
            BasePrice = pricing.Price.AmountRials,
            DiscountPercentage = pricing.DiscountPercentage,
            EffectivePrice = effectivePrice.AmountRials,
            ValidFrom = pricing.ValidFrom,
            ValidTo = pricing.ValidTo,
            IsActive = pricing.IsActive
        };
    }
}