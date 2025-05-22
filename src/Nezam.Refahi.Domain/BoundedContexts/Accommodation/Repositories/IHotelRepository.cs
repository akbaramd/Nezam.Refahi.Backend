using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.Repositories;

/// <summary>
/// Repository interface for Hotel entity
/// </summary>
public interface IHotelRepository : IGenericRepository<Hotel>
{
    /// <summary>
    /// Finds hotels based on location, available dates, and capacity
    /// </summary>
    Task<IEnumerable<Hotel>> FindAvailableHotelsAsync(
        string? city, 
        string? province, 
        DateRange? dateRange, 
        int? minCapacity);
    
    /// <summary>
    /// Searches for hotels by name, description, or features
    /// </summary>
    Task<IEnumerable<Hotel>> SearchHotelsAsync(string searchTerm);
    
    /// <summary>
    /// Checks if a hotel is available for the given date range
    /// </summary>
    Task<bool> IsHotelAvailableAsync(Guid hotelId, DateRange dateRange);
    
    /// <summary>
    /// Checks if a hotel is available for the given date range, excluding a specific reservation
    /// </summary>
    /// <param name="hotelId">The ID of the hotel to check</param>
    /// <param name="dateRange">The date range to check availability for</param>
    /// <param name="excludeReservationId">The ID of a reservation to exclude from availability check (useful when modifying an existing reservation)</param>
    /// <returns>True if the hotel is available for the specified date range, false otherwise</returns>
    Task<bool> IsHotelAvailableAsync(Guid hotelId, DateRange dateRange, Guid excludeReservationId);
}
