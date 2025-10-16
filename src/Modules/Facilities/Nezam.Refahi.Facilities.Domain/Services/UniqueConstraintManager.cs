using System.Security.Cryptography;
using System.Text;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// مدیریت قیود یکتای پویا
/// </summary>
public interface IUniqueConstraintManager
{
    /// <summary>
    /// ایجاد کلید یکتای سیاست‌محور
    /// </summary>
    Task<string> GeneratePolicyBasedUniqueKeyAsync(
        Guid facilityId,
        Guid? cycleId,
        Guid userId,
        Dictionary<string, object> policySnapshot,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی یکتایی PerCycleOnce
    /// </summary>
    Task<bool> IsPerCycleOnceUniqueAsync(
        Guid facilityId,
        Guid cycleId,
        Guid userId,
        string policyHash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی یکتایی ExclusiveSet
    /// </summary>
    Task<bool> IsExclusiveSetUniqueAsync(
        string exclusiveSetId,
        Guid userId,
        string policyHash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ایجاد قفل منطقی سبک
    /// </summary>
    Task<LogicalLock> AcquireLogicalLockAsync(
        string lockKey,
        TimeSpan lockDuration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// آزاد کردن قفل منطقی
    /// </summary>
    Task ReleaseLogicalLockAsync(
        string lockKey,
        string lockId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// قفل منطقی سبک
/// </summary>
public class LogicalLock
{
    public string LockKey { get; set; } = null!;
    public string LockId { get; set; } = null!;
    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsValid => DateTime.UtcNow < ExpiresAt;
}

/// <summary>
/// پیاده‌سازی ساده Unique Constraint Manager
/// </summary>
public class UniqueConstraintManager : IUniqueConstraintManager
{
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
        await Task.CompletedTask;
        
        // در پیاده‌سازی واقعی، اینجا باید با دیتابیس چک شود
        // که آیا کاربر قبلاً در این دوره با این policy درخواست داده یا نه
        return true; // Placeholder
    }

    public async Task<bool> IsExclusiveSetUniqueAsync(
        string exclusiveSetId,
        Guid userId,
        string policyHash,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        
        // در پیاده‌سازی واقعی، اینجا باید با دیتابیس چک شود
        // که آیا کاربر در مجموعه انحصاری دیگری درخواست فعال دارد یا نه
        return true; // Placeholder
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
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes)[..16]; // 16 کاراکتر اول
    }
}
