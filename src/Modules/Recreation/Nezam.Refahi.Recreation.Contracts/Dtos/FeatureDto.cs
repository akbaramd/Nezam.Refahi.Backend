namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Feature data transfer object
/// </summary>
public class FeatureDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public string? IconClass { get; set; }
  public Guid? CategoryId { get; set; }
  public int DisplayOrder { get; set; }
  public bool IsActive { get; set; }
  public bool IsRequired { get; set; }
  public string? DefaultValue { get; set; }
  public string? ValidationRules { get; set; }
}