using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using System;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User : BaseEntity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public NationalId NationalId { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    
    // Private constructor for EF Core
    private User() : base() { }
    
    public User(string firstName, string lastName, string nationalId, string? phoneNumber = null) 
        : base()
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
        if (string.IsNullOrWhiteSpace(nationalId))
            throw new ArgumentException("National ID cannot be empty", nameof(nationalId));
            
        FirstName = firstName;
        LastName = lastName;
        NationalId = new NationalId(nationalId);
        PhoneNumber = phoneNumber;
    }
    
    public void UpdateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
        FirstName = firstName;
        LastName = lastName;
        UpdateModifiedAt();
    }
    
    public void UpdatePhoneNumber(string? phoneNumber)
    {
        PhoneNumber = phoneNumber;
        UpdateModifiedAt();
    }
}
