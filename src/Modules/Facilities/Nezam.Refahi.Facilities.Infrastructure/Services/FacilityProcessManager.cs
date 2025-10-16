using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Domain.Services;

namespace Nezam.Refahi.Facilities.Infrastructure.Services;

public class FacilityProcessManager : IFacilityProcessManager
{
    private readonly IFacilityCycleRepository _cycleRepository;
    private readonly IFacilityRequestRepository _requestRepository;

    public FacilityProcessManager(
        IFacilityCycleRepository cycleRepository,
        IFacilityRequestRepository requestRepository)
    {
        _cycleRepository = cycleRepository;
        _requestRepository = requestRepository;
    }

    public async Task<CycleCloseResult> CloseCycleAsync(
        Guid cycleId,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        var cycle = await _cycleRepository.GetByIdAsync(cycleId, cancellationToken);
        if (cycle == null)
        {
            return new CycleCloseResult
            {
                CycleId = cycleId,
                Success = false,
                FailureReason = $"Cycle with ID {cycleId} not found"
            };
        }

        var applications = await _requestRepository.GetByCycleIdAsync(cycleId, cancellationToken);
        var submittedApplications = applications.Where(a => a.Status == FacilityRequestStatus.PendingApproval).ToList();
        var selectedApplications = new List<Guid>();
        var waitlistedApplications = new List<Guid>();
        var rejectedApplications = new List<Guid>();

        // Simple selection logic based on quota
        var availableQuota = cycle.Quota - cycle.UsedQuota;
        
        foreach (var application in submittedApplications.Take(availableQuota))
        {
            selectedApplications.Add(application.Id);
        }

        foreach (var application in submittedApplications.Skip(availableQuota))
        {
            if (cycle.WaitlistCapacity.HasValue && waitlistedApplications.Count < cycle.WaitlistCapacity.Value)
            {
                waitlistedApplications.Add(application.Id);
            }
            else
            {
                rejectedApplications.Add(application.Id);
            }
        }

        return new CycleCloseResult
        {
            CycleId = cycleId,
            Success = true,
            SelectedApplications = selectedApplications,
            WaitlistedApplications = waitlistedApplications,
            RejectedApplications = rejectedApplications,
            ProcessedAt = DateTime.UtcNow
        };
    }

    public async Task<DispatchBatchResult> CreateDispatchBatchAsync(
        Guid cycleId,
        List<Guid> applicationIds,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        var applications = new List<FacilityRequest>();
        var failedApplications = new List<Guid>();

        foreach (var applicationId in applicationIds)
        {
            var application = await _requestRepository.GetByIdAsync(applicationId, cancellationToken);
            if (application != null && application.Status == FacilityRequestStatus.Approved)
            {
                applications.Add(application);
            }
            else
            {
                failedApplications.Add(applicationId);
            }
        }

        var batchId = Guid.NewGuid();

        return new DispatchBatchResult
        {
            BatchId = batchId,
            Success = true,
            ApplicationsInBatch = applications.Select(a => a.Id).ToList(),
            FailedApplications = failedApplications,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<BankDispatchResult> DispatchToBankAsync(
        Guid batchId,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        // Simulate bank dispatch
        await Task.Delay(100, cancellationToken);

        var bankReference = $"BANK_{batchId:N}";
        var success = Random.Shared.NextDouble() > 0.1; // 90% success rate

        return new BankDispatchResult
        {
            BatchId = batchId,
            Success = success,
            BankReference = success ? bankReference : null,
            DispatchedAt = DateTime.UtcNow,
            FailureReason = success ? null : "Bank service unavailable",
            RetryCount = 0
        };
    }

    public async Task<BankAckResult> ProcessBankAckAsync(
        Guid batchId,
        string bankReference,
        BankAckStatus ackStatus,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        return new BankAckResult
        {
            BatchId = batchId,
            Success = ackStatus == BankAckStatus.Completed,
            AckStatus = ackStatus,
            BankReference = bankReference,
            ReceivedAt = DateTime.UtcNow,
            FailureReason = ackStatus == BankAckStatus.Failed ? "Bank processing failed" : null
        };
    }

    public async Task<DispatchFailureResult> HandleDispatchFailureAsync(
        Guid batchId,
        string failureReason,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var retryCount = 1; // This would be tracked in a real implementation
        var shouldRetry = retryCount < 3;
        var nextRetryAt = shouldRetry ? DateTime.UtcNow.AddMinutes(5) : (DateTime?)null;

        return new DispatchFailureResult
        {
            BatchId = batchId,
            Success = false,
            FailureReason = failureReason,
            FailedAt = DateTime.UtcNow,
            RetryCount = retryCount,
            ShouldRetry = shouldRetry,
            NextRetryAt = nextRetryAt
        };
    }
}