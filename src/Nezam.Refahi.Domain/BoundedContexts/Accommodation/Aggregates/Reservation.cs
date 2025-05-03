using System;
using System.Collections.Generic;
using System.Linq;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;

/// <summary>
/// Represents a hotel reservation as an aggregate root
/// </summary>
public class Reservation : BaseEntity
{
    private readonly List<Guest> _guests = new();
    
    public Guid HotelId { get; private set; }
    public Guid PrimaryGuestId { get; private set; }
    public DateRange StayPeriod { get; private set; } = null!;
    public ReservationStatus Status { get; private set; }
    public Money TotalPrice { get; private set; } = null!;
    public string? SpecialRequests { get; private set; }
    
    // Navigation property for EF Core (will be loaded from repository)
    public Hotel? Hotel { get; private set; }
    
    // Navigation property for primary guest (the person making the reservation)
    public Guest? PrimaryGuest { get; private set; }
    
    public IReadOnlyCollection<Guest> Guests => _guests.AsReadOnly();
    
    // Payment tracking
    public Guid? PaymentId { get; private set; }
    public DateTimeOffset? PaymentDate { get; private set; }
    
    // Reservation locking
    private DateTimeOffset _lockExpirationTime;
    public DateTimeOffset LockExpirationTime 
    { 
        get => _lockExpirationTime; 
        private set => _lockExpirationTime = value; 
    }
    
    // Private constructor for EF Core
    private Reservation() : base() { }
    
    public Reservation(Hotel hotel, Guest primaryGuest, DateRange stayPeriod, string? specialRequests = null) 
        : base()
    {
        if (hotel == null)
            throw new ArgumentNullException(nameof(hotel));
            
        if (primaryGuest == null)
            throw new ArgumentNullException(nameof(primaryGuest));
            
        if (stayPeriod == null)
            throw new ArgumentNullException(nameof(stayPeriod));
            
        // Check if hotel is available
        if (!hotel.IsAvailable)
            throw new InvalidOperationException("Hotel is not available for reservation");
            
        HotelId = hotel.Id;
        Hotel = hotel;
        PrimaryGuestId = primaryGuest.Id;
        PrimaryGuest = primaryGuest;
        StayPeriod = stayPeriod;
        SpecialRequests = specialRequests;
        
        // Add primary guest to the guest list
        _guests.Add(primaryGuest);
        
        // Calculate total price
        TotalPrice = hotel.PricePerNight * stayPeriod.NightCount;
        
        // Set initial status
        Status = hotel.IsFree ? ReservationStatus.Confirmed : ReservationStatus.InProgress;
        
        // Set lock expiration time (30 minutes from now)
        LockExpirationTime = DateTimeOffset.UtcNow.AddMinutes(30);
    }
    
    public void AddGuest(Guest guest)
    {
        if (guest == null)
            throw new ArgumentNullException(nameof(guest));
            
        if (Status != ReservationStatus.InProgress)
            throw new InvalidOperationException("Cannot add guests to a reservation that is not in progress");
            
        if (_guests.Any(g => g.NationalId.ToString() == guest.NationalId.ToString()))
            throw new InvalidOperationException("A guest with the same National ID already exists in this reservation");
            
        _guests.Add(guest);
        UpdateModifiedAt();
    }
    
    public void RemoveGuest(Guid guestId)
    {
        if (Status != ReservationStatus.InProgress)
            throw new InvalidOperationException("Cannot remove guests from a reservation that is not in progress");
            
        if (PrimaryGuestId == guestId)
            throw new InvalidOperationException("Cannot remove the primary guest from the reservation");
            
        var guest = _guests.FirstOrDefault(g => g.Id == guestId);
        if (guest != null)
        {
            _guests.Remove(guest);
            UpdateModifiedAt();
        }
    }
    
    public void UpdateSpecialRequests(string? specialRequests)
    {
        if (Status != ReservationStatus.InProgress && Status != ReservationStatus.PendingPayment)
            throw new InvalidOperationException("Cannot update special requests for a reservation that is not in progress or pending payment");
            
        SpecialRequests = specialRequests;
        UpdateModifiedAt();
    }
    
    public void ConfirmReservation()
    {
        // Free reservations can be confirmed from any state
        if (TotalPrice.IsFree)
        {
            Status = ReservationStatus.Confirmed;
            UpdateModifiedAt();
            return;
        }
        
        if (Status != ReservationStatus.InProgress && Status != ReservationStatus.PendingPayment)
            throw new InvalidOperationException("Cannot confirm a reservation that is not in progress or pending payment");
            
        if (!TotalPrice.IsFree && Status != ReservationStatus.PendingPayment)
            throw new InvalidOperationException("Paid reservations must be in PendingPayment status before confirmation");
            
        Status = ReservationStatus.Confirmed;
        UpdateModifiedAt();
    }
    
