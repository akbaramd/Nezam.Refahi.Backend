using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using System.Diagnostics;
using Nezam.Refahi.Identity.Application.Services.Contracts;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

/// <summary>
/// Implementation of OTP cleanup service for managing challenge data lifecycle
/// Handles big data scenarios and prevents system overload through batching
/// </summary>
public class OtpCleanupService : IOtpCleanupService
{
    private readonly IOtpChallengeRepository _otpChallengeRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ILogger<OtpCleanupService> _logger;

    public OtpCleanupService(
        IOtpChallengeRepository otpChallengeRepository,
        IIdentityUnitOfWork unitOfWork,
        ILogger<OtpCleanupService> logger)
    {
        _otpChallengeRepository = otpChallengeRepository ?? throw new ArgumentNullException(nameof(otpChallengeRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> PerformFullCleanupAsync(int batchSize = 1000, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting full OTP challenge cleanup with batch size {BatchSize}", batchSize);
        
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            var totalCleaned = await _otpChallengeRepository.BatchCleanupConsumedChallengesAsync(batchSize, cancellationToken);
            
            await _unitOfWork.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Full cleanup completed. Cleaned {TotalCleaned} challenges", totalCleaned);
            return totalCleaned;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error during full cleanup operation");
            throw;
        }
    }

    public async Task<int> CleanupOldChallengesAsync(int daysOld = 30, int batchSize = 1000, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting cleanup of challenges older than {DaysOld} days", daysOld);
        
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            var deleted = await _otpChallengeRepository.DeleteOldChallengesAsync(daysOld);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Old challenge cleanup completed. Deleted {Deleted} challenges", deleted);
            return deleted;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error during old challenges cleanup");
            throw;
        }
    }

    public async Task<int> EmergencyCleanupForPhoneAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
    {
        if (phoneNumber == null)
        {
            _logger.LogWarning("Emergency cleanup called with null phone number");
            return 0;
        }

        _logger.LogInformation("Starting emergency cleanup for phone {PhoneNumber}", phoneNumber.Value);
        
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            var deleted = await _otpChallengeRepository.DeleteChallengesForPhoneAsync(phoneNumber, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Emergency cleanup completed for phone {PhoneNumber}. Deleted {Deleted} challenges", 
                phoneNumber.Value, deleted);
            return deleted;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error during emergency cleanup for phone {PhoneNumber}", phoneNumber.Value);
            throw;
        }
    }

    public async Task<CleanupStats> PerformOptimizedMaintenanceAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting optimized maintenance cleanup");
        
        var stats = new CleanupStats();
        
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            // 1. Clean up expired challenges first
            var expiredCount = await _otpChallengeRepository.DeleteExpiredChallengesAsync();
            stats = stats with { ExpiredChallenges = expiredCount };
            
            // 2. Batch cleanup of consumed challenges (big data friendly)
            var consumedCount = await _otpChallengeRepository.BatchCleanupConsumedChallengesAsync(2000, cancellationToken);
            stats = stats with { ConsumedChallenges = consumedCount };
            
            // 3. Clean challenges older than 7 days (keep recent for potential analysis)
            var oldCount = await _otpChallengeRepository.DeleteOldChallengesAsync(7);
            stats = stats with { OldChallenges = oldCount };
            
            var totalCleaned = expiredCount + consumedCount + oldCount;
            stats = stats with { 
                TotalCleaned = totalCleaned,
                Duration = stopwatch.Elapsed
            };
            
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Optimized maintenance completed in {Duration}ms. " +
                                 "Expired: {Expired}, Consumed: {Consumed}, Old: {Old}, Total: {Total}",
                stopwatch.ElapsedMilliseconds,
                stats.ExpiredChallenges,
                stats.ConsumedChallenges, 
                stats.OldChallenges,
                stats.TotalCleaned);
            
            return stats;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error during optimized maintenance");
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}