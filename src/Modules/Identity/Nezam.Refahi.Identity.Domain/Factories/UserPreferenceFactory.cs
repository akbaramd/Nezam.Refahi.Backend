using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Enums;
using Nezam.Refahi.Identity.Domain.Services;
using Nezam.Refahi.Identity.Domain.Rules;

namespace Nezam.Refahi.Identity.Domain.Factories;

/// <summary>
/// Factory for creating user preferences with proper validation and business rules
/// </summary>
public static class UserPreferenceFactory
{
    /// <summary>
    /// Creates a new user preference with validation
    /// </summary>
    public static UserPreference Create(
        User user, 
        string key, 
        object value, 
        PreferenceType type, 
        string description, 
        int displayOrder = 0, 
        PreferenceCategory category = PreferenceCategory.General)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (!UserPreferenceBusinessRules.CanCreatePreference(user, key))
            throw new InvalidOperationException($"User cannot create preference '{key}'");

        var stringValue = ConvertToString(value, type);
        
        if (!UserPreferenceBusinessRules.IsValidPreferenceValue(key, stringValue, type))
            throw new ArgumentException($"Invalid value '{stringValue}' for preference '{key}'");

        var preferenceKey = new PreferenceKey(key);
        var preferenceValue = new PreferenceValue(stringValue, type);

        return new UserPreference(
            user.Id,
            preferenceKey,
            preferenceValue,
            description,
            displayOrder,
            category
        );
    }

    /// <summary>
    /// Creates a user preference from default preference info
    /// </summary>
    public static UserPreference CreateFromDefault(Guid userId, DefaultPreferenceInfo defaultInfo)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (defaultInfo == null)
            throw new ArgumentNullException(nameof(defaultInfo));

        var preferenceKey = new PreferenceKey(defaultInfo.Value);
        var preferenceValue = new PreferenceValue(defaultInfo.Value, defaultInfo.Type);

        return new UserPreference(
            userId,
            preferenceKey,
            preferenceValue,
            defaultInfo.Description,
            defaultInfo.DisplayOrder,
            defaultInfo.Category
        );
    }

    /// <summary>
    /// Creates a theme preference
    /// </summary>
    public static UserPreference CreateThemePreference(User user, string theme)
    {
        return Create(
            user,
            "THEME",
            theme,
            PreferenceType.String,
            "User interface theme preference",
            1,
            PreferenceCategory.UI
        );
    }

    /// <summary>
    /// Creates a language preference
    /// </summary>
    public static UserPreference CreateLanguagePreference(User user, string language)
    {
        return Create(
            user,
            "LANGUAGE",
            language,
            PreferenceType.String,
            "User preferred language",
            2,
            PreferenceCategory.UI
        );
    }

    /// <summary>
    /// Creates a notification preference
    /// </summary>
    public static UserPreference CreateNotificationPreference(User user, string notificationType, bool enabled)
    {
        return Create(
            user,
            notificationType.ToUpperInvariant(),
            enabled,
            PreferenceType.Boolean,
            $"Enable {notificationType.ToLowerInvariant()} notifications",
            10,
            PreferenceCategory.Notifications
        );
    }

    /// <summary>
    /// Creates a security preference
    /// </summary>
    public static UserPreference CreateSecurityPreference(User user, string securityType, object value)
    {
        return Create(
            user,
            securityType.ToUpperInvariant(),
            value,
            GetSecurityPreferenceType(securityType),
            $"Security setting for {securityType.ToLowerInvariant()}",
            40,
            PreferenceCategory.Security
        );
    }

    /// <summary>
    /// Creates a display preference
    /// </summary>
    public static UserPreference CreateDisplayPreference(User user, string displayType, string value)
    {
        return Create(
            user,
            displayType.ToUpperInvariant(),
            value,
            PreferenceType.String,
            $"Display setting for {displayType.ToLowerInvariant()}",
            50,
            PreferenceCategory.Display
        );
    }

    /// <summary>
    /// Creates all default preferences for a user
    /// </summary>
    public static IEnumerable<UserPreference> CreateAllDefaultPreferences(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return UserPreferenceDefaultsService.DefaultPreferences
            .Select(kvp => CreateFromDefault(user.Id, kvp.Value));
    }

    // Private helper methods
    private static string ConvertToString(object value, PreferenceType type)
    {
        return type switch
        {
            PreferenceType.String => value.ToString() ?? string.Empty,
            PreferenceType.Integer => value.ToString() ?? "0",
            PreferenceType.Boolean => value.ToString()?.ToLowerInvariant() ?? "false",
            PreferenceType.Decimal => value.ToString() ?? "0.0",
            PreferenceType.DateTime => value is DateTime dt ? dt.ToString("O") : value.ToString() ?? string.Empty,
            PreferenceType.Json => value is string json ? json : System.Text.Json.JsonSerializer.Serialize(value),
            PreferenceType.Email => value.ToString() ?? string.Empty,
            PreferenceType.Url => value.ToString() ?? string.Empty,
            PreferenceType.PhoneNumber => value.ToString() ?? string.Empty,
            PreferenceType.Color => value.ToString() ?? "#000000",
            _ => value.ToString() ?? string.Empty
        };
    }

    private static PreferenceType GetSecurityPreferenceType(string securityType)
    {
        return securityType.ToUpperInvariant() switch
        {
            "TWO_FACTOR_ENABLED" => PreferenceType.Boolean,
            "SESSION_TIMEOUT" => PreferenceType.Integer,
            "PASSWORD_EXPIRY_DAYS" => PreferenceType.Integer,
            _ => PreferenceType.String
        };
    }
}
