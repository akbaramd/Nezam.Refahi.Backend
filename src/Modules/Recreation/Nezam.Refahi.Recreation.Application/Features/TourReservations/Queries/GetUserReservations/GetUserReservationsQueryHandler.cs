using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetUserReservations;

public class GetUserReservationsQueryHandler : IRequestHandler<GetUserReservationsQuery, ApplicationResult<PaginatedResult<UserReservationDto>>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemberService _memberService;
    private readonly ILogger<GetUserReservationsQueryHandler> _logger;

    public GetUserReservationsQueryHandler(
        ITourReservationRepository reservationRepository,
        ICurrentUserService currentUserService,
        IMemberService memberService,
        ILogger<GetUserReservationsQueryHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _currentUserService = currentUserService;
        _memberService = memberService;
        _logger = logger;
    }

    public async Task<ApplicationResult<PaginatedResult<UserReservationDto>>> Handle(
        GetUserReservationsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate authentication
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
            {
                return ApplicationResult<PaginatedResult<UserReservationDto>>.Failure(
                    "کاربر احراز هویت نشده است");
            }

            // Get user's national number
            var userNationalNumber = await GetUserNationalNumberAsync(cancellationToken);
            if (string.IsNullOrEmpty(userNationalNumber))
            {
                return ApplicationResult<PaginatedResult<UserReservationDto>>.Failure(
                    "اطلاعات کاربر یافت نشد");
            }

            _logger.LogInformation("Getting reservations for user with national number: {NationalNumber}", 
                userNationalNumber);

            // Get reservations where user is a participant
            // Since we don't have GetQueryable, we'll use a different approach
            var allReservations = await _reservationRepository.FindAsync(x=>true,cancellationToken:cancellationToken);
            var userReservations = allReservations
                .Where(r => r.Participants.Any(p => p.NationalNumber == userNationalNumber))
                .AsQueryable();

            // Apply filters
            if (request.Status.HasValue)
            {
                userReservations = userReservations.Where(r => r.Status == request.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.TrackingCode))
            {
                userReservations = userReservations.Where(r => r.TrackingCode.Contains(request.TrackingCode.Trim()));
            }

            if (request.FromDate.HasValue)
            {
                userReservations = userReservations.Where(r => r.ReservationDate >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                userReservations = userReservations.Where(r => r.ReservationDate <= request.ToDate.Value);
            }

            // Get total count
            var totalCount = userReservations.Count();

            // Apply pagination
            var reservations = userReservations
                .OrderByDescending(r => r.ReservationDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var reservationDtos = reservations.Select(MapToUserReservationDto).ToList();

            var result = new PaginatedResult<UserReservationDto>
            {
                Items = reservationDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            _logger.LogInformation("Successfully retrieved {Count} reservations out of {Total} for user {NationalNumber}",
                reservationDtos.Count, totalCount, userNationalNumber);

            return ApplicationResult<PaginatedResult<UserReservationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting user reservations");
            return ApplicationResult<PaginatedResult<UserReservationDto>>.Failure(ex, "خطا در دریافت رزروهای کاربر");
        }
    }

    private async Task<string?> GetUserNationalNumberAsync(CancellationToken cancellationToken)
    {
        try
        {
            var member = await _memberService.GetMemberByExternalIdAsync(_currentUserService.UserId!.Value.ToString());
            return member?.NationalCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve member information for user {UserId}", _currentUserService.UserId);
            return null;
        }
    }

    private UserReservationDto MapToUserReservationDto(TourReservation reservation)
    {
        // Find the user's participant record
        var userNationalNumber = GetUserNationalNumberAsync(CancellationToken.None).Result;
        var userParticipant = reservation.Participants.FirstOrDefault(p => 
            p.NationalNumber == userNationalNumber);

        var dto = new UserReservationDto
        {
            Id = reservation.Id,
            TourId = reservation.TourId,
            TourTitle = reservation.Tour.Title,
            TourDescription = reservation.Tour.Description ?? string.Empty,
            TrackingCode = reservation.TrackingCode,
            Status = reservation.Status,
            StatusDisplayName = GetStatusDisplayName(reservation.Status),
            ReservationDate = reservation.ReservationDate,
            ExpiryDate = reservation.ExpiryDate,
            ConfirmationDate = reservation.ConfirmationDate,
            CancellationDate = reservation.CancellationDate,
            TotalAmountRials = reservation.TotalAmount?.AmountRials,
            PaidAmountRials = reservation.PaidAmount?.AmountRials,
            Notes = reservation.Notes,
            CancellationReason = reservation.CancellationReason,
            BillId = reservation.BillId,
            CapacityId = reservation.CapacityId,
            MemberId = reservation.MemberId,

            // Tour information
            TourStartDate = reservation.Tour.TourStart,
            TourEndDate = reservation.Tour.TourEnd,
            TourLocation = reservation.Tour.Description, // Using description as location
            TourCapacity = reservation.Tour.MaxParticipants,
            TourAvailableCapacity = reservation.Tour.MaxParticipants,

            // Calculated properties
            IsExpired = reservation.ExpiryDate.HasValue && reservation.ExpiryDate.Value < DateTime.UtcNow,
            CanCancel = CanCancelReservation(reservation),
            CanPay = CanPayReservation(reservation),
            CanConfirm = CanConfirmReservation(reservation),
            IsPaymentPending = IsPaymentPending(reservation),
            IsPaymentOverdue = IsPaymentOverdue(reservation),
            PaymentStatus = GetPaymentStatus(reservation)
        };

        // Add user participant information if found
        if (userParticipant != null)
        {
            dto.UserNationalNumber = userParticipant.NationalNumber;
            dto.UserFirstName = userParticipant.FirstName;
            dto.UserLastName = userParticipant.LastName;
            dto.UserFullName = userParticipant.FullName;
            dto.UserParticipantType = userParticipant.ParticipantType;
            dto.UserRequiredAmountRials = userParticipant.RequiredAmount.AmountRials;
            dto.UserPaidAmountRials = userParticipant.PaidAmount?.AmountRials;
            dto.UserPaymentDate = userParticipant.PaymentDate;
            dto.UserHasPaid = userParticipant.PaidAmount != null && userParticipant.PaidAmount.AmountRials > 0;
            dto.UserIsFullyPaid = userParticipant.PaidAmount != null && 
                                 userParticipant.PaidAmount.AmountRials >= userParticipant.RequiredAmount.AmountRials;
            dto.UserRemainingAmountRials = userParticipant.RequiredAmount.AmountRials - 
                                          (userParticipant.PaidAmount?.AmountRials ?? 0);
        }

        return dto;
    }

    private string GetStatusDisplayName(ReservationStatus status)
    {
        return status switch
        {
            ReservationStatus.Draft => "پیش‌نویس",
            ReservationStatus.Held => "رزرو شده",
            ReservationStatus.Paying => "در حال پرداخت",
            ReservationStatus.Confirmed => "تایید شده",
            ReservationStatus.Cancelled => "لغو شده",
            ReservationStatus.Expired => "منقضی شده",
            _ => status.ToString()
        };
    }

    private bool CanCancelReservation(TourReservation reservation)
    {
        return reservation.Status == ReservationStatus.Held || 
               reservation.Status == ReservationStatus.Paying;
    }

    private bool CanPayReservation(TourReservation reservation)
    {
        return reservation.Status == ReservationStatus.Held && 
               reservation.ExpiryDate.HasValue && 
               reservation.ExpiryDate.Value > DateTime.UtcNow;
    }

    private bool CanConfirmReservation(TourReservation reservation)
    {
        return reservation.Status == ReservationStatus.Paying && 
               reservation.PaidAmount != null && 
               reservation.TotalAmount != null &&
               reservation.PaidAmount.AmountRials >= reservation.TotalAmount.AmountRials;
    }

    private bool IsPaymentPending(TourReservation reservation)
    {
        return reservation.Status == ReservationStatus.Paying;
    }

    private bool IsPaymentOverdue(TourReservation reservation)
    {
        return reservation.Status == ReservationStatus.Held && 
               reservation.ExpiryDate.HasValue && 
               reservation.ExpiryDate.Value < DateTime.UtcNow;
    }

    private string GetPaymentStatus(TourReservation reservation)
    {
        if (reservation.Status == ReservationStatus.Confirmed)
            return "پرداخت شده";
        
        if (reservation.Status == ReservationStatus.Paying)
            return "در حال پرداخت";
        
        if (reservation.Status == ReservationStatus.Held)
        {
            if (reservation.ExpiryDate.HasValue && reservation.ExpiryDate.Value < DateTime.UtcNow)
                return "پرداخت معوق";
            return "در انتظار پرداخت";
        }
        
        if (reservation.Status == ReservationStatus.Cancelled)
            return "لغو شده";
        
        if (reservation.Status == ReservationStatus.Expired)
            return "منقضی شده";
        
        return "نامشخص";
    }
}
