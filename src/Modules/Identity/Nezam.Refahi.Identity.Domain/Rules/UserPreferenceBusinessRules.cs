using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Enums;

namespace Nezam.Refahi.Identity.Domain.Rules;

/// <summary>
/// Business rules for user preference operations
/// </summary>
public static class UserPreferenceBusinessRules
{
    /// <summary>
    /// Checks if a user can modify a specific preference
    /// </summary>
    public static bool CanUserModifyPreference(User user, string preferenceKey)
    {
        if (user == null)
            return false;

        if (!user.IsActive)
            return false;

        if (user.IsLocked())
            return false;

        // Check if preference is system-managed (read-only)
        if (IsSystemManagedPreference(preferenceKey))
            return false;

        return true;
    }

    /// <summary>
    /// Checks if a preference key is system-managed (read-only)
    /// </summary>
    public static bool IsSystemManagedPreference(string preferenceKey)
    {
        var systemManagedKeys = new[]
        {
            "SYSTEM_VERSION",
            "ACCOUNT_CREATED_AT",
            "LAST_LOGIN_AT"
        };

        return systemManagedKeys.Contains(preferenceKey.ToUpperInvariant());
    }

    /// <summary>
    /// Validates if a preference value is allowed for a specific key
    /// </summary>
    public static bool IsValidPreferenceValue(string preferenceKey, string value, PreferenceType type)
    {
        if (string.IsNullOrWhiteSpace(value))
            return type == PreferenceType.String;

        return preferenceKey.ToUpperInvariant() switch
        {
            "THEME" => IsValidTheme(value),
            "LANGUAGE" => IsValidLanguage(value),
            "TIMEZONE" => IsValidTimezone(value),
            "ITEMS_PER_PAGE" => IsValidItemsPerPage(value),
            "SESSION_TIMEOUT" => IsValidSessionTimeout(value),
            "PROFILE_VISIBILITY" => IsValidProfileVisibility(value),
            _ => true // Allow other preferences
        };
    }

    /// <summary>
    /// Checks if a user can create a preference with the given key
    /// </summary>
    public static bool CanCreatePreference(User user, string preferenceKey)
    {
        if (!CanUserModifyPreference(user, preferenceKey))
            return false;

        // Check if preference key is reserved
        if (IsReservedPreferenceKey(preferenceKey))
            return false;

        return true;
    }

    /// <summary>
    /// Checks if a preference key is reserved
    /// </summary>
    public static bool IsReservedPreferenceKey(string preferenceKey)
    {
        var reservedKeys = new[]
        {
            "USER_ID",
            "CREATED_AT",
            "UPDATED_AT",
            "IS_ACTIVE"
        };

        return reservedKeys.Contains(preferenceKey.ToUpperInvariant());
    }



    // Private validation methods
    private static bool IsValidTheme(string theme)
    {
        var validThemes = new[] { "light", "dark", "auto" };
        return validThemes.Contains(theme.ToLowerInvariant());
    }

    private static bool IsValidLanguage(string language)
    {
        var validLanguages = new[] { "fa", "en", "ar" };
        return validLanguages.Contains(language.ToLowerInvariant());
    }

    private static bool IsValidTimezone(string timezone)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidItemsPerPage(string value)
    {
        if (int.TryParse(value, out int items))
        {
            return items >= 5 && items <= 100;
        }
        return false;
    }

    private static bool IsValidSessionTimeout(string value)
    {
        if (int.TryParse(value, out int minutes))
        {
            return minutes >= 5 && minutes <= 1440; // 5 minutes to 24 hours
        }
        return false;
    }

    private static bool IsValidProfileVisibility(string visibility)
    {
        var validVisibilities = new[] { "public", "private", "friends" };
        return validVisibilities.Contains(visibility.ToLowerInvariant());
    }
}
