using Nezam.Refahi.Identity.Domain.Enums;

namespace Nezam.Refahi.Identity.Domain.Services;

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