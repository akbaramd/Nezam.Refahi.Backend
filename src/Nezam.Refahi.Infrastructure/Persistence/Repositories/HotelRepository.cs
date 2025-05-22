using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementation of the hotel repository interface using EF Core
    /// </summary>
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        public HotelRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Hotel>> FindAvailableHotelsAsync(
            string? city, 
            string? province, 
            DateRange? dateRange, 
            int? minCapacity)
        {
            var query = AsDbSet().AsQueryable();

            // Apply filters based on provided parameters
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(h => h.Location.CityName.Contains(city));
            }

            if (!string.IsNullOrEmpty(province))
            {
                query = query.Where(h => h.Location.ProvinceName.Contains(province));
            }

            if (minCapacity.HasValue)
            {
                query = query.Where(h => h.Capacity >= minCapacity.Value);
            }

            // Get all hotels matching the basic criteria
            var hotels = await query.ToListAsync();

            // If a date range is provided, filter for availability
            if (dateRange != null)
            {
                // Get all reservations for the selected hotels in the given date range
                var hotelIds = hotels.Select(h => h.Id).ToList();
                var reservations = await AsDbContext().Reservations
                    .Where(r => hotelIds.Contains(r.HotelId) && 
                               (r.Status == ReservationStatus.Confirmed ||
                                r.Status == ReservationStatus.PendingPayment) &&
                               ((r.StayPeriod.CheckIn <= dateRange.CheckOut && r.StayPeriod.CheckOut >= dateRange.CheckIn)))
                    .ToListAsync();

                // Group reservations by hotel to check capacity constraints
                var reservationsByHotel = reservations
                    .GroupBy(r => r.HotelId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Filter hotels based on availability during the specified date range
                hotels = hotels.Where(hotel => 
                    !reservationsByHotel.ContainsKey(hotel.Id) || 
                    reservationsByHotel[hotel.Id].Count < hotel.Capacity).ToList();
            }

            return hotels;
        }

        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await AsDbSet().ToListAsync();

            return await AsDbSet()
                .Where(h => h.Name.Contains(searchTerm) || 
                            h.Description.Contains(searchTerm) ||
                            h.Location.Address.Contains(searchTerm) ||
                            h.Features.Any(f => f.Name.Contains(searchTerm) || f.Value.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<bool> IsHotelAvailableAsync(Guid hotelId, DateRange dateRange)
        {
            return await IsHotelAvailableAsync(hotelId, dateRange, Guid.Empty);
        }

        public async Task<bool> IsHotelAvailableAsync(Guid hotelId, DateRange dateRange, Guid excludeReservationId)
        {
            var hotel = await AsDbSet().FindAsync(hotelId);
            if (hotel == null)
                return false;

            // Count all confirmed and pending reservations for this hotel in the given date range
            // excluding the specified reservation (if any)
            var reservationsCount = await AsDbContext().Reservations
                .Where(r => r.HotelId == hotelId &&
                           (excludeReservationId == Guid.Empty || r.Id != excludeReservationId) &&
                           (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.PendingPayment) &&
                           (r.StayPeriod.CheckIn <= dateRange.CheckOut && r.StayPeriod.CheckOut >= dateRange.CheckIn))
                .CountAsync();

            // Hotel is available if there are fewer reservations than the hotel's capacity
            return reservationsCount < hotel.Capacity;
        }
    }
}
