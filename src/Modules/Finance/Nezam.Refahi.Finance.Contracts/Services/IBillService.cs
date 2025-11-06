using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Services;

/// <summary>
/// Service interface for bill creation and management operations
/// Provides application-level orchestration for bill-related use cases
/// </summary>
public interface IBillService
{
    /// <summary>
    /// Creates a new bill in draft status
    /// </summary>
    /// <param name="request">Bill creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Application result with created bill information</returns>
    Task<ApplicationResult<BillCreationResult>> CreateBillAsync(
        CreateBillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Issues a bill (finalizes it and makes it ready for payment)
    /// </summary>
    /// <param name="billId">ID of the bill to issue</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Application result with issued bill information</returns>
    Task<ApplicationResult<BillIssueResult>> IssueBillAsync(
        Guid billId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates and immediately issues a bill in one operation
    /// </summary>
    /// <param name="request">Bill creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Application result with created and issued bill information</returns>
    Task<ApplicationResult<BillIssueResult>> CreateAndIssueBillAsync(
        CreateBillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a bill and expires associated payments
    /// </summary>
    /// <param name="billId">ID of the bill to cancel</param>
    /// <param name="reason">Reason for cancellation (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Application result with cancellation information</returns>
    Task<ApplicationResult<BillCancellationResult>> CancelBillAsync(
        Guid billId,
        string reason,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Request for creating a bill
/// </summary>
public class CreateBillRequest
{
    /// <summary>
    /// Title of the bill
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Reference tracking code for correlation
    /// </summary>
    public string ReferenceTrackingCode { get; set; } = string.Empty;

    /// <summary>
    /// Reference identifier (e.g., ReservationId, OrderId)
    /// </summary>
    public string ReferenceId { get; set; } = string.Empty;

    /// <summary>
    /// Type of bill (e.g., "TourReservation", "FacilityRequest")
    /// </summary>
    public string BillType { get; set; } = string.Empty;

    /// <summary>
    /// External user ID (from User service)
    /// </summary>

    /// <summary>
    /// Full name of the user (optional)
    /// </summary>
    public string? UserFullName { get; set; }

    /// <summary>
    /// Description or notes for the bill
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Due date for payment (optional)
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Additional metadata for the bill
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// List of items to include in the bill
    /// </summary>
    public List<BillItemRequest>? Items { get; set; }
}

/// <summary>
/// Request for creating a bill item
/// </summary>
public class BillItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPriceRials { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal? DiscountPercentage { get; set; }
}

/// <summary>
/// Result of bill creation
/// </summary>
public class BillCreationResult
{
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public decimal TotalAmountRials { get; set; }
    public string Currency { get; set; } = string.Empty;
}

/// <summary>
/// Result of bill issuing
/// </summary>
public class BillIssueResult
{
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public decimal TotalAmountRials { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal RemainingAmountRials { get; set; }
}

/// <summary>
/// Result of bill cancellation
/// </summary>
public class BillCancellationResult
{
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? CancellationReason { get; set; }
    public DateTime CancelledAt { get; set; }
}

