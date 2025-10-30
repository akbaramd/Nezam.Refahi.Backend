namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Simple user member information data transfer object
/// Contains only essential fields for member info display
/// </summary>
public record UserMemberInfoDto
{
  /// <summary>
  /// Member ID
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Member full name
  /// </summary>
  public string FullName { get; init; } = null!;

  /// <summary>
  /// Member national ID
  /// </summary>
  public string NationalId { get; init; } = null!;

  /// <summary>
  /// Member phone number
  /// </summary>
  public string? PhoneNumber { get; init; }


}