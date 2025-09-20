using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Membership.Domain.ValueObjects;

/// <summary>
/// Value object representing a person's full name
/// </summary>
public class FullName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public override string ToString() => $"{FirstName} {LastName}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}