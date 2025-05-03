using System;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;

/// <summary>
/// Represents a date range for a hotel reservation
/// </summary>
public class DateRange : IEquatable<DateRange>
{
    // Private setters for EF Core compatibility
    public DateOnly CheckIn { get; private set; }
    public DateOnly CheckOut { get; private set; }
    
    /// <summary>
    /// The number of nights in the reservation
    /// </summary>
    public int NightCount => CheckOut.DayNumber - CheckIn.DayNumber;

    // Required parameterless constructor for EF Core
    protected DateRange() { }

    public DateRange(DateOnly checkIn, DateOnly checkOut)
    {
        if (checkIn > checkOut)
            throw new ArgumentException("Check-in date cannot be after check-out date", nameof(checkIn));
        
        if (checkIn == checkOut)
            throw new ArgumentException("Check-in and check-out cannot be on the same day", nameof(checkOut));
            
        CheckIn = checkIn;
        CheckOut = checkOut;
    }
    
    /// <summary>
    /// Checks if this date range overlaps with another date range
    /// </summary>
    public bool OverlapsWith(DateRange other)
    {
        return CheckIn < other.CheckOut && CheckOut > other.CheckIn;
    }
    
    /// <summary>
    /// Checks if a specific date is within this date range (inclusive check-in, exclusive check-out)
    /// </summary>
    public bool Contains(DateOnly date)
    {
        return date >= CheckIn && date < CheckOut;
    }
    
    public bool Equals(DateRange? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return CheckIn.Equals(other.CheckIn) && CheckOut.Equals(other.CheckOut);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((DateRange)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CheckIn, CheckOut);
    }

    public static bool operator ==(DateRange? left, DateRange? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(DateRange? left, DateRange? right)
    {
        return !(left == right);
    }
    
    public override string ToString() => $"{CheckIn:yyyy/MM/dd} to {CheckOut:yyyy/MM/dd} ({NightCount} nights)";
}
