using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Enums;
using System;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public NationalId? NationalId { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public Role Role { get; private set; } = Role.User;
    public DateTime? LastLoginAt { get; private set; }
    // Private constructor for EF Core
    private User() : base() { }
    
    /// <summary>
    /// Creates a new user with full details
    /// </summary>
    public User(string firstName, string lastName, string nationalId, string? phoneNumber = null, Role role = Role.User) 
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
        PhoneNumber = phoneNumber ?? string.Empty;
        Role = role;
    }
    
    /// <summary>
    /// Creates a new user with just a phone number (for OTP authentication)
    /// </summary>
    public User(string phoneNumber, Role role = Role.User)
        : base()
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
            
        PhoneNumber = phoneNumber;
        Role = role;
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
    
    public void UpdatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
            
        PhoneNumber = phoneNumber;
        UpdateModifiedAt();
    }
    
    /// <summary>
    /// Assigns the specified role to the user
    /// </summary>
    /// <param name="role">The role to assign</param>
    public void AssignRole(Role role)
    {
        Role = role;
        UpdateModifiedAt();
    }
    
    /// <summary>
    /// Adds the specified role to the user's existing role
    /// </summary>
    /// <param name="role">The role to add</param>
    public void AddRole(Role role)
    {
        Role |= role;
        UpdateModifiedAt();
    }
    
    /// <summary>
    /// Removes the specified role from the user
    /// </summary>
    /// <param name="role">The role to remove</param>
    public void RemoveRole(Role role)
    {
        Role &= ~role;
        UpdateModifiedAt();
    }
    
    /// <summary>
    /// Checks if the user has the specified role
    /// </summary>
    /// <param name="role">The role to check</param>
    /// <returns>True if the user has the role, false otherwise</returns>
    public bool HasRole(Role role)
    {
        return (Role & role) == role;
    }
    
    /// <summary>
    /// Updates the user's profile information
    /// </summary>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name</param>
    /// <param name="nationalId">User's national ID</param>
    public void UpdateProfile(string firstName, string lastName, NationalId nationalId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
        if (nationalId == null)
            throw new ArgumentException("National ID cannot be null", nameof(nationalId));
        
        // Update basic information
        FirstName = firstName;
        LastName = lastName;
        NationalId = nationalId;
        

        
  
        
        UpdateModifiedAt();
    }
    
    /// <summary>
    /// Gets the user's roles as a list
    /// </summary>
    /// <returns>List of roles the user has</returns>
    public List<Role> GetRoles()
    {
        var roles = new List<Role>();
        foreach (Role role in Enum.GetValues(typeof(Role)))
        {
            if (HasRole(role) && role != Role.None)
            {
                roles.Add(role);
            }
        }
        return roles;
    }

    public void LoggedIn()
    {
        // create field and set lastLoginAt
        LastLoginAt = DateTime.UtcNow;
    }
}
