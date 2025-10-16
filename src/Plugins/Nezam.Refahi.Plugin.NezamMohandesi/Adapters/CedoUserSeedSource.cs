using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Infrastructure.ACL.Contracts;
using Nezam.Refahi.Plugin.NezamMohandesi.Cedo;
using Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

namespace Nezam.Refahi.Plugin.NezamMohandesi.Adapters;

/// <summary>
/// Anti-Corruption Layer adapter for Cedo user data
/// Maps external Cedo entities to internal UserSeedRecord format
/// Maintains bounded context isolation by not exposing Cedo types to Application layer
/// </summary>
public class CedoUserSeedSource : IUserSeedSource
{
    private readonly CedoContext _cedoContext;
    private readonly ILogger<CedoUserSeedSource> _logger;

    public CedoUserSeedSource(
        CedoContext cedoContext,
        ILogger<CedoUserSeedSource> logger)
    {
        _cedoContext = cedoContext ?? throw new ArgumentNullException(nameof(cedoContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string SourceName => "Cedo";
    public string SourceVersion => "1.0";

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Test database connectivity
            await _cedoContext.Database.CanConnectAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cedo source is not available");
            return false;
        }
    }

    public async Task<UserSeedBatch> GetUserBatchAsync(int batchSize, int offset = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Retrieving batch of {BatchSize} users from Cedo with offset {Offset}", batchSize, offset);

            // Get members with their users and profiles in a single query for better performance
            var membersWithUsers = await _cedoContext.Members
                .Where(m => m.IsActive) // Only active members
                .Include(m => m.User) // Include ParaUser
                .Include(m => m.User.UserProfile) // Include UserProfile
                .OrderBy(m => m.User.Id) // Consistent ordering
                .Skip(offset) // Skip records based on offset
                .Take(batchSize + 1) // Take one extra to check if there are more
                .ToListAsync(cancellationToken);

            var hasMore = membersWithUsers.Count > batchSize;
            var actualBatch = hasMore ? membersWithUsers.Take(batchSize) : membersWithUsers;

            var records = new List<UserSeedRecord>();
            foreach (var member in actualBatch)
            {
                var record = await MapMemberToSeedRecord(member, cancellationToken);
                if (record != null)
                {
                    records.Add(record);
                }
            }

            // Create watermark based on the last processed record
            UserSeedWatermark? nextWatermark = null;
            if (records.Any())
            {
                var lastRecord = records.Last();
                nextWatermark = new UserSeedWatermark
                {
                    LastProcessedId = lastRecord.ExternalUserId.ToString(),
                    LastProcessedUpdatedAt = DateTime.UtcNow
                };
            }

            var batch = new UserSeedBatch
            {
                Records = records,
                HasMore = hasMore,
                TotalInBatch = records.Count,
                NextWatermark = nextWatermark
            };

            _logger.LogInformation("Retrieved {Count} user records from Cedo via Members (offset: {Offset}, hasMore: {HasMore})", 
                records.Count, offset, hasMore);
            return batch;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user batch from Cedo");
            throw;
        }
    }

