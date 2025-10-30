using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Shared.Domain.ValueObjects;

/// <summary>
/// Value object representing money amount in Iranian Rials
/// </summary>
public class Money : ValueObject
{
    private readonly decimal _amountRials;
    private readonly string _currency;
    
    public static Money Zero =  new Money(0);

    public decimal AmountRials => _amountRials;
    public string Currency => _currency;

    public Money(decimal amountRials)
    {
        if (amountRials < 0)
            throw new ArgumentException("Amount must be >= 0", nameof(amountRials));

        _amountRials = amountRials;
        _currency = "IRR";
    }

    public static Money FromRials(decimal other)
    {
      return new Money(other);
    }

    public Money Multiply(decimal other)
    {
        return new Money(_amountRials * other);
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(checked(_amountRials + other._amountRials));
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        var result = _amountRials - other._amountRials;
        if (result < 0)
            throw new InvalidOperationException("Resulting money cannot be negative.");
        return new Money(result);
    }

    public bool IsGreaterThan(Money other)
    {
        EnsureSameCurrency(other);
        return _amountRials > other._amountRials;
    }

    public bool IsLessThan(Money other)
    {
        EnsureSameCurrency(other);
        return _amountRials < other._amountRials;
    }

    private void EnsureSameCurrency(Money other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));
        if (!string.Equals(_currency, other._currency, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Currency mismatch.");
    }

    public override string ToString() => $"{_amountRials:N0} {_currency}";

    public static implicit operator string(Money money) => money.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _amountRials;
        yield return _currency;
    }
}