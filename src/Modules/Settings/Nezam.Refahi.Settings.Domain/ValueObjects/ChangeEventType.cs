namespace Nezam.Refahi.Settings.Domain.ValueObjects;

/// <summary>
/// Represents the type of change event
/// </summary>
public enum ChangeEventType
{
    ValueChanged = 1,
    DescriptionUpdated = 2,
    StatusChanged = 3,
    DisplayOrderChanged = 4,
    ReadOnlyChanged = 5,
    CategoryChanged = 6,
    SettingCreated = 7,
    SettingDeleted = 8
}
