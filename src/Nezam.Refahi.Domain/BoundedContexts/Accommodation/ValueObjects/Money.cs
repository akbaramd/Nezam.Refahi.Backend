using System;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;

/// <summary>
/// Represents a monetary amount with currency
/// </summary>
public class Money : IEquatable<Money>
{
    // Use private setters for EF Core compatibility
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;
    
    public static Money Zero => new Money(0, "IRR");
    public bool IsZero => Amount == 0;
    public bool IsFree => IsZero;

    // Factory method to create Money from decimal amount
    public static Money FromDecimal(decimal amount, string currency = "IRR")
    {
        return new Money(amount, currency);
    }

    // Required parameterless constructor for EF Core
    protected Money() { }

    public Money(decimal amount, string currency = "IRR")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));
            
        Amount = amount;
        Currency = currency;
    }
    
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
            
        return new Money(left.Amount + right.Amount, left.Currency);
    }
    
    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
            
        if (left.Amount < right.Amount)
            throw new InvalidOperationException("Cannot result in negative money");
            
        return new Money(left.Amount - right.Amount, left.Currency);
    }
    
    public static Money operator *(Money money, decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentException("Multiplier cannot be negative", nameof(multiplier));
            
        return new Money(money.Amount * multiplier, money.Currency);
    }
    
    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && 
               string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Money)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency.ToLowerInvariant());
    }

    public static bool operator ==(Money? left, Money? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !(left == right);
    }
    
    public override string ToString() => $"{Amount:N0} {Currency}";
}
