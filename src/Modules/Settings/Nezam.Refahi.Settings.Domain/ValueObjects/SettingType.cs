namespace Nezam.Refahi.Settings.Domain.ValueObjects;

/// <summary>
/// Represents the data type of a setting value
/// </summary>
public enum SettingType
{
    String = 1,
    Integer = 2,
    Boolean = 3,
    Decimal = 4,
    DateTime = 5,
    Json = 6,
    Email = 7,
    Url = 8,
    PhoneNumber = 9,
    Color = 10
}
