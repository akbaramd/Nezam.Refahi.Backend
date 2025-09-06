using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Contracts;

/// <summary>
/// Contract for settings service operations
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Gets a setting value by key
    /// </summary>
    /// <param name="key">The setting key</param>
    /// <returns>The setting value or null if not found</returns>
    Task<SettingValue?> GetSettingValueAsync(string key);

    /// <summary>
    /// Gets a setting value by key with fallback to default
    /// </summary>
    /// <param name="key">The setting key</param>
    /// <param name="defaultValue">Default value if setting not found</param>
    /// <returns>The setting value or default</returns>
    Task<string> GetSettingValueAsync(string key, string defaultValue);

    /// <summary>
    /// Gets a typed setting value by key
    /// </summary>
    /// <typeparam name="T">The expected type</typeparam>
    /// <param name="key">The setting key</param>
    /// <returns>The typed setting value or default</returns>
    Task<T> GetSettingValueAsync<T>(string key);

    /// <summary>
    /// Gets a typed setting value by key with fallback to default
    /// </summary>
    /// <typeparam name="T">The expected type</typeparam>
    /// <param name="key">The setting key</param>
    /// <param name="defaultValue">Default value if setting not found</param>
    /// <returns>The typed setting value or default</returns>
    Task<T> GetSettingValueAsync<T>(string key, T defaultValue);

    /// <summary>
    /// Gets all settings by section
    /// </summary>
    /// <param name="sectionName">The section name</param>
    /// <returns>Dictionary of setting key-value pairs</returns>
    Task<Dictionary<string, string>> GetSettingsBySectionAsync(string sectionName);

    /// <summary>
    /// Checks if a setting exists
    /// </summary>
    /// <param name="key">The setting key</param>
    /// <returns>True if setting exists, false otherwise</returns>
    Task<bool> SettingExistsAsync(string key);
}
