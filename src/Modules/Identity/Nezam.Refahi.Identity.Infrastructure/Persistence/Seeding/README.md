# Identity Module Seeding

This directory contains seeders for the Identity module that automatically populate the database with default system roles and admin users.

## Overview

The Identity seeding system provides:
- **Default System Roles**: Pre-configured roles with appropriate permissions
- **Admin Users**: Default administrator accounts for system access
- **Automatic Seeding**: Runs automatically on application startup
- **Idempotent Operations**: Safe to run multiple times without creating duplicates

## Components

### 1. RoleSeeder
Seeds default system roles with their associated claims:
- **Administrator**: Full system access with all permissions
- **Manager**: User management and system settings access
- **Moderator**: Limited administrative access for content moderation
- **User**: Standard user with basic profile access
- **Guest**: Read-only access for guest users

### 2. UserSeeder
Creates default admin users:
- **Primary Admin**: مدیر سیستم (2741153671 / 09371770774)
- **Secondary Admin**: Admin User (1234567890 / 09123456789)
- **Super Admin**: Super Admin (0987654321 / 09987654321)

### 3. IdentityDataSeeder
Main orchestrator that coordinates the seeding process:
- Manages the seeding workflow
- Provides validation and statistics
- Handles error scenarios gracefully

### 4. IdentitySeedingService
Background service that automatically runs seeding on application startup:
- Checks if seeding is needed
- Performs seeding if required
- Validates results
- Logs all operations

## Usage

### Automatic Seeding
The seeding runs automatically when the application starts. No manual intervention is required.

### Manual Seeding
If you need to run seeding manually:

```csharp
// Inject the seeder service
var seeder = serviceProvider.GetRequiredService<IdentityDataSeeder>();

// Seed all data
await seeder.SeedAllDataAsync();

// Or seed specific components
await seeder.SeedRolesOnlyAsync();
await seeder.SeedUsersOnlyAsync();

// Validate seeding
var validation = await seeder.ValidateSeedingAsync();
```

### PowerShell Script
Use the provided PowerShell script:
```powershell
.\seed-identity.ps1
```

## Configuration

### Default Roles
Roles are configured in `RoleSeeder.GetDefaultSystemRoles()`:
- Each role has a name, description, and display order
- System roles are marked as non-deletable
- Roles include appropriate permission and scope claims

### Default Admin Users
Admin users are configured in `UserSeeder.GetDefaultAdminUsers()`:
- Each user has first name, last name, national ID, and phone number
- Users are automatically verified (phone verification)
- Users are assigned the Administrator role

## Validation

The seeding system includes comprehensive validation:
- Checks if roles exist and have correct claims
- Verifies admin users exist and have proper roles
- Provides detailed statistics and error reporting
- Logs all operations for debugging

## Error Handling

The seeding system is designed to be robust:
- **Idempotent**: Safe to run multiple times
- **Graceful Failures**: Logs errors but doesn't crash the application
- **Detailed Logging**: Comprehensive logging for troubleshooting
- **Validation**: Post-seeding validation to ensure success

## Logging

All seeding operations are logged with appropriate levels:
- **Information**: Normal operations and successful completions
- **Warning**: Non-critical issues (e.g., data already exists)
- **Error**: Critical failures that prevent seeding
- **Debug**: Detailed information for troubleshooting

## Database Schema

The seeding works with the Identity module's database schema:
- **Schema**: `identity`
- **Tables**: `Users`, `Roles`, `UserRoles`, `RoleClaims`, etc.
- **Migrations**: Independent migrations for the Identity context

## Security Considerations

- Admin users are created with verified phone numbers
- Roles include appropriate permission claims
- System roles cannot be deleted or modified
- All operations are logged for audit purposes

## Troubleshooting

### Common Issues

1. **Seeding Fails on Startup**
   - Check database connection string
   - Ensure database is accessible
   - Verify migrations have been applied

2. **Duplicate Data Errors**
   - The seeding is idempotent, so this shouldn't happen
   - Check for manual data insertion
   - Verify repository implementations

3. **Role Assignment Failures**
   - Ensure roles are seeded before users
   - Check role repository implementation
   - Verify user-role relationship setup

### Debugging

Enable detailed logging to troubleshoot issues:
```json
{
  "Logging": {
    "LogLevel": {
      "Nezam.Refahi.Identity.Infrastructure.Persistence.Seeding": "Debug"
    }
  }
}
```

## Extending the Seeding

To add new default roles or users:

1. **Add New Roles**: Update `RoleSeeder.GetDefaultSystemRoles()`
2. **Add New Users**: Update `UserSeeder.GetDefaultAdminUsers()`
3. **Add New Claims**: Extend the claims configuration in role definitions
4. **Custom Logic**: Add new methods to the appropriate seeder classes

## Best Practices

- Keep seeded data minimal and essential
- Use meaningful names and descriptions
- Include appropriate permissions and scopes
- Test seeding in development environments
- Document any custom configurations
- Monitor seeding logs in production
