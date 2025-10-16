namespace Nezam.Refahi.Identity.Domain.Dtos;

/// <summary>
/// User validation information specifically for Identity context needs
/// Used during user registration and authentication processes
/// </summary>
public class UserValidationInfo
{
    public Guid UserId { get; init; }
    public string NationalCode { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? ExternalId { get; init; }
    public bool IsActiveUser { get; init; }
    public bool HasValidStatus { get; init; }
    public DateTime? StatusExpiryDate { get; init; }
    public string? UserRole { get; init; }
    
    /// <summary>
    /// Gets the user's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Indicates if this user can be used for account creation
    /// </summary>
    public bool CanCreateUserAccount => IsActiveUser && HasValidStatus;
    
    /// <summary>
    /// Gets the display name for user interface
    /// </summary>
    public string DisplayName => !string.IsNullOrWhiteSpace(ExternalId) 
        ? $"{FullName} ({ExternalId})" 
        : FullName;
    
    /// <summary>
    /// Creates an empty validation result for non-existent users
    /// </summary>
    public static UserValidationInfo Empty(string nationalCode) => new()
    {
        UserId = Guid.Empty,
        NationalCode = nationalCode,
        FirstName = string.Empty,
        LastName = string.Empty,
        IsActiveUser = false,
        HasValidStatus = false
    };
}