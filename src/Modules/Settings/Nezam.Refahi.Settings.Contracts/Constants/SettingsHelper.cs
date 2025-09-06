namespace Nezam.Refahi.Settings.Contracts.Constants;

/// <summary>
/// Helper class for working with settings constants and default values
/// </summary>
public static class SettingsHelper
{
    /// <summary>
    /// Gets the default value for a setting key
    /// </summary>
    /// <param name="key">The setting key</param>
    /// <returns>The default value or empty string if not found</returns>
    public static string GetDefaultValue(string key)
    {
        return key switch
        {
            // WebApp defaults
            SettingsConstants.WebApp.Name => SettingsDefaultValues.WebApp.Name,
            SettingsConstants.WebApp.Description => SettingsDefaultValues.WebApp.Description,
            SettingsConstants.WebApp.Logo => SettingsDefaultValues.WebApp.Logo,
            SettingsConstants.WebApp.Version => SettingsDefaultValues.WebApp.Version,

            // Webhook defaults
            SettingsConstants.Webhooks.EngineerMemberServiceUrl => SettingsDefaultValues.Webhooks.EngineerMemberServiceUrl,

        


            _ => string.Empty
        };
    }

    /// <summary>
    /// Checks if a setting key is a webhook configuration
    /// </summary>
    /// <param name="key">The setting key</param>
    /// <returns>True if it's a webhook setting</returns>
    public static bool IsWebhookSetting(string key)
    {
        return key.StartsWith("WEBHOOK_", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if a setting key is an authentication configuration
    /// </summary>
    /// <param name="key">The setting key</param>
    /// <returns>True if it's an authentication setting</returns>
    public static bool IsAuthenticationSetting(string key)
    {
        return key.StartsWith("AUTH_", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if a setting key is a webapp configuration
    /// </summary>
    /// <param name="key">The setting key</param>
    /// <returns>True if it's a webapp setting</returns>
    public static bool IsWebAppSetting(string key)
    {
        return key.StartsWith("WEBAPP_", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets all webhook settings keys
    /// </summary>
    /// <returns>Array of webhook setting keys</returns>
    public static string[] GetWebhookSettings()
    {
        return new[]
        {
            SettingsConstants.Webhooks.EngineerMemberServiceUrl,

        };
    }

    /// <summary>
    /// Gets all webapp settings keys
    /// </summary>
    /// <returns>Array of webapp setting keys</returns>
    public static string[] GetWebAppSettings()
    {
        return new[]
        {
            SettingsConstants.WebApp.Name,
            SettingsConstants.WebApp.Description,
            SettingsConstants.WebApp.Logo,
            SettingsConstants.WebApp.Version,

        };
    }

    /// <summary>
    /// Checks if engineer member service webhook is enabled
    /// </summary>
    /// <param name="settings">Dictionary of settings</param>
    /// <returns>True if enabled and URL is configured</returns>
    public static bool IsEngineerMemberServiceEnabled(IDictionary<string, string> settings)
    {
        if (!settings.TryGetValue(SettingsConstants.Webhooks.EngineerMemberServiceUrl, out var enabled))
            return false;

        if (!bool.TryParse(enabled, out var isEnabled) || !isEnabled)
            return false;

        if (!settings.TryGetValue(SettingsConstants.Webhooks.EngineerMemberServiceUrl, out var url))
            return false;

        return !string.IsNullOrWhiteSpace(url);
    }

    /// <summary>
    /// Gets engineer member service webhook URL
    /// </summary>
    /// <param name="settings">Dictionary of settings</param>
    /// <returns>Webhook URL or empty string if not configured</returns>
    public static string GetEngineerMemberServiceUrl(IDictionary<string, string> settings)
    {
        if (!IsEngineerMemberServiceEnabled(settings))
            return string.Empty;

        settings.TryGetValue(SettingsConstants.Webhooks.EngineerMemberServiceUrl, out var url);
        return url ?? string.Empty;
    }
}
