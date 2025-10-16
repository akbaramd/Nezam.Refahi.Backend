namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;

/// <summary>
/// Applicant information data transfer object
/// </summary>
public record ApplicantInfoDto
{
  /// <summary>
  /// Member ID
  /// </summary>
  public Guid MemberId { get; init; }

  /// <summary>
  /// User full name
  /// </summary>
  public string? FullName { get; init; }

  /// <summary>
  /// User national ID
  /// </summary>
  public string? NationalId { get; init; }

  /// <summary>
  /// Indicates if applicant information is complete
  /// </summary>
  public bool IsComplete => !string.IsNullOrWhiteSpace(FullName) && !string.IsNullOrWhiteSpace(NationalId);
}