using MCA.SharedKernel.Domain.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Enums;
using Nezam.Refahi.Identity.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Domain.Repositories;

/// <summary>
/// Repository interface for OTP challenge operations
/// </summary>
public interface IOtpChallengeRepository : IRepository<OtpChallenge, Guid>
{
    /// <summary>
    /// Gets an OTP challenge by its challenge ID
    /// </summary>
    /// <param name="challengeId">The challenge ID</param>
    /// <returns>The challenge if found, null otherwise</returns>
    Task<OtpChallenge?> GetByChallengeIdAsync(string challengeId);
    
    /// <summary>
    /// Gets active OTP challenges for a phone number
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <returns>Collection of active challenges</returns>
    Task<IEnumerable<OtpChallenge>> GetActiveChallengesByPhoneAsync(PhoneNumber phoneNumber);
    
    /// <summary>
    /// Gets OTP challenges by phone number and status
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <param name="status">The challenge status</param>
    /// <returns>Collection of challenges</returns>
    Task<IEnumerable<OtpChallenge>> GetByPhoneAndStatusAsync(PhoneNumber phoneNumber, ChallengeStatus status);
    
    /// <summary>
    /// Gets OTP challenges by phone number within a time range
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Collection of challenges</returns>
    Task<IEnumerable<OtpChallenge>> GetByPhoneAndDateRangeAsync(PhoneNumber phoneNumber, DateTime fromDate, DateTime toDate);
    
    /// <summary>
    /// Gets OTP challenges by IP address within a time range
    /// </summary>
    /// <param name="ipAddress">The IP address</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Collection of challenges</returns>
    Task<IEnumerable<OtpChallenge>> GetByIpAddressAndDateRangeAsync(string ipAddress, DateTime fromDate, DateTime toDate);
    
    /// <summary>
    /// Gets OTP challenges by device fingerprint within a time range
    /// </summary>
    /// <param name="deviceFingerprint">The device fingerprint</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Collection of challenges</returns>
    Task<IEnumerable<OtpChallenge>> GetByDeviceFingerprintAndDateRangeAsync(string deviceFingerprint, DateTime fromDate, DateTime toDate);
    
    /// <summary>
    /// Gets expired OTP challenges
    /// </summary>
    /// <returns>Collection of expired challenges</returns>
    Task<IEnumerable<OtpChallenge>> GetExpiredChallengesAsync();
    
    /// <summary>
    /// Gets locked OTP challenges
    /// </summary>
    /// <returns>Collection of locked challenges</returns>
    Task<IEnumerable<OtpChallenge>> GetLockedChallengesAsync();
    
    /// <summary>
    /// Counts active challenges for a phone number within the last hour
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <returns>Count of active challenges</returns>
    Task<int> CountActiveChallengesByPhoneInLastHourAsync(PhoneNumber phoneNumber);
    
    /// <summary>
    /// Counts active challenges for an IP address within the last hour
    /// </summary>
    /// <param name="ipAddress">The IP address</param>
    /// <returns>Count of active challenges</returns>
    Task<int> CountActiveChallengesByIpInLastHourAsync(string ipAddress);
    
    /// <summary>
    /// Counts active challenges for a device fingerprint within the last hour
    /// </summary>
    /// <param name="deviceFingerprint">The device fingerprint</param>
    /// <returns>Count of active challenges</returns>
    Task<int> CountActiveChallengesByDeviceInLastHourAsync(string deviceFingerprint);
    
    /// <summary>
    /// Deletes expired OTP challenges
    /// </summary>
    /// <returns>Number of deleted challenges</returns>
    Task<int> DeleteExpiredChallengesAsync();
    
    /// <summary>
    /// Deletes old OTP challenges (older than specified days)
    /// </summary>
    /// <param name="daysOld">Minimum age in days</param>
    /// <returns>Number of deleted challenges</returns>
    Task<int> DeleteOldChallengesAsync(int daysOld);
}
