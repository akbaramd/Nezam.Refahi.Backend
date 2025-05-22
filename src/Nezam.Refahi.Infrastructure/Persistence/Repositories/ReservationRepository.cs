using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementation of the reservation repository using EF Core
    /// </summary>
    public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(r => r.PrimaryGuestId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByGuestIdAsync(Guid guestId)
        {
            return await _dbSet
                .Where(r => r.Guests.Any(g => g.Id == guestId))
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByHotelIdAsync(Guid hotelId)
        {
            return await _dbSet
                .Where(r => r.HotelId == hotelId)
                .OrderByDescending(r => r.StayPeriod.CheckIn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByHotelAndDateRangeAsync(Guid hotelId, DateRange dateRange)
        {
            return await _dbSet
                .Where(r => r.HotelId == hotelId &&
                           r.StayPeriod.CheckIn <= dateRange.CheckOut &&
                           r.StayPeriod.CheckOut >= dateRange.CheckIn)
                .OrderBy(r => r.StayPeriod.CheckIn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByStatusAsync(ReservationStatus status)
        {
            return await _dbSet
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.ModifiedAt ?? r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync()
        {
            var now = DateTimeOffset.UtcNow;
            
            return await _dbSet
                .Where(r => (r.Status == ReservationStatus.InProgress || 
                            r.Status == ReservationStatus.PendingPayment) &&
                            r.LockExpirationTime < now)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByCheckInDateAsync(DateOnly checkInDate)
        {
            return await _dbSet
                .Where(r => r.StayPeriod.CheckIn == checkInDate && 
                           r.Status == ReservationStatus.Confirmed)
                .OrderBy(r => r.HotelId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByCheckOutDateAsync(DateOnly checkOutDate)
        {
            return await _dbSet
                .Where(r => r.StayPeriod.CheckOut == checkOutDate && 
                           r.Status == ReservationStatus.Confirmed)
                .OrderBy(r => r.HotelId)
                .ToListAsync();
        }
    }
}
