namespace Nezam.Refahi.Identity.Contracts.Dtos;

/// <summary>
/// DTO for ValueObject: PreferenceValue
/// Type باید نام enum (مثلاً Integer/Boolean/Json/...) باشد.
/// </summary>
public  class PreferenceValueDto
{
  public string RawValue { get; set; } = string.Empty;
  public int Type { get; set; } 
}