namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;

/// <summary>
/// Represents the status of a hotel reservation
/// </summary>
public enum ReservationStatus
{
    /// <summary>
    /// Reservation is currently in progress (locked for this user)
    /// </summary>
    InProgress,
    
    /// <summary>
    /// Reservation is pending payment
    /// </summary>
    PendingPayment,
    
    /// <summary>
    /// Reservation is confirmed (payment completed or free reservation)
    /// </summary>
    Confirmed,
    
    /// <summary>
    /// Reservation has been cancelled
    /// </summary>
    Cancelled,
    
    /// <summary>
    /// Reservation is completed (user has checked out)
    /// </summary>
    Completed,
    
    /// <summary>
    /// Reservation has expired during the payment process
    /// </summary>
    Expired
}
