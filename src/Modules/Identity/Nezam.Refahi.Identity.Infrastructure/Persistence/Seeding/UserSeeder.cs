using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeder for default admin users
/// </summary>
public class UserSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ILogger<UserSeeder> _logger;

    public UserSeeder(IUserRepository userRepository, IRoleRepository roleRepository, IIdentityUnitOfWork unitOfWork, ILogger<UserSeeder> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Seeds default admin users if they don't exist
    /// </summary>
    public async Task SeedDefaultAdminUsersAsync()
    {
        _logger.LogInformation("Starting to seed default admin users...");

        var defaultAdmins = GetDefaultAdminUsers();
        var usersCreated = 0;

        try
        {
            await _unitOfWork.BeginAsync();

            foreach (var admin in defaultAdmins)
            {
                // Check if user already exists by phone number
                var existingUser = await _userRepository.GetByPhoneNumberAsync(admin.PhoneNumber);
                if (existingUser != null)
                {
                    _logger.LogInformation("Admin user with phone '{PhoneNumber}' already exists, skipping...", admin.PhoneNumber);
                    continue;
                }

                // Check if user already exists by national ID
                if (!string.IsNullOrEmpty(admin.NationalId))
                {
                    var nationalId = new NationalId(admin.NationalId);
                    var existingUserByNationalId = await _userRepository.GetByNationalIdAsync(nationalId);
                    if (existingUserByNationalId != null)
                    {
                        _logger.LogInformation("Admin user with national ID '{NationalId}' already exists, skipping...", admin.NationalId);
                        continue;
                    }
                }

                // Create new admin user
                User user;
                if (!string.IsNullOrEmpty(admin.NationalId))
                {
                    user = new User(admin.FirstName, admin.LastName, admin.NationalId, admin.PhoneNumber);
                }
                else
                {
                    user = new User(admin.PhoneNumber);
                    user.UpdateName(admin.FirstName, admin.LastName);
                }

                // Verify phone number for admin users
                user.VerifyPhone();
                // Now that user has an ID, initialize default preferences
                user.EnsureDefaultPreferences();

                // Assign administrator role
                var adminRole = await _roleRepository.GetByNameAsync("Administrator");
                if (adminRole != null)
                {
                  user.AssignRole(adminRole.Id);
                  _logger.LogInformation("Successfully prepared admin user '{FirstName} {LastName}' with phone '{PhoneNumber}' and assigned Administrator role", 
                    admin.FirstName, admin.LastName, admin.PhoneNumber);
                }
                else
                {
                  _logger.LogWarning("Administrator role not found, admin user '{FirstName} {LastName}' will be created without role assignment", 
                    admin.FirstName, admin.LastName);
                }
                // Add user to repository
                await _userRepository.AddAsync(user);
                usersCreated++;
            }

            // Save all users first to get their IDs
            await _unitOfWork.SaveAsync();



            // Save all changes (preferences and roles)
            _logger.LogInformation("Saving {UsersCreated} users to database...", usersCreated);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Committing transaction...");
            await _unitOfWork.CommitAsync();
            
            _logger.LogInformation("Completed seeding default admin users. Created {UsersCreated} users", usersCreated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed default admin users");
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Gets the default admin users configuration
    /// </summary>
    private static List<DefaultAdminUser> GetDefaultAdminUsers()
    {
        return new List<DefaultAdminUser>
        {
            new DefaultAdminUser
            {
                FirstName = "مدیر",
                LastName = "سیستم",
                NationalId = "2741153671",
                PhoneNumber = "09371770774",
                Description = "Primary system administrator"
            },
           
        };
    }

    /// <summary>
    /// Validates that admin users exist
    /// </summary>
    public async Task<bool> ValidateAdminUsersAsync()
    {
        _logger.LogInformation("Validating admin users...");

        var defaultAdmins = GetDefaultAdminUsers();
        var allAdminsExist = true;

        foreach (var admin in defaultAdmins)
        {
            var user = await _userRepository.GetByPhoneNumberAsync(admin.PhoneNumber);
            if (user == null)
            {
                _logger.LogWarning("Admin user with phone '{PhoneNumber}' is missing", admin.PhoneNumber);
                allAdminsExist = false;
            }
            else
            {
                // Check if user has administrator role
                var hasAdminRole = user.HasRole("Administrator");
                if (!hasAdminRole)
                {
                    _logger.LogWarning("Admin user '{FirstName} {LastName}' exists but doesn't have Administrator role", 
                        user.FirstName, user.LastName);
                    allAdminsExist = false;
                }
                else
                {
                    _logger.LogDebug("Admin user '{FirstName} {LastName}' exists with Administrator role", 
                        user.FirstName, user.LastName);
                }
            }
        }

        if (allAdminsExist)
        {
            _logger.LogInformation("All admin users are present and properly configured");
        }
        else
        {
            _logger.LogWarning("Some admin users are missing or not properly configured");
        }

        return allAdminsExist;
    }

    /// <summary>
    /// Gets the count of admin users
    /// </summary>
    public async Task<int> GetAdminUserCountAsync()
    {
        var adminRole = await _roleRepository.GetByNameAsync("Administrator");
        if (adminRole == null)
            return 0;

        var allUsers = await _userRepository.FindAsync(x=>true);
        var adminUsers = allUsers.Count(u => u.HasRole(adminRole.Id));
        return adminUsers;
    }

    /// <summary>
    /// Gets the count of verified users
    /// </summary>
    public async Task<int> GetVerifiedUserCountAsync()
    {
        var verifiedUsers = await _userRepository.GetVerifiedUsersAsync();
        return verifiedUsers.Count();
    }

    /// <summary>
    /// Represents a default admin user configuration
    /// </summary>
    private class DefaultAdminUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
