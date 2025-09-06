namespace Nezam.Refahi.Settings.Contracts.Constants;

/// <summary>
/// Constants for application settings keys and default values
/// </summary>
public static class SettingsConstants
{
    /// <summary>
    /// Web Application Settings
    /// </summary>
    public static class WebApp
    {
        public const string Name = "WEBAPP_NAME";
        public const string Description = "WEBAPP_DESCRIPTION";
        public const string Logo = "WEBAPP_LOGO";
        public const string Version = "WEBAPP_VERSION";

    }

    /// <summary>
    /// Webhook Configuration Settings
    /// </summary>
    public static class Webhooks
    {
        public const string EngineerMemberServiceUrl = "WEBHOOK_ENGINEER_MEMBER_SERVICE_URL";
        public const string EngineerServiceTimeout = "ENGINEER_SERVICE_TIMEOUT_SECONDS";
        public const string EngineerServiceMaxRetries = "ENGINEER_SERVICE_MAX_RETRIES";
        public const string EngineerServiceRetryDelay = "ENGINEER_SERVICE_RETRY_DELAY_MS";
    }



}
