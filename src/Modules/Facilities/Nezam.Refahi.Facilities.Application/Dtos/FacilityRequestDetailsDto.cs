namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Detailed facility request data transfer object
/// Contains full request information including all details
/// </summary>
public class FacilityRequestDetailsDto : FacilityRequestDto
{
  /// <summary>
  /// Request number
  /// </summary>
  public string RequestNumber { get; set; } = null!;


  /// <summary>
  /// Request description 
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Rejection reason (if rejected)
  /// </summary>
  public RequestFinancialInfoDto FinancialInfo { get; set; } = null!;

  /// <summary>
  /// Request status details
  /// </summary>
  public RequestStatusDto StatusDetails { get; set; } = null!;

  /// <summary>
  /// Request timeline
  /// </summary>
  public RequestTimelineDto Timeline { get; set; } = null!;

  /// <summary>
  /// Request metadata
  /// </summary>
  public Dictionary<string, string> Metadata { get; set; } = new();

  /// <summary>
  /// Indicates if status is terminal
  /// </summary>
  public bool IsTerminal { get; set; }

  /// <summary>
  /// Indicates if request can be cancelled
  /// </summary>
  public bool CanBeCancelled { get; set; }

  /// <summary>
  /// Indicates if request requires applicant action
  /// </summary>
  public bool RequiresApplicantAction { get; set; }

  /// <summary>
  /// Indicates if request requires bank action
  /// </summary>
  public bool RequiresBankAction { get; set; }

  /// <summary>
  /// Days until bank appointment (if scheduled)
  /// </summary>
  public int? DaysUntilBankAppointment { get; set; }

  /// <summary>
  /// Indicates if bank appointment is overdue
  /// </summary>
  public bool IsBankAppointmentOverdue { get; set; }

  /// <summary>
  /// Last modification timestamp
  /// </summary>
  public DateTime? LastModifiedAt { get; set; }

  /// <summary>
  /// Facility details (if requested)
  /// </summary>
  public FacilityDetailsDto? FacilityDetails { get; set; }


}