    public void CancelReservation()
    {
        if (Status == ReservationStatus.Completed || Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel a reservation that is already completed or cancelled");
            
        Status = ReservationStatus.Cancelled;
        UpdateModifiedAt();
    }
    
    public void CompleteReservation()
    {
        if (Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Cannot complete a reservation that is not confirmed");
            
        Status = ReservationStatus.Completed;
        UpdateModifiedAt();
    }

    public void MarkPaymentPending()
    {
        if (Status != ReservationStatus.InProgress)
            throw new InvalidOperationException("Cannot mark payment pending for a reservation that is not in progress");
            
        if (TotalPrice.IsFree)
            throw new InvalidOperationException("Cannot mark payment pending for a free reservation");
            
        Status = ReservationStatus.PendingPayment;
        // Extend lock expiration time for payment (15 minutes)
        LockExpirationTime = LockExpirationTime.AddMinutes(15);
        UpdateModifiedAt();
    }
    
    public void RecordPayment(Guid paymentId)
    {
        if (Status != ReservationStatus.PendingPayment)
            throw new InvalidOperationException("Cannot record payment for a reservation that is not pending payment");
            
        if (TotalPrice.IsFree)
            throw new InvalidOperationException("Cannot record payment for a free reservation");
            
        PaymentId = paymentId;
        PaymentDate = DateTimeOffset.UtcNow;
        ConfirmReservation();
    }
    
    public void ExtendLock(int minutes)
    {
        if (Status != ReservationStatus.InProgress && Status != ReservationStatus.PendingPayment)
            throw new InvalidOperationException("Cannot extend lock for a reservation that is not in progress or pending payment");
            
        if (minutes <= 0)
            throw new ArgumentException("Lock extension minutes must be positive", nameof(minutes));
            
        // Add minutes to the current lock expiration time, not reset it to now + minutes
        LockExpirationTime = LockExpirationTime.AddMinutes(minutes);
        UpdateModifiedAt();
    }
    
    public bool IsLockExpired() => DateTimeOffset.UtcNow > LockExpirationTime;
    
    public void ExpireReservation()
    {
        if (Status != ReservationStatus.InProgress && Status != ReservationStatus.PendingPayment)
            throw new InvalidOperationException("Cannot expire a reservation that is not in progress or pending payment");
            
        if (!IsLockExpired())
            throw new InvalidOperationException("Cannot expire a reservation that is not locked");
            
        Status = ReservationStatus.Expired;
        UpdateModifiedAt();
    }
    
    /// <summary>
    /// Updates the stay period of the reservation
    /// </summary>
    /// <param name="newStayPeriod">The new stay period</param>
    /// <exception cref="InvalidOperationException">Thrown when the reservation is not in a valid state for modification</exception>
    /// <exception cref="ArgumentNullException">Thrown when the new stay period is null</exception>
    public void ModifyStayPeriod(DateRange newStayPeriod)
    {
        if (newStayPeriod == null)
            throw new ArgumentNullException(nameof(newStayPeriod));
            
        if (Status != ReservationStatus.InProgress && Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Cannot modify stay period for a reservation that is not in progress or confirmed");
            
        // Update the stay period
        StayPeriod = newStayPeriod;
        
        // Recalculate the total price if we have a hotel reference
        if (Hotel != null)
        {
            TotalPrice = Hotel.PricePerNight * newStayPeriod.NightCount;
        }
        
        UpdateModifiedAt();
    }
    
    /// <summary>
    /// Records a partial payment for the reservation
    /// </summary>
    /// <param name="paymentId">The payment ID</param>
    /// <param name="amount">The payment amount</param>
    /// <exception cref="InvalidOperationException">Thrown when the reservation is not in a valid state for payment</exception>
    /// <exception cref="ArgumentException">Thrown when the payment amount is invalid</exception>
    public void RecordPartialPayment(Guid paymentId, Money amount)
    {
        if (Status != ReservationStatus.InProgress && Status != ReservationStatus.PendingPayment)
            throw new InvalidOperationException("Cannot record payment for a reservation that is not in progress or pending payment");
            
        if (TotalPrice.IsFree)
            throw new InvalidOperationException("Cannot record payment for a free reservation");
            
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));
            
        if (amount.Amount <= 0)
            throw new ArgumentException("Payment amount must be positive", nameof(amount));
            
        if (amount.Currency != TotalPrice.Currency)
            throw new ArgumentException("Payment currency must match reservation currency", nameof(amount));
            
        // For partial payments, we would normally track them in a collection
        // and update the remaining balance. For simplicity, we'll just record the payment
        // and set the status to PendingPayment if not already.
        
        PaymentId = paymentId;
        PaymentDate = DateTimeOffset.UtcNow;
        
        // If the amount equals or exceeds the total price, confirm the reservation
        if (amount.Amount >= TotalPrice.Amount)
        {
            ConfirmReservation();
        }
        else if (Status == ReservationStatus.InProgress)
        {
            // If we're still in progress, mark as pending payment
            Status = ReservationStatus.PendingPayment;
            UpdateModifiedAt();
        }
    }
    
    /// <summary>
    /// Records a cancellation reason for the reservation
    /// </summary>
    /// <param name="reason">The cancellation reason</param>
    /// <exception cref="InvalidOperationException">Thrown when the reservation is already cancelled or completed</exception>
    /// <exception cref="ArgumentException">Thrown when the reason is null or empty</exception>
    public void CancelWithReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason cannot be empty", nameof(reason));
            
        if (Status == ReservationStatus.Completed || Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel a reservation that is already completed or cancelled");
            
        // In a real implementation, we would store the cancellation reason
        // For now, we'll just cancel the reservation
        Status = ReservationStatus.Cancelled;
        UpdateModifiedAt();
    }
    
    public int GuestCount => _guests.Count;
}
