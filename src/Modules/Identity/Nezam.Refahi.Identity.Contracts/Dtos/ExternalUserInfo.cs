namespace Nezam.Refahi.Identity.Contracts.Dtos;

/// <summary>
/// External user information for Identity context
/// Used to get complete user data from external sources for account creation
/// </summary>
public class ExternalUserInfo
{
    public Guid ExternalUserId { get; init; }
    public string NationalCode { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? ExternalId { get; init; }
    public DateTime? BirthDate { get; init; }
    public string? Address { get; init; }
    public bool IsActiveUser { get; init; }
    public DateTime? StatusStartDate { get; init; }
    public DateTime? StatusEndDate { get; init; }
    public string? UserRole { get; init; }
    public Dictionary<string, object> AdditionalData { get; init; } = new();
    
    /// <summary>
    /// Gets the user's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Gets the user status description
    /// </summary>
    public string UserStatus
    {
        get
        {
            if (!IsActiveUser)
                return "Inactive";
                
            if (!StatusEndDate.HasValue)
                return "Active (Permanent)";
                
            return DateTime.UtcNow <= StatusEndDate.Value 
                ? "Active" 
                : "Expired";
        }
    }
    
    /// <summary>
    /// Gets the days until status expires (null if permanent or expired)
    /// </summary>
    public int? DaysUntilExpiry
    {
        get
        {
            if (!IsActiveUser || !StatusEndDate.HasValue)
                return null;
                
            var days = (int)(StatusEndDate.Value - DateTime.UtcNow).TotalDays;
            return days > 0 ? days : 0;
        }
    }
    
    /// <summary>
    /// Indicates if the status will expire soon (within 30 days)
    /// </summary>
    public bool IsExpiringSoon => DaysUntilExpiry.HasValue && DaysUntilExpiry <= 30;
    
    /// <summary>
    /// Indicates if this user can create an Identity account
    /// </summary>
    public bool CanCreateAccount => IsActiveUser && 
        !string.IsNullOrWhiteSpace(FirstName) && 
        !string.IsNullOrWhiteSpace(LastName) &&
        !string.IsNullOrWhiteSpace(NationalCode);
}