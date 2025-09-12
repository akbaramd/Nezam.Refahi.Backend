using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Enums;
using Nezam.Refahi.Identity.Domain.Services;

namespace Nezam.Refahi.Identity.Domain.Specifications;

/// <summary>
/// Specifications for querying user preferences
/// </summary>
public static class UserPreferenceSpecifications
{
    /// <summary>
    /// Specification for active preferences
    /// </summary>
    public static class Active
    {
        public static bool IsSatisfiedBy(UserPreference preference)
        {
            return preference.IsActive;
        }
    }

    /// <summary>
    /// Specification for preferences by category
    /// </summary>
    public static class ByCategory
    {
        public static Func<UserPreference, bool> Create(PreferenceCategory category)
        {
            return preference => preference.Category == category;
        }
    }

    /// <summary>
    /// Specification for preferences by type
    /// </summary>
    public static class ByType
    {
        public static Func<UserPreference, bool> Create(PreferenceType type)
        {
            return preference => preference.Value.Type == type;
        }
    }

    /// <summary>
    /// Specification for preferences by key pattern
    /// </summary>
    public static class ByKeyPattern
    {
        public static Func<UserPreference, bool> Create(string pattern)
        {
            return preference => preference.Key.Value.Contains(pattern, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Specification for preferences by value
    /// </summary>
    public static class ByValue
    {
        public static Func<UserPreference, bool> Create(string value)
        {
            return preference => preference.Value.RawValue.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Specification for UI preferences
    /// </summary>
    public static class UIPreferences
    {
        public static bool IsSatisfiedBy(UserPreference preference)
        {
            return preference.Category == PreferenceCategory.UI && preference.IsActive;
        }
    }

    /// <summary>
    /// Specification for notification preferences
    /// </summary>
    public static class NotificationPreferences
    {
        public static bool IsSatisfiedBy(UserPreference preference)
        {
            return preference.Category == PreferenceCategory.Notifications && preference.IsActive;
        }
    }

    /// <summary>
    /// Specification for security preferences
    /// </summary>
    public static class SecurityPreferences
    {
        public static bool IsSatisfiedBy(UserPreference preference)
        {
            return preference.Category == PreferenceCategory.Security && preference.IsActive;
        }
    }

    /// <summary>
    /// Specification for default preferences
    /// </summary>
    public static class DefaultPreferences
    {
        public static bool IsSatisfiedBy(UserPreference preference)
        {
            return UserPreferenceDefaultsService.DefaultPreferences.ContainsKey(preference.Key.Value);
        }
    }

    /// <summary>
    /// Specification for custom preferences (non-default)
    /// </summary>
    public static class CustomPreferences
    {
        public static bool IsSatisfiedBy(UserPreference preference)
        {
            return !UserPreferenceDefaultsService.DefaultPreferences.ContainsKey(preference.Key.Value);
        }
    }

 

    /// <summary>
    /// Specification for preferences with specific display order range
    /// </summary>
    public static class ByDisplayOrderRange
    {
        public static Func<UserPreference, bool> Create(int minOrder, int maxOrder)
        {
            return preference => preference.DisplayOrder >= minOrder && preference.DisplayOrder <= maxOrder;
        }
    }

    /// <summary>
    /// Specification for boolean preferences
    /// </summary>
    public static class BooleanPreferences
    {
        public static bool IsSatisfiedBy(UserPreference preference)
        {
            return preference.Value.Type == PreferenceType.Boolean;
        }
    }

    /// <summary>
    /// Specification for string preferences
    /// </summary>
    public static class StringPreferences
    {
        public static bool IsSatisfiedBy(UserPreference preference)
        {
            return preference.Value.Type == PreferenceType.String;
        }
    }

    /// <summary>
    /// Specification for numeric preferences
    /// </summary>
    public static class NumericPreferences
    {
        public static bool IsSatisfiedBy(UserPreference preference)
        {
            return preference.Value.Type == PreferenceType.Integer || 
                   preference.Value.Type == PreferenceType.Decimal;
        }
    }
}