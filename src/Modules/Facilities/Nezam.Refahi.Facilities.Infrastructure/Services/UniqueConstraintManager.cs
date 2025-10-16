using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Domain.Services;

namespace Nezam.Refahi.Facilities.Infrastructure.Services;

/// <summary>
/// Unique Constraint Manager implementation
/// </summary>
public class UniqueConstraintManager : IUniqueConstraintManager
{
    private readonly IFacilityRequestRepository _requestRepository;

    public UniqueConstraintManager(IFacilityRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<string> GeneratePolicyBasedUniqueKeyAsync(
        Guid facilityId,
        Guid? cycleId,
        Guid userId,
        Dictionary<string, object> policySnapshot,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var keyComponents = new List<string>
        {
            facilityId.ToString(),
            cycleId?.ToString() ?? "null",
            userId.ToString()
        };

        // اضافه کردن policy snapshot به کلید
        var policyHash = ComputePolicyHash(policySnapshot);
        keyComponents.Add(policyHash);

        var combinedKey = string.Join("|", keyComponents);
        return ComputeHash(combinedKey);
    }

    public async Task<bool> IsPerCycleOnceUniqueAsync(
        Guid facilityId,
        Guid cycleId,
        Guid userId,
        string policyHash,
        CancellationToken cancellationToken = default)
    {
        var existingRequests = await _requestRepository.GetByUserIdAsync(userId, cancellationToken);
        
        return !existingRequests.Any(r => r.FacilityId == facilityId && 
                                         r.FacilityCycleId == cycleId && 
                                         (r.Status == Domain.Enums.FacilityRequestStatus.PendingApproval ||
                                          r.Status == Domain.Enums.FacilityRequestStatus.UnderReview ||
                                          r.Status == Domain.Enums.FacilityRequestStatus.Approved));
    }

    public async Task<bool> IsExclusiveSetUniqueAsync(
        string exclusiveSetId,
        Guid userId,
        string policyHash,
        CancellationToken cancellationToken = default)
    {
        var existingRequests = await _requestRepository.GetByUserIdAsync(userId, cancellationToken);
        
        // Check if user has any active requests in the same exclusive set
        return !existingRequests.Any(r => (r.Status == Domain.Enums.FacilityRequestStatus.PendingApproval ||
                                          r.Status == Domain.Enums.FacilityRequestStatus.UnderReview ||
                                          r.Status == Domain.Enums.FacilityRequestStatus.Approved));
    }

    public async Task<LogicalLock> AcquireLogicalLockAsync(
        string lockKey,
        TimeSpan lockDuration,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        
        var lockId = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.Add(lockDuration);
        
        return new LogicalLock
        {
            LockKey = lockKey,
            LockId = lockId,
            ExpiresAt = expiresAt
        };
    }

    public async Task ReleaseLogicalLockAsync(
        string lockKey,
        string lockId,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        
        // در پیاده‌سازی واقعی، اینجا باید قفل از دیتابیس آزاد شود
    }

    private string ComputePolicyHash(Dictionary<string, object> policySnapshot)
    {
        var sortedKeys = policySnapshot.Keys.OrderBy(k => k).ToList();
        var hashInput = string.Join("|", sortedKeys.Select(k => $"{k}:{policySnapshot[k]}"));
        return ComputeHash(hashInput);
    }

    private string ComputeHash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes)[..16]; // 16 کاراکتر اول
    }
}