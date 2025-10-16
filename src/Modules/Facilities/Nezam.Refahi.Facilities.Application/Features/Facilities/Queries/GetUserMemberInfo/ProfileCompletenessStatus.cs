namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

/// <summary>
/// Profile completeness status enumeration
/// </summary>
public enum ProfileCompletenessStatus
{
  /// <summary>
  /// Profile is incomplete
  /// </summary>
  Incomplete,

  /// <summary>
  /// Profile is partially complete
  /// </summary>
  Partial,

  /// <summary>
  /// Profile is complete
  /// </summary>
  Complete,

  /// <summary>
  /// Profile is fully complete with all optional fields
  /// </summary>
  FullyComplete
}