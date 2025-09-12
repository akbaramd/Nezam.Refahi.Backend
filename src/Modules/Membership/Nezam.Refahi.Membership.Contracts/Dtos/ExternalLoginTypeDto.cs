using System.Text.Json.Serialization;

namespace Nezam.Refahi.Membership.Contracts.Dtos;

public class ExternalLoginTypeDto
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;

  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("description")]
  public string Description { get; set; } = string.Empty;
}