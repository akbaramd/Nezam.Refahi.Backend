namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;

/// <summary>
/// Request status categories for filtering
/// </summary>
public enum RequestStatusCategory
{
  /// <summary>
  /// All requests
  /// </summary>
  All,

  /// <summary>
  /// In progress requests
  /// </summary>
  InProgress,

  /// <summary>
  /// Completed requests
  /// </summary>
  Completed,

  /// <summary>
  /// Rejected requests
  /// </summary>
  Rejected,

  /// <summary>
  /// Cancelled requests
  /// </summary>
  Cancelled,

  /// <summary>
  /// Terminal requests (completed, rejected, cancelled)
  /// </summary>
  Terminal
}