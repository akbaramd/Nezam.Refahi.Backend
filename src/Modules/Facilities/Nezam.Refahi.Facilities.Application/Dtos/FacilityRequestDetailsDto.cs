using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequestDetails;

/// <summary>
/// Detailed facility request data transfer object
/// Contains full request information including all details
/// </summary>
public record FacilityRequestDetailsDto : FacilityRequestDto
{
  /// <summary>
  /// Request number
  /// </summary>
  public string RequestNumber { get; init; } = null!;

  /// <summary>
  /// Request description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Rejection reason (if rejected)
  /// </summary>
  public string? RejectionReason { get; init; }

  /// <summary>
  /// Financial information
  /// </summary>
  public RequestFinancialInfoDto FinancialInfo { get; init; } = null!;

  /// <summary>
  /// Request status details
  /// </summary>
  public RequestStatusDto StatusDetails { get; init; } = null!;

  /// <summary>
  /// Request timeline
  /// </summary>
  public RequestTimelineDto Timeline { get; init; } = null!;

  /// <summary>
  /// Request metadata
  /// </summary>
  public Dictionary<string, string> Metadata { get; init; } = new();

  /// <summary>
  /// Indicates if status is terminal
  /// </summary>
  public bool IsTerminal { get; init; }

  /// <summary>
  /// Indicates if request can be cancelled
  /// </summary>
  public bool CanBeCancelled { get; init; }

  /// <summary>
  /// Indicates if request requires applicant action
  /// </summary>
  public bool RequiresApplicantAction { get; init; }

  /// <summary>
  /// Indicates if request requires bank action
  /// </summary>
  public bool RequiresBankAction { get; init; }

  /// <summary>
  /// Days until bank appointment (if scheduled)
  /// </summary>
  public int? DaysUntilBankAppointment { get; init; }

  /// <summary>
  /// Indicates if bank appointment is overdue
  /// </summary>
  public bool IsBankAppointmentOverdue { get; init; }

  /// <summary>
  /// Last modification timestamp
  /// </summary>
  public DateTime? LastModifiedAt { get; init; }

  /// <summary>
  /// Facility details (if requested)
  /// </summary>
  public FacilityDetailsDto? FacilityDetails { get; init; }

  /// <summary>
  /// Cycle details (if requested)
  /// </summary>
  public FacilityCycleDetailsDto? CycleDetails { get; init; }

  /// <summary>
  /// Policy snapshot at time of request
  /// </summary>
}