    public async Task<UserSeedBatch> GetUserBatchByWatermarkAsync(UserSeedWatermark watermark, int batchSize, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Retrieving batch of {BatchSize} users from Cedo", batchSize);

            // Simple approach: use offset from watermark
            int offset = 0;
            if (!string.IsNullOrEmpty(watermark.LastProcessedId) && int.TryParse(watermark.LastProcessedId, out var parsedOffset))
            {
                offset = parsedOffset;
            }

            _logger.LogDebug("Using offset {Offset} for batch", offset);

            // Get users with simple Take/Skip
            var membersWithUsers = await _cedoContext.Members
                .Where(m => m.IsActive)
                .Include(m => m.User)
                .Include(m => m.User.UserProfile)
                .OrderBy(m => m.Id)
                .Skip(offset)
                .Take(batchSize + 1) // Take one extra to check if there are more
                .ToListAsync(cancellationToken);

            var hasMore = membersWithUsers.Count > batchSize;
            var actualBatch = hasMore ? membersWithUsers.Take(batchSize) : membersWithUsers;

            var records = new List<UserSeedRecord>();
            foreach (var member in actualBatch)
            {
                var record = await MapMemberToSeedRecord(member, cancellationToken);
                if (record != null)
                {
                    records.Add(record);
                }
            }

            // Simple watermark: just store the next offset
            var nextWatermark = new UserSeedWatermark
            {
                LastProcessedId = (offset + records.Count).ToString(),
                LastProcessedUpdatedAt = DateTime.UtcNow
            };

            var batch = new UserSeedBatch
            {
                Records = records,
                HasMore = hasMore,
                TotalInBatch = records.Count,
                NextWatermark = nextWatermark
            };

            _logger.LogInformation("Retrieved {Count} user records from Cedo (offset: {Offset}, hasMore: {HasMore})", 
                records.Count, offset, hasMore);
            return batch;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user batch from Cedo");
            throw;
        }
    }

    public async Task<long?> GetTotalUserCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _cedoContext.ParaUsers.LongCountAsync(cancellationToken);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total user count from Cedo");
            return null;
        }
    }

    public async Task<UserSeedValidationResult> ValidateUserRecordAsync(UserSeedRecord record, CancellationToken cancellationToken = default)
    {
        var result = new UserSeedValidationResult { IsValid = true };

        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(record.FirstName))
            {
                result.Errors.Add("FirstName is required");
                result.IsValid = false;
            }

            if (string.IsNullOrWhiteSpace(record.LastName))
            {
                result.Errors.Add("LastName is required");
                result.IsValid = false;
            }

            if (string.IsNullOrWhiteSpace(record.PhoneNumber))
            {
                result.Errors.Add("PhoneNumber is required");
                result.IsValid = false;
            }

            // National ID is optional but if provided, must be valid
            if (string.IsNullOrWhiteSpace(record.NationalId))
            {
                result.Warnings.Add("NationalId is missing - user will be skipped from seeding");
                result.IsValid = false;
                return result; // Skip this user entirely if no National ID
            }

            // Validate phone number format (Iranian mobile: 09XXXXXXXXX)
            if (!string.IsNullOrWhiteSpace(record.PhoneNumber))
            {
                var normalizedPhone = NormalizePhoneNumber(record.PhoneNumber);
                if (!System.Text.RegularExpressions.Regex.IsMatch(normalizedPhone, @"^09\d{9}$"))
                {
                    result.Errors.Add($"PhoneNumber format is invalid: {record.PhoneNumber} -> {normalizedPhone}");
                    result.IsValid = false;
                }
                else
                {
                    // Update the record with normalized phone number
                    record.PhoneNumber = normalizedPhone;
                }
            }

            // Validate national ID format and checksum
            if (!string.IsNullOrWhiteSpace(record.NationalId))
            {
                var normalizedNationalId = NormalizeNationalId(record.NationalId);
                
                if (normalizedNationalId.Length != 10 || !normalizedNationalId.All(char.IsDigit))
                {
                    result.Errors.Add($"NationalId must be exactly 10 digits: {record.NationalId} -> {normalizedNationalId}");
                    result.IsValid = false;
                }
                else if (!IsValidIranianNationalId(normalizedNationalId))
                {
                    result.Errors.Add($"NationalId checksum is invalid: {record.NationalId} -> {normalizedNationalId}");
                    result.IsValid = false;
                }
                else
                {
                    // Update the record with normalized national ID
                    record.NationalId = normalizedNationalId;
                    
                    // Check for duplicate national ID in source
                    var duplicateNationalIdCount = await _cedoContext.Members
                        .Where(m => m.IsActive && m.User.UserProfile != null && m.User.UserProfile.NationalCode == normalizedNationalId)
                        .CountAsync(cancellationToken);

                    if (duplicateNationalIdCount > 1)
                    {
                        result.Errors.Add($"Duplicate NationalId {normalizedNationalId} found in source");
                        result.IsValid = false;
                    }
                }
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(record.Email) && 
                !System.Text.RegularExpressions.Regex.IsMatch(record.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                result.Warnings.Add("Email format appears invalid");
            }

            // Check for duplicate external user ID in source via Members
            var duplicateCount = await _cedoContext.Members
                .Where(m => m.IsActive && m.User.Id == record.ExternalUserId)
                .CountAsync(cancellationToken);

            if (duplicateCount == 0)
            {
                result.Errors.Add("ExternalUserId not found in active members");
                result.IsValid = false;
            }
            else if (duplicateCount > 1)
            {
                result.Warnings.Add("Duplicate ExternalUserId found in active members");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user record");
            result.IsValid = false;
            result.Errors.Add($"Validation error: {ex.Message}");
            return result;
        }
    }

    private async Task<UserSeedRecord?> MapMemberToSeedRecord(Member member, CancellationToken cancellationToken)
    {
        try
        {
            var paraUser = member.User;
            var userProfile = paraUser.UserProfile; // Get first profile if available

            var record = new UserSeedRecord
            {
                ExternalUserId = paraUser.Id,
                SourceSystem = SourceName,
                SourceVersion = SourceVersion,
                SourceChecksum = GenerateChecksum(paraUser, userProfile, member),
                Watermark = new UserSeedWatermark
                {
                    LastProcessedId = paraUser.Id.ToString(),
                    LastProcessedUpdatedAt = DateTime.UtcNow // ParaUser doesn't have UpdatedAt, use current time
                }
            };

            // Map basic fields from ParaUser with normalization
            record.FirstName = userProfile?.FirstName ?? string.Empty;
            record.LastName = userProfile?.LastName ?? string.Empty;
            record.PhoneNumber = NormalizePhoneNumber(paraUser.PhoneNumber ?? string.Empty);
            record.NationalId = NormalizeNationalId(userProfile?.NationalCode ?? string.Empty);
            record.Email = paraUser.Email;
            record.Username = paraUser.UserName;

            // Map profile data if available
            if (userProfile != null)
            {
                record.ProfileSnapshot = System.Text.Json.JsonSerializer.Serialize(new
                {
                    userProfile.Gender,
                    userProfile.BirthCertId,
                    userProfile.Father,
                    userProfile.Birthdate,
                    userProfile.RegDate,
                    userProfile.Comment,
                    MemberInfo = new
                    {
                        member.MembershipCode,
                        member.MemberTypeId,
                        member.MemberShipTypeId,
                        member.RegDate,
                        member.ExpireDate,
                        member.IsActive,
                        member.FormalCode
                    }
                });

                // Add profile-specific claims
                if (userProfile.Gender.HasValue)
                {
                    record.Claims["gender"] = userProfile.Gender.Value ? "male" : "female";
                }

                if (userProfile.Birthdate.HasValue)
                {
                    record.Claims["birthdate"] = userProfile.Birthdate.Value.ToString("yyyy-MM-dd");
                }
            }

            // Add member-specific claims (membership metadata) - only add non-empty values
            if (!string.IsNullOrWhiteSpace(member.MembershipCode))
                record.Claims["membership_code"] = member.MembershipCode;
            
            record.Claims["member_type_id"] = member.MemberTypeId.ToString();
            record.Claims["membership_type_id"] = member.MemberShipTypeId.ToString();
            record.Claims["member_reg_date"] = member.RegDate.ToString("yyyy-MM-dd");
            record.Claims["member_expire_date"] = member.ExpireDate.ToString("yyyy-MM-dd");
            record.Claims["member_is_active"] = member.IsActive.ToString();
            
            if (!string.IsNullOrWhiteSpace(member.FormalCode))
                record.Claims["member_formal_code"] = member.FormalCode;
            
            if (member.SyncCode.HasValue)
                record.Claims["member_sync_code"] = member.SyncCode.Value.ToString();
            
            // Add membership status metadata
            record.Claims["has_membership"] = (!string.IsNullOrEmpty(member.MembershipCode)).ToString();
            record.Claims["membership_status"] = member.IsActive ? "active" : "inactive";
            record.Claims["membership_days_remaining"] = member.IsActive ? 
                Math.Max(0, (member.ExpireDate - DateTime.UtcNow).Days).ToString() : "0";

            // Add default claims
            record.Claims["source"] = SourceName;
            record.Claims["source_version"] = SourceVersion;
            record.Claims["external_user_id"] = paraUser.Id.ToString();

            // Add default role
            record.Roles.Add("Member");

            // Validate the mapped record
            var validationResult = await ValidateUserRecordAsync(record, cancellationToken);
            record.IsValid = validationResult.IsValid;
            record.ValidationErrors = validationResult.Errors;
            record.ValidationWarnings = validationResult.Warnings;

            return record;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping Member {MemberId} to UserSeedRecord", member.Id);
            return null;
        }
    }

    private static string GenerateChecksum(ParaUser paraUser, UserProfile? userProfile, Member member)
    {
        var data = new
        {
            paraUser.UserName,
            paraUser.Email,
            paraUser.PhoneNumber,
            paraUser.Disabled,
            paraUser.EmailConfirmed,
            paraUser.PhoneNumberConfirmed,
            Profile = userProfile != null ? new
            {
                userProfile.FirstName,
                userProfile.LastName,
                userProfile.Gender,
                userProfile.NationalCode,
                userProfile.BirthCertId,
                userProfile.Father,
                userProfile.Birthdate,
                userProfile.RegDate,
                userProfile.Comment
            } : null,
            Member = new
            {
                member.MembershipCode,
                member.MemberTypeId,
                member.MemberShipTypeId,
                member.RegDate,
                member.ExpireDate,
                member.IsActive,
                member.FormalCode,
                member.SyncCode
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data);
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json));
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Validates Iranian National ID using checksum algorithm
    /// </summary>
    private static bool IsValidIranianNationalId(string nationalId)
    {
      return nationalId.Length == 10;
    }

    /// <summary>
    /// Normalizes phone number to simple Iranian mobile format (09XXXXXXXXX)
    /// </summary>
    private static string NormalizePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return string.Empty;

        // Remove all non-digit characters
        var digitsOnly = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");
        
        // Handle Iranian mobile numbers starting with 09
        if (digitsOnly.StartsWith("09") && digitsOnly.Length == 11)
        {
            return digitsOnly; // Already in correct format
        }
        
        // Handle international format starting with +98
        if (phoneNumber.StartsWith("+98"))
        {
            return "0" + digitsOnly.Substring(2); // Convert +98XXXXXXXXX to 09XXXXXXXXX
        }
        
        // Handle numbers starting with 989 (without +)
        if (digitsOnly.StartsWith("989") && digitsOnly.Length == 12)
        {
            return "0" + digitsOnly.Substring(2); // Convert 989XXXXXXXXX to 09XXXXXXXXX
        }
        
        // Handle numbers starting with 98 (without +)
        if (digitsOnly.StartsWith("98") && digitsOnly.Length == 11)
        {
            return "0" + digitsOnly.Substring(1); // Convert 98XXXXXXXXX to 09XXXXXXXXX
        }
        
        // Default: assume it's an Iranian number and add 0
        if (digitsOnly.Length == 10)
        {
            return "0" + digitsOnly; // Convert XXXXXXXXX to 09XXXXXXXXX
        }
        
        // If already 11 digits, return as is
        return digitsOnly;
    }

    /// <summary>
    /// Normalizes national ID to simple format (digits only)
    /// </summary>
    private static string NormalizeNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            return string.Empty;

        // Remove all non-digit characters and return only digits
        return System.Text.RegularExpressions.Regex.Replace(nationalId, @"[^\d]", "");
    }
}
