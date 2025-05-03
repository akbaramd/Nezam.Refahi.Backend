using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.Repositories;

/// <summary>
/// Repository interface for Reservation aggregate
/// </summary>
public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Gets all reservations for a specific user
    /// </summary>
    Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Gets all reservations for a specific guest
    /// </summary>
    Task<IEnumerable<Reservation>> GetByGuestIdAsync(Guid guestId);
    
    /// <summary>
    /// Gets all reservations for a specific hotel
    /// </summary>
    Task<IEnumerable<Reservation>> GetByHotelIdAsync(Guid hotelId);
    
    /// <summary>
    /// Gets all reservations for a specific hotel during a date range
    /// </summary>
    Task<IEnumerable<Reservation>> GetByHotelAndDateRangeAsync(Guid hotelId, DateRange dateRange);
    
    /// <summary>
    /// Finds all reservations with a specific status
    /// </summary>
    Task<IEnumerable<Reservation>> GetByStatusAsync(ReservationStatus status);
    
    /// <summary>
    /// Finds all expired reservations (lock expired but status still InProgress or PendingPayment)
    /// </summary>
    Task<IEnumerable<Reservation>> GetExpiredReservationsAsync();
    
    /// <summary>
    /// Gets all reservations checking in on a specific date
    /// </summary>
    Task<IEnumerable<Reservation>> GetByCheckInDateAsync(DateOnly checkInDate);
    
    /// <summary>
    /// Gets all reservations checking out on a specific date
    /// </summary>
    Task<IEnumerable<Reservation>> GetByCheckOutDateAsync(DateOnly checkOutDate);
    
    Task AddAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
    Task DeleteAsync(Guid id);
}
