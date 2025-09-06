namespace Nezam.Refahi.Settings.Domain.ValueObjects;

/// <summary>
/// Represents the status of a setting
/// </summary>
public enum SettingStatus
{
    Active = 1,
    Inactive = 2,
    Deprecated = 3,
    Pending = 4,
    Archived = 5
}
