using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// پیاده‌سازی Repository برای موجودیت‌های TourReservation
/// </summary>
public class TourReservationRepository : EfRepository<RecreationDbContext, TourReservation, Guid>, ITourReservationRepository
{
    public TourReservationRepository(RecreationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TourReservation>> GetByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.TourId == tourId)
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TourReservation>> GetByTourIdAndStatusAsync(Guid tourId, ReservationStatus status, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(r => r.TourId == tourId && 
                       r.Status == status &&
                       (r.ExpiryDate == null || r.ExpiryDate > currentTime)) // حذف رزروهای منقضی شده
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TourReservation>> GetConfirmedReservationsByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        return await GetByTourIdAndStatusAsync(tourId, ReservationStatus.Confirmed, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<TourReservation>> GetPendingReservationsByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        return await GetByTourIdAndStatusAsync(tourId, ReservationStatus.Held, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<TourReservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(r => r.Status == ReservationStatus.Held &&
                       r.ExpiryDate.HasValue &&
                       r.ExpiryDate.Value < currentTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<TourReservation?> GetByTrackingCodeAsync(string trackingCode, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(r => r.TrackingCode.ToLower() == trackingCode.ToLower(), cancellationToken:cancellationToken);
    }

    public async Task<int> GetConfirmedParticipantCountAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(r => r.TourId == tourId && 
                       r.Status == ReservationStatus.Confirmed &&
                       (r.ExpiryDate == null || r.ExpiryDate > currentTime)) // حذف رزروهای منقضی شده
            .SelectMany(r => r.Participants)
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetPendingParticipantCountAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(r => r.TourId == tourId && 
                       (r.Status == ReservationStatus.Held || r.Status == ReservationStatus.Paying) && // شامل رزروهای در حال پرداخت
                       (r.ExpiryDate == null || r.ExpiryDate > currentTime)) // حذف رزروهای منقضی شده
            .SelectMany(r => r.Participants)
            .CountAsync(cancellationToken);
    }

    public async Task<bool> TrackingCodeExistsAsync(string trackingCode, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(r => r.TrackingCode == trackingCode.ToUpperInvariant(), cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<TourReservation>> GetByTourIdAndNationalNumberAsync(Guid tourId, string nationalNumber, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.TourId == tourId &&
                       r.Participants.Any(p => p.NationalNumber == nationalNumber))
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TourReservation>> GetByTourIdsAndNationalNumberAsync(IEnumerable<Guid> tourIds, string nationalNumber, CancellationToken cancellationToken = default)
    {
        var tourIdsList = tourIds.ToList();
        return await _dbContext.Set<TourReservation>()
            .Include(r => r.Participants)
            .Include(r => r.Tour)
            .Where(r => tourIdsList.Contains(r.TourId) &&
                       r.Participants.Any(p => p.NationalNumber == nationalNumber))
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<TourReservation?> GetByIdWithParticipantsAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TourReservation>()
            .Include(r => r.Participants)
            .Include(r => r.Tour)
            .FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);
    }

    public async Task<int> GetCapacityUtilizationAsync(Guid capacityId, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(r => r.CapacityId == capacityId &&
                       (r.Status == ReservationStatus.Confirmed || 
                        r.Status == ReservationStatus.Held || 
                        r.Status == ReservationStatus.Paying) && // اضافه کردن رزروهای در حال پرداخت
                       (r.ExpiryDate == null || r.ExpiryDate > currentTime)) // چک انقضا
            .SelectMany(r => r.Participants)
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetTourUtilizationAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(r => r.TourId == tourId &&
                       (r.Status == ReservationStatus.Confirmed || 
                        r.Status == ReservationStatus.Held || 
                        r.Status == ReservationStatus.Paying) && // اضافه کردن رزروهای در حال پرداخت
                       (r.ExpiryDate == null || r.ExpiryDate > currentTime)) // چک انقضا
            .SelectMany(r => r.Participants)
            .CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<TourReservation>> GetExpiredReservationsAsync(DateTime cutoffTime, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.ExpiryDate.HasValue && 
                       r.ExpiryDate.Value <= cutoffTime &&
                       (r.Status == ReservationStatus.Held || r.Status == ReservationStatus.Paying))
            .Include(r => r.Participants)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TourReservation>> GetExpiringReservationsAsync(DateTime expiryTime, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.ExpiryDate.HasValue && 
                       r.ExpiryDate.Value <= expiryTime &&
                       r.ExpiryDate.Value > DateTime.UtcNow &&
                       (r.Status == ReservationStatus.Held || r.Status == ReservationStatus.Paying))
            .Include(r => r.Participants)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetTourUtilizationBatchAsync(IEnumerable<Guid> tourIds, CancellationToken cancellationToken = default)
    {
        var tourIdsList = tourIds.ToList();
        if (!tourIdsList.Any())
            return new Dictionary<Guid, int>();

        var currentTime = DateTime.UtcNow;
        var utilizations = await PrepareQuery(_dbSet)
            .Where(r => tourIdsList.Contains(r.TourId) &&
                       (r.Status == ReservationStatus.Confirmed || 
                        r.Status == ReservationStatus.Held || 
                        r.Status == ReservationStatus.Paying) && // اضافه کردن رزروهای در حال پرداخت
                       (r.ExpiryDate == null || r.ExpiryDate > currentTime)) // حذف رزروهای منقضی شده
            .SelectMany(r => r.Participants)
            .GroupBy(p => p.Reservation.TourId)
            .Select(g => new { TourId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var result = tourIdsList.ToDictionary(id => id, _ => 0);
        foreach (var utilization in utilizations)
        {
            result[utilization.TourId] = utilization.Count;
        }

        return result;
    }

    public async Task<Dictionary<Guid, int>> GetCapacityUtilizationBatchAsync(IEnumerable<Guid> capacityIds, CancellationToken cancellationToken = default)
    {
        var capacityIdsList = capacityIds.ToList();
        if (!capacityIdsList.Any())
            return new Dictionary<Guid, int>();

        var currentTime = DateTime.UtcNow;
        var utilizations = await PrepareQuery(_dbSet)
            .Where(r => r.CapacityId.HasValue && 
                       capacityIdsList.Contains(r.CapacityId.Value) &&
                       (r.Status == ReservationStatus.Confirmed || 
                        r.Status == ReservationStatus.Held || 
                        r.Status == ReservationStatus.Paying) && // اضافه کردن رزروهای در حال پرداخت
                       (r.ExpiryDate == null || r.ExpiryDate > currentTime)) // حذف رزروهای منقضی شده
            .SelectMany(r => r.Participants)
            .GroupBy(p => p.Reservation.CapacityId)
            .Select(g => new { CapacityId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var result = capacityIdsList.ToDictionary(id => id, _ => 0);
        foreach (var utilization in utilizations)
        {
            if (utilization.CapacityId.HasValue)
            {
                result[utilization.CapacityId.Value] = utilization.Count;
            }
        }

        return result;
    }


    protected override IQueryable<TourReservation> PrepareQuery(IQueryable<TourReservation> query)
    {
      query = query.Include(x => x.Participants)
                   .Include(x => x.Capacity)
                   .Include(x => x.Tour);
      return base.PrepareQuery(query);
    }
}