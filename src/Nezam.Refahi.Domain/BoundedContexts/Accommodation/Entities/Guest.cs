using System;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;

/// <summary>
/// Represents a guest staying at a hotel
/// </summary>
public class Guest : BaseEntity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public NationalId NationalId { get; private set; } = null!;
    public int Age { get; private set; }
    
    // Private constructor for EF Core
    private Guest() : base() { }
    
    public Guest(string firstName, string lastName, string nationalId, int age) 
        : base()
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
        if (age <= 0)
            throw new ArgumentException("Age must be positive", nameof(age));
            
        FirstName = firstName;
        LastName = lastName;
        NationalId = new NationalId(nationalId);
        Age = age;
    }
    
    public string FullName => $"{FirstName} {LastName}";
    
    public void UpdatePersonalInfo(string firstName, string lastName, int age)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
        if (age <= 0)
            throw new ArgumentException("Age must be positive", nameof(age));
            
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        UpdateModifiedAt();
    }
}
