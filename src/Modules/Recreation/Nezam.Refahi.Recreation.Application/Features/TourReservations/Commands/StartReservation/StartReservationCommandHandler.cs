using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.StartReservation;

public sealed class StartReservationCommandHandler
    : IRequestHandler<StartReservationCommand, ApplicationResult<StartReservationCommandResult>>
{
    private const int DEFAULT_ALLOCATION = 1;

    private readonly ITourRepository _tourRepository;
    private readonly ITourReservationRepository _reservationRepository;
    private readonly MemberValidationService _memberValidationService;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ILogger<StartReservationCommandHandler> _logger;

    public StartReservationCommandHandler(
        ITourRepository tourRepository,
        ITourReservationRepository reservationRepository,
        MemberValidationService memberValidationService,
        IRecreationUnitOfWork unitOfWork,
        ILogger<StartReservationCommandHandler> logger)
    {
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _memberValidationService = memberValidationService ?? throw new ArgumentNullException(nameof(memberValidationService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<StartReservationCommandResult>> Handle(StartReservationCommand request, CancellationToken ct)
    {
        if (request.TourId == Guid.Empty)
            return ApplicationResult<StartReservationCommandResult>.Failure("شناسه تور معتبر نیست");
        if (request.CapacityId == Guid.Empty)
            return ApplicationResult<StartReservationCommandResult>.Failure("ظرفیت انتخاب‌شده الزامی است");
        if (string.IsNullOrWhiteSpace(request.UserNationalNumber))
            return ApplicationResult<StartReservationCommandResult>.Failure("کد ملی کاربر الزامی است");

        // Load Tour with Capacities tracked
        var tour = await _tourRepository.FindOneAsync(t => t.Id == request.TourId, ct);
        if (tour is null)
            return ApplicationResult<StartReservationCommandResult>.Failure("تور یافت نشد");
        if (!tour.IsActive)
            return ApplicationResult<StartReservationCommandResult>.Failure("تور غیرفعال است");
        if (!tour.IsRegistrationOpen(DateTime.UtcNow))
            return ApplicationResult<StartReservationCommandResult>.Failure("ثبت‌نام برای این تور باز نیست");

        // Member
        var nationalId = new NationalId(request.UserNationalNumber);
        MemberInfoDto? member = await _memberValidationService.GetMemberInfoAsync(nationalId);
        if (member is null)
            return ApplicationResult<StartReservationCommandResult>.Failure("عضو یافت نشد");
        if (!member.HasActiveMembership)
            return ApplicationResult<StartReservationCommandResult>.Failure("عضویت فعال برای این عضو موجود نیست");

        // Restriction + access
        var restrictedCheck = await ValidateRestrictedToursAsync(tour, nationalId, ct);
        if (!restrictedCheck.IsSuccess)
            return ApplicationResult<StartReservationCommandResult>.Failure(restrictedCheck.Errors);

        var accessCheck = await ValidateMemberAccessAnyOfAsync(tour, member, ct);
        if (!accessCheck.IsSuccess)
            return ApplicationResult<StartReservationCommandResult>.Failure(accessCheck.Errors);

        // Capacity validation (must belong to tour)
        var capacity = tour.Capacities?.FirstOrDefault(c => c.Id == request.CapacityId);
        if (capacity is null)
            return ApplicationResult<StartReservationCommandResult>.Failure("ظرفیت انتخاب‌شده متعلق به این تور نیست");

        var isVip = IsSpecialMember(member);
        if (!capacity.IsVisibleToMember(isVip))
            return ApplicationResult<StartReservationCommandResult>.Failure("دسترسی به این ظرفیت برای شما مجاز نیست");
        if (!capacity.CanAccommodateForMember(DEFAULT_ALLOCATION, isVip))
            return ApplicationResult<StartReservationCommandResult>.Failure("ظرفیت انتخاب‌شده شرایط یا ظرفیت کافی را ندارد");

        // Prevent multiple drafts/held for same tour/member
        var existing = await _reservationRepository
            .GetByTourIdAndNationalNumberAsync(request.TourId, nationalId.Value, ct);

        if (existing.Any(r => r.Status == ReservationStatus.Draft ||
                              (r.Status == ReservationStatus.OnHold && !r.IsExpired())))
        {
            return ApplicationResult<StartReservationCommandResult>.Failure("پیش‌نویس/رزرو فعال قبلی برای این تور موجود است");
        }

        try
        {
            await _unitOfWork.BeginAsync(ct);

            // Atomic allocate 1 seat
            if (!capacity.TryAllocateParticipants(DEFAULT_ALLOCATION))
                return ApplicationResult<StartReservationCommandResult>.Failure("ظرفیت انتخاب‌شده هم‌اکنون قابل تخصیص نیست");

            var trackingCode = await GenerateUniqueTrackingCodeAsync(ct);

            var reservation = new TourReservation(
                tourId: tour.Id,
                trackingCode: trackingCode,
                externalUserId: member.Id,
                capacityId: capacity.Id,
                memberId: member.Id,
                expiryDate: null,
                notes: null);

            await _reservationRepository.AddAsync(reservation, cancellationToken:ct);
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);

            return ApplicationResult<StartReservationCommandResult>.Success(new StartReservationCommandResult
            {
                ReservationId = reservation.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed StartReservation TourId={TourId} Nat={Nat}", request.TourId, request.UserNationalNumber);
            try { await _unitOfWork.RollbackAsync(ct); } catch { /* ignore */ }
            return ApplicationResult<StartReservationCommandResult>.Failure("خطا در ایجاد رزرو");
        }
    }

    // Helpers

    private async Task<string> GenerateUniqueTrackingCodeAsync(CancellationToken ct)
    {
        for (int i = 0; i < 7; i++)
        {
            var code = $"RSV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
            var exists = await _reservationRepository.TrackingCodeExistsAsync(code, ct);
            if (!exists) return code;
            await Task.Delay(5, ct);
        }
        throw new ApplicationException("امکان تولید کد پیگیری یکتا فراهم نشد");
    }

    private async Task<ApplicationResult> ValidateRestrictedToursAsync(Tour tour, NationalId nationalId, CancellationToken ct)
    {
        if (!tour.TourRestrictedTours.Any())
            return ApplicationResult.Success();

        var restrictedIds = tour.TourRestrictedTours.Select(r => r.RestrictedTourId).ToList();
        var reservations = await _reservationRepository.GetByTourIdsAndNationalNumberAsync(restrictedIds, nationalId.Value, ct);
        var active = reservations.Where(r => r.IsActive()).ToList();
        if (active.Any())
        {
            var titles = string.Join("، ", active.Select(r => r.Tour.Title));
            return ApplicationResult.Failure($"به‌دلیل رزرو فعال در تورهای محدودکننده، امکان رزرو این تور وجود ندارد: {titles}");
        }
        return ApplicationResult.Success();
    }

    private async Task<ApplicationResult> ValidateMemberAccessAnyOfAsync(Tour tour, MemberInfoDto member, CancellationToken ct)
    {
        var reqCaps  = tour.MemberCapabilities.Select(x => x.CapabilityId).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var reqFeats = tour.MemberFeatures   .Select(x => x.FeatureId)   .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        if (!reqCaps.Any() && !reqFeats.Any())
            return ApplicationResult.Success();

        var errors = new List<string>();

        if (reqCaps.Any())
        {
            var hasAny = (member.Capabilities ?? new List<string>()).Intersect(reqCaps, StringComparer.OrdinalIgnoreCase).Any();
            if (!hasAny)
            {
                foreach (var cap in reqCaps)
                    if (await _memberValidationService.HasCapabilityAsync(member.NationalCode, cap)) { hasAny = true; break; }
            }
            if (!hasAny) errors.Add($"عدم احراز حداقل یکی از صلاحیت‌های موردنیاز: {string.Join(", ", reqCaps)}");
        }

        if (reqFeats.Any())
        {
            var hasAny = (member.Features ?? new List<string>()).Intersect(reqFeats, StringComparer.OrdinalIgnoreCase).Any();
            if (!hasAny)
            {
                foreach (var f in reqFeats)
                    if (await _memberValidationService.HasFeatureAsync(member.NationalCode, f)) { hasAny = true; break; }
            }
            if (!hasAny) errors.Add($"عدم احراز حداقل یکی از ویژگی‌های موردنیاز: {string.Join(", ", reqFeats)}");
        }

        return errors.Count > 0 ? ApplicationResult.Failure(errors) : ApplicationResult.Success();
    }

    private static bool IsSpecialMember(MemberInfoDto member)
    {
        var caps = member.Capabilities ?? Enumerable.Empty<string>();
        var feats = member.Features ?? Enumerable.Empty<string>();
        return caps.Contains("VIP", StringComparer.OrdinalIgnoreCase)
            || feats.Contains("VIP", StringComparer.OrdinalIgnoreCase);
    }
}
