namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Request timeline data transfer object
/// </summary>
public record RequestTimelineDto
{
  /// <summary>
  /// Request creation date
  /// </summary>
  public DateTime CreatedAt { get; init; }

  /// <summary>
  /// Request approval date (if approved)
  /// </summary>
  public DateTime? ApprovedAt { get; init; }

  /// <summary>
  /// Request rejection date (if rejected)
  /// </summary>
  public DateTime? RejectedAt { get; init; }

  /// <summary>
  /// Bank appointment scheduled date (if scheduled)
  /// </summary>
  public DateTime? BankAppointmentScheduledAt { get; init; }

  /// <summary>
  /// Bank appointment actual date (if completed)
  /// </summary>
  public DateTime? BankAppointmentDate { get; init; }

  /// <summary>
  /// Disbursement date (if disbursed)
  /// </summary>
  public DateTime? DisbursedAt { get; init; }

  /// <summary>
  /// Completion date (if completed)
  /// </summary>
  public DateTime? CompletedAt { get; init; }

  /// <summary>
  /// Days since request creation
  /// </summary>
  public int DaysSinceCreated { get; init; }

  /// <summary>
  /// Days until bank appointment (if scheduled)
  /// </summary>
  public int? DaysUntilBankAppointment { get; init; }

  /// <summary>
  /// Indicates if bank appointment is overdue
  /// </summary>
  public bool IsBankAppointmentOverdue { get; init; }

  /// <summary>
  /// Processing time in days (if completed)
  /// </summary>
  public int? ProcessingTimeDays { get; init; }
}