namespace Nezam.Refahi.Identity.Contracts.Dtos;

/// <summary>
/// Result of user lookup operations for batch processing
/// Used when Identity needs to validate multiple users at once
/// </summary>
public class UserLookupResult
{
    public string SearchKey { get; init; } = string.Empty;
    public UserLookupType SearchType { get; init; }
    public bool Found { get; init; }
    public UserValidationInfo? UserInfo { get; init; }
    public string? ErrorMessage { get; init; }
    
    /// <summary>
    /// Creates a successful lookup result
    /// </summary>
    public static UserLookupResult Success(
        string searchKey, 
        UserLookupType searchType, 
        UserValidationInfo userInfo) => new()
    {
        SearchKey = searchKey,
        SearchType = searchType,
        Found = true,
        UserInfo = userInfo
    };
    
    /// <summary>
    /// Creates a not found result
    /// </summary>
    public static UserLookupResult NotFound(
        string searchKey, 
        UserLookupType searchType) => new()
    {
        SearchKey = searchKey,
        SearchType = searchType,
        Found = false,
        UserInfo = null
    };
    
    /// <summary>
    /// Creates an error result
    /// </summary>
    public static UserLookupResult Error(
        string searchKey, 
        UserLookupType searchType, 
        string errorMessage) => new()
    {
        SearchKey = searchKey,
        SearchType = searchType,
        Found = false,
        UserInfo = null,
        ErrorMessage = errorMessage
    };
}