using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Services;

namespace Nezam.Refahi.Identity.Domain.Repositories;

/// <summary>
/// Repository interface for UserPreference entity operations
/// </summary>
public interface IUserPreferenceRepository : IRepository<UserPreference, Guid>
{
    /// <summary>
    /// Gets a preference by user ID and key
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="key">The preference key</param>
    /// <returns>The preference if found, null otherwise</returns>
    Task<UserPreference?> GetByUserAndKeyAsync(Guid userId, PreferenceKey key);
    
    /// <summary>
    /// Gets a preference by user ID and key string
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="key">The preference key as string</param>
    /// <returns>The preference if found, null otherwise</returns>
    Task<UserPreference?> GetByUserAndKeyAsync(Guid userId, string key);
    
    /// <summary>
    /// Gets all preferences for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Collection of user preferences</returns>
    Task<IEnumerable<UserPreference>> GetByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Gets all active preferences for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Collection of active user preferences</returns>
    Task<IEnumerable<UserPreference>> GetActiveByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Gets preferences by user ID and multiple keys
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="keys">Collection of preference keys</param>
    /// <returns>Collection of matching preferences</returns>
    Task<IEnumerable<UserPreference>> GetByUserAndKeysAsync(Guid userId, IEnumerable<string> keys);
    
    /// <summary>
    /// Gets preferences by user ID and preference type
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="type">The preference type</param>
    /// <returns>Collection of preferences of the specified type</returns>
    Task<IEnumerable<UserPreference>> GetByUserAndTypeAsync(Guid userId, Enums.PreferenceType type);
    
    /// <summary>
    /// Checks if a preference exists for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="key">The preference key</param>
    /// <returns>True if preference exists, false otherwise</returns>
    Task<bool> ExistsAsync(Guid userId, PreferenceKey key);
    
    /// <summary>
    /// Checks if a preference exists for a user by key string
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="key">The preference key as string</param>
    /// <returns>True if preference exists, false otherwise</returns>
    Task<bool> ExistsAsync(Guid userId, string key);
    
    /// <summary>
    /// Gets the count of preferences for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Number of preferences for the user</returns>
    Task<int> GetCountByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Gets the count of active preferences for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Number of active preferences for the user</returns>
    Task<int> GetActiveCountByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Searches preferences by description
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="searchTerm">Search term to match in description</param>
    /// <returns>Collection of matching preferences</returns>
    Task<IEnumerable<UserPreference>> SearchByDescriptionAsync(Guid userId, string searchTerm);
    

    
    /// <summary>
    /// Deletes all preferences for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Number of preferences deleted</returns>
    Task<int> DeleteByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Deactivates all preferences for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Number of preferences deactivated</returns>
    Task<int> DeactivateByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Gets preferences with specific values
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="key">The preference key</param>
    /// <param name="value">The value to search for</param>
    /// <returns>Collection of preferences with the specified value</returns>
    Task<IEnumerable<UserPreference>> GetByValueAsync(Guid userId, string key, string value);
    
    /// <summary>
    /// Gets preferences by user ID and category
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="category">The preference category</param>
    /// <returns>Collection of preferences in the specified category</returns>
    Task<IEnumerable<UserPreference>> GetByUserAndCategoryAsync(Guid userId, PreferenceCategory category);
    
    /// <summary>
    /// Gets active preferences by user ID and category
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="category">The preference category</param>
    /// <returns>Collection of active preferences in the specified category</returns>
    Task<IEnumerable<UserPreference>> GetActiveByUserAndCategoryAsync(Guid userId, PreferenceCategory category);
    
    /// <summary>
    /// Gets the count of preferences for a user by category
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="category">The preference category</param>
    /// <returns>Number of preferences in the specified category</returns>
    Task<int> GetCountByUserAndCategoryAsync(Guid userId, PreferenceCategory category);
    
    /// <summary>
    /// Gets the count of active preferences for a user by category
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="category">The preference category</param>
    /// <returns>Number of active preferences in the specified category</returns>
    Task<int> GetActiveCountByUserAndCategoryAsync(Guid userId, PreferenceCategory category);
}
