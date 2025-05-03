using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.Services;

/// <summary>
/// Domain service for hotel reservation operations
/// </summary>
public class ReservationDomainService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IReservationRepository _reservationRepository;
    
    public ReservationDomainService(
        IHotelRepository hotelRepository,
        IReservationRepository reservationRepository)
    {
        _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
    }
    
    /// <summary>
    /// Checks if a hotel is available for reservation in the specified date range
    /// </summary>
    public async Task<bool> IsHotelAvailableForReservationAsync(Guid hotelId, DateRange dateRange)
    {
        if (dateRange == null)
            throw new ArgumentNullException(nameof(dateRange));
            
        // Get the hotel
        var hotel = await _hotelRepository.GetByIdAsync(hotelId);
        if (hotel == null || !hotel.IsAvailable)
            return false;
            
        // Check if there are any existing reservations for this hotel in the date range
        // that would prevent a new reservation
        return await _hotelRepository.IsHotelAvailableAsync(hotelId, dateRange);
    }
    
    /// <summary>
    /// Creates a new reservation for a hotel
    /// </summary>
    public async Task<Reservation> CreateReservationAsync(
        Guid hotelId, 
        Guest primaryGuest, 
        DateRange stayPeriod, 
        string? specialRequests = null)
    {
        if (primaryGuest == null)
            throw new ArgumentNullException(nameof(primaryGuest));
            
        if (stayPeriod == null)
            throw new ArgumentNullException(nameof(stayPeriod));
            
        // Check if the hotel exists and is available
        var hotel = await _hotelRepository.GetByIdAsync(hotelId);
        if (hotel == null)
            throw new InvalidOperationException($"Hotel with ID {hotelId} not found");
            
        if (!hotel.IsAvailable)
            throw new InvalidOperationException("Hotel is not available for reservation");
            
        // Check if the hotel is available for the specified date range
        if (!await IsHotelAvailableForReservationAsync(hotelId, stayPeriod))
            throw new InvalidOperationException("Hotel is not available for the specified date range");
            
        // Create the reservation
        var reservation = new Reservation(hotel, primaryGuest, stayPeriod, specialRequests);
        
        // Save the reservation
        await _reservationRepository.AddAsync(reservation);
        
        return reservation;
    }
    
    /// <summary>
    /// Adds a guest to an existing reservation
    /// </summary>
    public async Task AddGuestToReservationAsync(Guid reservationId, Guest guest)
    {
        if (guest == null)
            throw new ArgumentNullException(nameof(guest));
            
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
        reservation.AddGuest(guest);
        
        await _reservationRepository.UpdateAsync(reservation);
    }
    
    /// <summary>
    /// Removes a guest from an existing reservation
    /// </summary>
    public async Task RemoveGuestFromReservationAsync(Guid reservationId, Guid guestId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
        
        // Cannot remove primary guest
        if (reservation.PrimaryGuestId == guestId)
            throw new InvalidOperationException("Cannot remove the primary guest from a reservation");
            
        var guest = reservation.Guests.FirstOrDefault(g => g.Id == guestId);
        if (guest == null)
            throw new InvalidOperationException($"Guest with ID {guestId} not found in reservation");
            
        reservation.RemoveGuest(guest.Id);
        
        await _reservationRepository.UpdateAsync(reservation);
    }
    
    /// <summary>
    /// Updates the special requests for a reservation
    /// </summary>
    public async Task UpdateSpecialRequestsAsync(Guid reservationId, string specialRequests)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
        reservation.UpdateSpecialRequests(specialRequests);
        
        await _reservationRepository.UpdateAsync(reservation);
    }
    
    /// <summary>
    /// Extends the lock on a reservation to prevent it from expiring
    /// </summary>
    public async Task<bool> ExtendReservationLockAsync(Guid reservationId, int minutes)
    {
        if (minutes <= 0)
            throw new ArgumentException("Extension minutes must be greater than zero", nameof(minutes));
            
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
        // Only extend if not already expired
        if (reservation.IsLockExpired())
            return false;
            
        reservation.ExtendLock(minutes);
        await _reservationRepository.UpdateAsync(reservation);
        return true;
    }
    
    /// <summary>
    /// Processes payment for a reservation
    /// </summary>
    public async Task<bool> ProcessPaymentAsync(Guid reservationId, Guid paymentId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
        // If the reservation is free, confirm it directly
        if (reservation.TotalPrice.IsFree)
        {
            reservation.ConfirmReservation();
            await _reservationRepository.UpdateAsync(reservation);
            return true;
        }
        
        // For paid reservations, mark as pending payment first if needed
        if (reservation.Status == ReservationStatus.InProgress)
        {
            reservation.MarkPaymentPending();
        }
        
        // Record the payment and confirm the reservation
        try
        {
            reservation.RecordPayment(paymentId);
            await _reservationRepository.UpdateAsync(reservation);
            return true;
        }
        catch (Exception)
        {
            // Payment processing failed
            return false;
        }
    }
    
    /// <summary>
    /// Processes a partial payment for a reservation
    /// </summary>
    public async Task<bool> ProcessPartialPaymentAsync(Guid reservationId, Guid paymentId, Money paymentAmount)
    {
        if (paymentAmount == null)
            throw new ArgumentNullException(nameof(paymentAmount));
            
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
        // Cannot make partial payment for free reservations
        if (reservation.TotalPrice.IsFree)
            throw new InvalidOperationException("Cannot make partial payment for a free reservation");
            
        try
        {
            // Use the new RecordPartialPayment method in Reservation entity
            reservation.RecordPartialPayment(paymentId, paymentAmount);
            await _reservationRepository.UpdateAsync(reservation);
            return true;
        }
        catch (Exception)
        {
            // Payment processing failed
            return false;
        }
    }
    
    /// <summary>
    /// Cancels a reservation
    /// </summary>
    public async Task CancelReservationAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
        reservation.CancelReservation();
        await _reservationRepository.UpdateAsync(reservation);
    }
    
    /// <summary>
    /// Cancels a reservation with a specific reason
    /// </summary>
    public async Task CancelReservationWithReasonAsync(Guid reservationId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason cannot be empty", nameof(reason));
            
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
        // Use the new CancelWithReason method in Reservation entity
        reservation.CancelWithReason(reason);
        await _reservationRepository.UpdateAsync(reservation);
    }
    
    /// <summary>
    /// Completes a reservation after guest checkout
    /// </summary>
    public async Task CompleteReservationAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
        reservation.CompleteReservation();
        await _reservationRepository.UpdateAsync(reservation);
    }
    
    /// <summary>
    /// Checks for and expires reservations with expired locks
    /// </summary>
    public async Task ExpireReservationsWithExpiredLocksAsync()
    {
        var expiredReservations = await _reservationRepository.GetExpiredReservationsAsync();
        
        foreach (var reservation in expiredReservations)
        {
            if (reservation.IsLockExpired())
            {
                reservation.ExpireReservation();
                await _reservationRepository.UpdateAsync(reservation);
            }
        }
    }
    
    /// <summary>
    /// Modifies the date range of an existing reservation
    /// </summary>
    public async Task<bool> ModifyReservationDatesAsync(Guid reservationId, DateRange newDateRange)
    {
        if (newDateRange == null)
            throw new ArgumentNullException(nameof(newDateRange));
            
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found");
            
        // Check if the reservation is in a valid state for modification
        if (reservation.Status != ReservationStatus.InProgress && reservation.Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Cannot modify dates for a reservation that is not in progress or confirmed");
            
        // Check if the hotel is available for the new date range
        if (!await _hotelRepository.IsHotelAvailableAsync(reservation.HotelId, newDateRange, reservation.Id))
            return false;
            
        // Use the new ModifyStayPeriod method in Reservation entity
        reservation.ModifyStayPeriod(newDateRange);
        await _reservationRepository.UpdateAsync(reservation);
        return true;
    }
    
    /// <summary>
    /// Gets all reservations for a specific guest
    /// </summary>
    public async Task<IEnumerable<Reservation>> GetReservationsByGuestAsync(Guid guestId)
    {
        return await _reservationRepository.GetByGuestIdAsync(guestId);
    }
    
    /// <summary>
    /// Gets all reservations for a specific hotel in a date range
    /// </summary>
    public async Task<IEnumerable<Reservation>> GetReservationsByHotelAndDateRangeAsync(
        Guid hotelId, DateRange dateRange)
    {
        if (dateRange == null)
            throw new ArgumentNullException(nameof(dateRange));
            
        return await _reservationRepository.GetByHotelAndDateRangeAsync(hotelId, dateRange);
    }
    
    /// <summary>
    /// Gets all upcoming reservations for a specific guest
    /// </summary>
    public async Task<IEnumerable<Reservation>> GetUpcomingReservationsByGuestAsync(Guid guestId)
    {
        var allReservations = await _reservationRepository.GetByGuestIdAsync(guestId);
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        return allReservations.Where(r => 
            (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.InProgress) && 
            r.StayPeriod.CheckIn >= today);
    }
    
    /// <summary>
    /// Gets all completed reservations for a specific guest
    /// </summary>
    public async Task<IEnumerable<Reservation>> GetCompletedReservationsByGuestAsync(Guid guestId)
    {
        var allReservations = await _reservationRepository.GetByGuestIdAsync(guestId);
        
        return allReservations.Where(r => r.Status == ReservationStatus.Completed);
    }
    
    /// <summary>
    /// Gets all reservations that are checking in today
    /// </summary>
    public async Task<IEnumerable<Reservation>> GetTodayCheckInsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        return await _reservationRepository.GetByCheckInDateAsync(today);
    }
    
    /// <summary>
    /// Gets all reservations that are checking out today
    /// </summary>
    public async Task<IEnumerable<Reservation>> GetTodayCheckOutsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        return await _reservationRepository.GetByCheckOutDateAsync(today);
    }
}
