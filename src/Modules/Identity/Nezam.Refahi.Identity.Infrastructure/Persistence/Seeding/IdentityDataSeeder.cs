using Microsoft.Extensions.Logging;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Seeding;

/// <summary>
/// Main seeder service for Identity module data
/// </summary>
public class IdentityDataSeeder
{
    private readonly RoleSeeder _roleSeeder;
    private readonly UserSeeder _userSeeder;
    private readonly ILogger<IdentityDataSeeder> _logger;

    public IdentityDataSeeder(
        RoleSeeder roleSeeder,
        UserSeeder userSeeder,
        ILogger<IdentityDataSeeder> logger)
    {
        _roleSeeder = roleSeeder;
        _userSeeder = userSeeder;
        _logger = logger;
    }

    /// <summary>
    /// Seeds all identity data (roles and users)
    /// </summary>
    public async Task SeedAllDataAsync()
    {
        _logger.LogInformation("Starting Identity module data seeding...");

        try
        {
            // Seed roles first (users depend on roles)
            await _roleSeeder.SeedDefaultRolesAsync();
            
            // Then seed admin users
            await _userSeeder.SeedDefaultAdminUsersAsync();

            _logger.LogInformation("Identity module data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed Identity module data");
            throw;
        }
    }

    /// <summary>
    /// Seeds only roles (useful for testing or when users already exist)
    /// </summary>
    public async Task SeedRolesOnlyAsync()
    {
        _logger.LogInformation("Starting Identity roles seeding...");

        try
        {
            await _roleSeeder.SeedDefaultRolesAsync();
            _logger.LogInformation("Identity roles seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed Identity roles");
            throw;
        }
    }

    /// <summary>
    /// Seeds only admin users (assumes roles already exist)
    /// </summary>
    public async Task SeedUsersOnlyAsync()
    {
        _logger.LogInformation("Starting Identity admin users seeding...");

        try
        {
            await _userSeeder.SeedDefaultAdminUsersAsync();
            _logger.LogInformation("Identity admin users seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed Identity admin users");
            throw;
        }
    }

    /// <summary>
    /// Validates that all seeded data exists and is properly configured
    /// </summary>
    public async Task<IdentitySeedingValidationResult> ValidateSeedingAsync()
    {
        _logger.LogInformation("Validating Identity module seeding...");

        var result = new IdentitySeedingValidationResult();

        try
        {
            // Validate roles
            result.RolesValid = await _roleSeeder.ValidateDefaultRolesAsync();
            result.RoleCount = await _roleSeeder.GetSeededRoleCountAsync();

            // Validate users
            result.UsersValid = await _userSeeder.ValidateAdminUsersAsync();
            result.AdminUserCount = await _userSeeder.GetAdminUserCountAsync();
            result.VerifiedUserCount = await _userSeeder.GetVerifiedUserCountAsync();

            result.IsValid = result.RolesValid && result.UsersValid;

            if (result.IsValid)
            {
                _logger.LogInformation("Identity module seeding validation passed. Roles: {RoleCount}, Admin Users: {AdminUserCount}, Verified Users: {VerifiedUserCount}", 
                    result.RoleCount, result.AdminUserCount, result.VerifiedUserCount);
            }
            else
            {
                _logger.LogWarning("Identity module seeding validation failed. Roles Valid: {RolesValid}, Users Valid: {UsersValid}", 
                    result.RolesValid, result.UsersValid);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate Identity module seeding");
            result.IsValid = false;
            result.ValidationError = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// Gets seeding statistics
    /// </summary>
    public async Task<IdentitySeedingStatistics> GetSeedingStatisticsAsync()
    {
        _logger.LogInformation("Getting Identity module seeding statistics...");

        try
        {
            var statistics = new IdentitySeedingStatistics
            {
                RoleCount = await _roleSeeder.GetSeededRoleCountAsync(),
                AdminUserCount = await _userSeeder.GetAdminUserCountAsync(),
                VerifiedUserCount = await _userSeeder.GetVerifiedUserCountAsync(),
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Identity module seeding statistics: Roles: {RoleCount}, Admin Users: {AdminUserCount}, Verified Users: {VerifiedUserCount}", 
                statistics.RoleCount, statistics.AdminUserCount, statistics.VerifiedUserCount);

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Identity module seeding statistics");
            throw;
        }
    }

    /// <summary>
    /// Checks if seeding is needed
    /// </summary>
    public async Task<bool> IsSeedingNeededAsync()
    {
        try
        {
            var roleCount = await _roleSeeder.GetSeededRoleCountAsync();
            var adminUserCount = await _userSeeder.GetAdminUserCountAsync();

            // Seeding is needed if no system roles or no admin users exist
            var needsSeeding = roleCount == 0 || adminUserCount == 0;

            _logger.LogInformation("Seeding check: Role Count: {RoleCount}, Admin UserDetail Count: {AdminUserCount}, Needs Seeding: {NeedsSeeding}", 
                roleCount, adminUserCount, needsSeeding);

            return needsSeeding;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if seeding is needed");
            return true; // Assume seeding is needed if we can't check
        }
    }
}