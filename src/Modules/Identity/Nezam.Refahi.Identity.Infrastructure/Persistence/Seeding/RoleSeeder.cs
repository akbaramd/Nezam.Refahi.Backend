using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeder for default system roles
/// </summary>
public class RoleSeeder
{
    private readonly IRoleRepository _roleRepository;
    private readonly IIdentityUnitOfWork  _unitOfWork;
    private readonly ILogger<RoleSeeder> _logger;

    public RoleSeeder(IRoleRepository roleRepository, IIdentityUnitOfWork unitOfWork, ILogger<RoleSeeder> logger)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Seeds default system roles if they don't exist
    /// </summary>
    public async Task SeedDefaultRolesAsync()
    {
        _logger.LogInformation("Starting to seed default system roles...");

        var defaultRoles = GetDefaultSystemRoles();
        var rolesCreated = 0;

        try
        {
            await _unitOfWork.BeginAsync();

            foreach (var (roleName, description, isSystemRole, displayOrder, claims) in defaultRoles)
            {
                // Check if role already exists
                var existingRole = await _roleRepository.GetByNameAsync(roleName);
                if (existingRole != null)
                {
                    _logger.LogInformation("Role '{RoleName}' already exists, skipping...", roleName);
                    continue;
                }

                // Create new role
                var role = new Role(roleName, description, isSystemRole, displayOrder);

                // Add role to repository
                await _roleRepository.AddAsync(role);
                
                // Now add claims to role
                foreach (var (claimType, claimValue) in claims)
                {
                    var claim = new Claim(claimType, claimValue);
                    role.AddClaim(claim);
                }

                rolesCreated++;
                _logger.LogInformation("Successfully prepared role '{RoleName}' with {ClaimCount} claims", 
                    roleName, claims.Count);
            }

            // Save all changes at once
            _logger.LogInformation("Saving {RolesCreated} roles to database...", rolesCreated);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Committing transaction...");
            await _unitOfWork.CommitAsync();
            
            _logger.LogInformation("Completed seeding default system roles. Created {RolesCreated} roles", rolesCreated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed default system roles");
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Gets the default system roles configuration
    /// </summary>
    private static List<(string Name, string Description, bool IsSystemRole, int DisplayOrder, List<(string Type, string Value)> Claims)> GetDefaultSystemRoles()
    {
        return new List<(string, string, bool, int, List<(string, string)>)>
        {
            // Administrator Role
            (
                "Administrator",
                "System administrator with full access to all features and settings",
                true,
                1,
                new List<(string, string)>
                {
                    ("permission", "users.read"),
                    ("permission", "users.create"),
                    ("permission", "users.update"),
                    ("permission", "users.delete"),
                    ("permission", "roles.read"),
                    ("permission", "roles.create"),
                    ("permission", "roles.update"),
                    ("permission", "roles.delete"),
                    ("permission", "system.restore"),
                    ("scope", "panel"),
                    ("scope", "app")
                }
            ),

            // Employer Role
            (
                "Employer",
                "Employer with access to panel features",
                true,
                2,
                new List<(string, string)>
                {
                    ("permission", "panel.access"),
                    ("permission", "members.read"),
                    ("permission", "members.manage"),
                    ("permission", "reports.view"),
                    ("scope", "panel")
                }
            ),
          
            // Member Role
            (
                "Member",
                "Member with access to app features",
                true,
                3,
                new List<(string, string)>
                {
                    ("permission", "profile.read"),
                    ("permission", "profile.update"),
                    ("permission", "services.access"),
                    ("scope", "app")
                }
            ),

            // Guest Role
            (
                "Guest",
                "Guest user with read-only access",
                true,
                4,
                new List<(string, string)>
                {
                    ("permission", "profile.read"),
                    ("scope", "app")
                }
            )
        };
    }

    /// <summary>
    /// Validates that all default roles exist
    /// </summary>
    public async Task<bool> ValidateDefaultRolesAsync()
    {
        _logger.LogInformation("Validating default system roles...");

        var defaultRoles = GetDefaultSystemRoles();
        var allRolesExist = true;

        foreach (var (roleName, _, _, _, _) in defaultRoles)
        {
            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null)
            {
                _logger.LogWarning("Default role '{RoleName}' is missing", roleName);
                allRolesExist = false;
            }
            else
            {
                _logger.LogDebug("Default role '{RoleName}' exists", roleName);
            }
        }

        if (allRolesExist)
        {
            _logger.LogInformation("All default system roles are present");
        }
        else
        {
            _logger.LogWarning("Some default system roles are missing");
        }

        return allRolesExist;
    }

    /// <summary>
    /// Gets the count of seeded roles
    /// </summary>
    public async Task<int> GetSeededRoleCountAsync()
    {
        var systemRoles = await _roleRepository.GetSystemRolesAsync();
        return systemRoles.Count();
    }
}
