using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Events;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// Process Manager برای مدیریت پایان‌دوره و اعزام به بانک
/// </summary>
public interface IFacilityProcessManager
{
    /// <summary>
    /// بستن دوره و انتخاب درخواست‌ها
    /// </summary>
    Task<CycleCloseResult> CloseCycleAsync(
        Guid cycleId,
        string correlationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ایجاد batch برای اعزام به بانک
    /// </summary>
    Task<DispatchBatchResult> CreateDispatchBatchAsync(
        Guid cycleId,
        List<Guid> applicationIds,
        string correlationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// اعزام batch به بانک
    /// </summary>
    Task<BankDispatchResult> DispatchToBankAsync(
        Guid batchId,
        string correlationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت تأیید از بانک
    /// </summary>
    Task<BankAckResult> ProcessBankAckAsync(
        Guid batchId,
        string bankReference,
        BankAckStatus ackStatus,
        string correlationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// پردازش شکست اعزام
    /// </summary>
    Task<DispatchFailureResult> HandleDispatchFailureAsync(
        Guid batchId,
        string failureReason,
        string correlationId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// نتیجه بستن دوره
/// </summary>
public class CycleCloseResult
{
    public Guid CycleId { get; set; }
    public bool Success { get; set; }
    public List<Guid> SelectedApplications { get; set; } = new();
    public List<Guid> WaitlistedApplications { get; set; } = new();
    public List<Guid> RejectedApplications { get; set; } = new();
    public string? FailureReason { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// نتیجه ایجاد batch
/// </summary>
public class DispatchBatchResult
{
    public Guid BatchId { get; set; }
    public bool Success { get; set; }
    public List<Guid> ApplicationsInBatch { get; set; } = new();
    public List<Guid> FailedApplications { get; set; } = new();
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// نتیجه اعزام به بانک
/// </summary>
public class BankDispatchResult
{
    public Guid BatchId { get; set; }
    public bool Success { get; set; }
    public string? BankReference { get; set; }
    public DateTime DispatchedAt { get; set; } = DateTime.UtcNow;
    public string? FailureReason { get; set; }
    public int RetryCount { get; set; }
}

/// <summary>
/// نتیجه تأیید بانک
/// </summary>
public class BankAckResult
{
    public Guid BatchId { get; set; }
    public bool Success { get; set; }
    public BankAckStatus AckStatus { get; set; }
    public string? BankReference { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public string? FailureReason { get; set; }
}

/// <summary>
/// نتیجه شکست اعزام
/// </summary>
public class DispatchFailureResult
{
    public Guid BatchId { get; set; }
    public bool Success { get; set; }
    public string FailureReason { get; set; } = null!;
    public DateTime FailedAt { get; set; } = DateTime.UtcNow;
    public int RetryCount { get; set; }
    public bool ShouldRetry { get; set; }
    public DateTime? NextRetryAt { get; set; }
}

/// <summary>
/// وضعیت تأیید بانک
/// </summary>
public enum BankAckStatus
{
    Received = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Timeout = 5
}
