using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Enums;

namespace Nezam.Refahi.Identity.Domain.Services;

/// <summary>
/// Service for managing default user preferences
/// This service ensures all users have essential preferences set
/// </summary>
public static class UserPreferenceDefaultsService
{
    /// <summary>
    /// Default preferences that every user should have
    /// </summary>
    public static readonly Dictionary<string, DefaultPreferenceInfo> DefaultPreferences = new()
    {
        // UI/UX Preferences
        ["THEME"] = new("light", PreferenceType.String, "User interface theme preference", 1, PreferenceCategory.UI),
        ["LANGUAGE"] = new("fa", PreferenceType.String, "User preferred language", 2, PreferenceCategory.UI),
        ["TIMEZONE"] = new("Asia/Tehran", PreferenceType.String, "User timezone", 3, PreferenceCategory.UI),
        
        // Notification Preferences
        ["EMAIL_NOTIFICATIONS"] = new("true", PreferenceType.Boolean, "Enable email notifications", 10, PreferenceCategory.Notifications),
        ["SMS_NOTIFICATIONS"] = new("false", PreferenceType.Boolean, "Enable SMS notifications", 11, PreferenceCategory.Notifications),
        ["PUSH_NOTIFICATIONS"] = new("true", PreferenceType.Boolean, "Enable push notifications", 12, PreferenceCategory.Notifications),
        
        // Privacy Preferences
        ["PROFILE_VISIBILITY"] = new("private", PreferenceType.String, "Profile visibility setting", 20, PreferenceCategory.Privacy),
        ["SHOW_ONLINE_STATUS"] = new("true", PreferenceType.Boolean, "Show online status to others", 21, PreferenceCategory.Privacy),
        
        // Application Preferences
        ["ITEMS_PER_PAGE"] = new("20", PreferenceType.Integer, "Number of items to show per page", 30, PreferenceCategory.Application),
        ["AUTO_SAVE"] = new("true", PreferenceType.Boolean, "Auto-save form data", 31, PreferenceCategory.Application),
        ["REMEMBER_ME"] = new("false", PreferenceType.Boolean, "Remember user login", 32, PreferenceCategory.Application),
        
        // Security Preferences
        ["TWO_FACTOR_ENABLED"] = new("false", PreferenceType.Boolean, "Two-factor authentication enabled", 40, PreferenceCategory.Security),
        ["SESSION_TIMEOUT"] = new("30", PreferenceType.Integer, "Session timeout in minutes", 41, PreferenceCategory.Security),
        
        // Display Preferences
        ["DATE_FORMAT"] = new("yyyy/MM/dd", PreferenceType.String, "Preferred date format", 50, PreferenceCategory.Display),
        ["CURRENCY"] = new("IRR", PreferenceType.String, "Preferred currency", 51, PreferenceCategory.Display),
        ["NUMBER_FORMAT"] = new("fa-IR", PreferenceType.String, "Number format locale", 52, PreferenceCategory.Display)
    };

    /// <summary>
    /// Creates default preferences for a new user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Collection of default user preferences</returns>
    public static IEnumerable<UserPreference> CreateDefaultPreferences(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        return DefaultPreferences.Select(kvp =>
        {
            var key = new PreferenceKey(kvp.Key);
            var value = new PreferenceValue(kvp.Value.Value, kvp.Value.Type);
            
            return new UserPreference(
                userId,
                key,
                value,
                kvp.Value.Description,
                kvp.Value.DisplayOrder,
                kvp.Value.Category
            );
        });
    }

    /// <summary>
    /// Gets missing default preferences for an existing user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="existingPreferences">User's current preferences</param>
    /// <returns>Collection of missing default preferences</returns>
    public static IEnumerable<UserPreference> GetMissingDefaultPreferences(Guid userId, IEnumerable<UserPreference> existingPreferences)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        var existingKeys = existingPreferences
            .Where(p => p.IsActive)
            .Select(p => p.Key.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingPreferences = DefaultPreferences
            .Where(kvp => !existingKeys.Contains(kvp.Key))
            .Select(kvp =>
            {
                var key = new PreferenceKey(kvp.Key);
                var value = new PreferenceValue(kvp.Value.Value, kvp.Value.Type);
                
                return new UserPreference(
                    userId,
                    key,
                    value,
                    kvp.Value.Description,
                    kvp.Value.DisplayOrder,
                    kvp.Value.Category
                );
            });

        return missingPreferences;
    }

    /// <summary>
    /// Checks if a user has all required default preferences
    /// </summary>
    /// <param name="existingPreferences">User's current preferences</param>
    /// <returns>True if user has all default preferences, false otherwise</returns>
    public static bool HasAllDefaultPreferences(IEnumerable<UserPreference> existingPreferences)
    {
        var existingKeys = existingPreferences
            .Where(p => p.IsActive)
            .Select(p => p.Key.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return DefaultPreferences.Keys.All(key => existingKeys.Contains(key));
    }

    /// <summary>
    /// Gets the count of missing default preferences
    /// </summary>
    /// <param name="existingPreferences">User's current preferences</param>
    /// <returns>Number of missing default preferences</returns>
    public static int GetMissingDefaultPreferencesCount(IEnumerable<UserPreference> existingPreferences)
    {
        var existingKeys = existingPreferences
            .Where(p => p.IsActive)
            .Select(p => p.Key.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return DefaultPreferences.Keys.Count(key => !existingKeys.Contains(key));
    }

    /// <summary>
    /// Gets default preference information by key
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <returns>Default preference info if found, null otherwise</returns>
    public static DefaultPreferenceInfo? GetDefaultPreferenceInfo(string key)
    {
        return DefaultPreferences.TryGetValue(key, out var info) ? info : null;
    }

    /// <summary>
    /// Gets all default preference keys
    /// </summary>
    /// <returns>Collection of all default preference keys</returns>
    public static IEnumerable<string> GetAllDefaultPreferenceKeys()
    {
        return DefaultPreferences.Keys;
    }

    /// <summary>
    /// Gets default preferences by category
    /// </summary>
    /// <param name="category">The category to filter by</param>
    /// <returns>Collection of default preferences in the specified category</returns>
    public static IEnumerable<KeyValuePair<string, DefaultPreferenceInfo>> GetDefaultPreferencesByCategory(PreferenceCategory category)
    {
        return DefaultPreferences.Where(kvp => kvp.Value.Category == category);
    }
}

/// <summary>
/// Information about a default preference
/// </summary>
public class DefaultPreferenceInfo
{
    public string Value { get; }
    public PreferenceType Type { get; }
    public string Description { get; }
    public int DisplayOrder { get; }
    public PreferenceCategory Category { get; }

    public DefaultPreferenceInfo(string value, PreferenceType type, string description, int displayOrder, PreferenceCategory category = PreferenceCategory.General)
    {
        Value = value;
        Type = type;
        Description = description;
        DisplayOrder = displayOrder;
        Category = category;
    }
}

/// <summary>
/// Categories for organizing preferences
/// </summary>
public enum PreferenceCategory
{
    General = 1,
    UI = 2,
    Notifications = 3,
    Privacy = 4,
    Security = 5,
    Display = 6,
    Application = 7
}
