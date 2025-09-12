using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Enums;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of OTP challenge repository using EF Core
/// </summary>
public class OtpChallengeRepository : EfRepository<IdentityDbContext, OtpChallenge, Guid>, IOtpChallengeRepository
{
    public OtpChallengeRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }



    public async Task<IEnumerable<OtpChallenge>> GetActiveChallengesByPhoneAsync(PhoneNumber phoneNumber)
    {
        if (phoneNumber == null)
            return Enumerable.Empty<OtpChallenge>();

        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        
        return await _dbContext.OtpChallenges
            .Where(c => c.PhoneNumber.Value == phoneNumber.Value && 
                       c.CreatedAt >= oneHourAgo &&
                       (c.Status == ChallengeStatus.Created || c.Status == ChallengeStatus.Sent))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OtpChallenge>> GetByPhoneAndStatusAsync(PhoneNumber phoneNumber, ChallengeStatus status)
    {
        if (phoneNumber == null)
            return Enumerable.Empty<OtpChallenge>();

        return await _dbContext.OtpChallenges
            .Where(c => c.PhoneNumber.Value == phoneNumber.Value && c.Status == status)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OtpChallenge>> GetByPhoneAndDateRangeAsync(PhoneNumber phoneNumber, DateTime fromDate, DateTime toDate)
    {
        if (phoneNumber == null)
            return Enumerable.Empty<OtpChallenge>();

        return await _dbContext.OtpChallenges
            .Where(c => c.PhoneNumber.Value == phoneNumber.Value && 
                       c.CreatedAt >= fromDate && c.CreatedAt <= toDate)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OtpChallenge>> GetByIpAddressAndDateRangeAsync(string ipAddress, DateTime fromDate, DateTime toDate)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return Enumerable.Empty<OtpChallenge>();

        return await _dbContext.OtpChallenges
            .Where(c => c.IpAddress != null && 
                       c.IpAddress.Value == ipAddress &&
                       c.CreatedAt >= fromDate && c.CreatedAt <= toDate)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OtpChallenge>> GetByDeviceFingerprintAndDateRangeAsync(string deviceFingerprint, DateTime fromDate, DateTime toDate)
    {
        if (string.IsNullOrWhiteSpace(deviceFingerprint))
            return Enumerable.Empty<OtpChallenge>();

        return await _dbContext.OtpChallenges
            .Where(c => c.DeviceFingerprint != null && 
                       c.DeviceFingerprint.Value == deviceFingerprint &&
                       c.CreatedAt >= fromDate && c.CreatedAt <= toDate)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OtpChallenge>> GetExpiredChallengesAsync()
    {
        return await _dbContext.OtpChallenges
            .Where(c => c.Status == ChallengeStatus.Expired || 
                       (c.ExpiresAt < DateTime.UtcNow && 
                        c.Status != ChallengeStatus.Verified && 
                        c.Status != ChallengeStatus.Consumed))
            .OrderBy(c => c.ExpiresAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OtpChallenge>> GetLockedChallengesAsync()
    {
        return await _dbContext.OtpChallenges
            .Where(c => c.Status == ChallengeStatus.Locked)
            .OrderByDescending(c => c.LockedAt)
            .ToListAsync();
    }

    public async Task<int> CountActiveChallengesByPhoneInLastHourAsync(PhoneNumber phoneNumber)
    {
        if (phoneNumber == null)
            return 0;

        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        
        return await _dbContext.OtpChallenges
            .CountAsync(c => c.PhoneNumber.Value == phoneNumber.Value && 
                            c.CreatedAt >= oneHourAgo &&
                            (c.Status == ChallengeStatus.Created || c.Status == ChallengeStatus.Sent));
    }

    public async Task<int> CountActiveChallengesByIpInLastHourAsync(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return 0;

        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        
        return await _dbContext.OtpChallenges
            .CountAsync(c => c.IpAddress != null && 
                            c.IpAddress.Value == ipAddress &&
                            c.CreatedAt >= oneHourAgo &&
                            (c.Status == ChallengeStatus.Created || c.Status == ChallengeStatus.Sent));
    }

    public async Task<int> CountActiveChallengesByDeviceInLastHourAsync(string deviceFingerprint)
    {
        if (string.IsNullOrWhiteSpace(deviceFingerprint))
            return 0;

        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        
        return await _dbContext.OtpChallenges
            .CountAsync(c => c.DeviceFingerprint != null && 
                            c.DeviceFingerprint.Value == deviceFingerprint &&
                            c.CreatedAt >= oneHourAgo &&
                            (c.Status == ChallengeStatus.Created || c.Status == ChallengeStatus.Sent));
    }

    public async Task<int> DeleteExpiredChallengesAsync()
    {
        var expiredChallenges = await GetExpiredChallengesAsync();
        var count = expiredChallenges.Count();
        
        if (count > 0)
        {
            _dbContext.OtpChallenges.RemoveRange(expiredChallenges);
        }

        await _dbContext.SaveChangesAsync();
        return count;
    }

    public async Task<int> DeleteOldChallengesAsync(int daysOld)
    {
        if (daysOld <= 0)
            return 0;

        var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
        
        var count = await _dbContext.OtpChallenges
            .Where(c => c.CreatedAt < cutoffDate)
            .ExecuteDeleteAsync();
        
    
        
        return count;
    }

    public async Task<int> DeleteChallengesForPhoneAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
    {
        if (phoneNumber == null)
            return 0;

        var count = await _dbContext.OtpChallenges
            .Where(c => c.PhoneNumber.Value == phoneNumber.Value)
            .ExecuteDeleteAsync(cancellationToken);
        
      
        return count;
    }

    public async Task<int> BatchDeleteChallengesByIdsAsync(List<Guid> challengeIds, CancellationToken cancellationToken = default)
    {
        if (!challengeIds?.Any() == true)
            return 0;

        var deletedCount = await _dbContext.OtpChallenges
            .Where(c => challengeIds != null && challengeIds.Contains(c.Id))
            .ExecuteDeleteAsync(cancellationToken);
            
        return deletedCount;
    }

    public async Task<int> BatchCleanupConsumedChallengesAsync(int batchSize = 1000, CancellationToken cancellationToken = default)
    {
        if (batchSize <= 0)
            batchSize = 1000;

        var totalDeleted = 0;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var batch = await _dbContext.OtpChallenges
                .Where(c => c.Status == ChallengeStatus.Consumed || 
                           c.Status == ChallengeStatus.Verified ||
                           (c.Status == ChallengeStatus.Expired && c.CreatedAt < DateTime.UtcNow.AddDays(-7)))
                .OrderBy(c => c.CreatedAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);
                
            if (!batch.Any())
                break;
                
            _dbContext.OtpChallenges.RemoveRange(batch);
            totalDeleted += batch.Count;
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            if (batch.Count < batchSize)
                break;
        }
        
        return totalDeleted;
    }
}
