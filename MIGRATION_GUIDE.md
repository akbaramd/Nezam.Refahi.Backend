# Database Migration Guide

This guide explains how to create and apply database migrations for the Membership and BasicDefinitions modules in the Nezam.Refahi project.

## Overview

The project uses Entity Framework Core with separate DbContexts for each bounded context:
- **Membership Module**: `MembershipDbContext` (schema: `membership`)
- **BasicDefinitions Module**: `BasicDefinitionsDbContext` (schema: `definitions`)

Each module has its own DbContext factory for design-time operations (migrations).

## Migration Scripts

### 1. Comprehensive Migration Script (`migrate-modules.ps1`)

The main PowerShell script that handles migrations for both modules.

**Usage:**
```powershell
# Create migration for Membership module
.\migrate-modules.ps1 -Module membership -MigrationName "AddNewField" -ApplyMigration

# Create migration for BasicDefinitions module
.\migrate-modules.ps1 -Module basicdefinitions -MigrationName "UpdateCapabilities" -ApplyMigration

# Create migrations for both modules
.\migrate-modules.ps1 -Module all -ApplyMigration
```

**Parameters:**
- `-Module`: Specify which module to migrate (`membership`, `basicdefinitions`, or `all`)
- `-MigrationName`: Custom name for the migration (optional)
- `-ApplyMigration`: Automatically apply the migration to the database
- `-Force`: Force overwrite existing migrations

### 2. Individual Module Scripts

#### Membership Module (`migrate-membership.ps1`)
```powershell
.\migrate-membership.ps1
```

#### BasicDefinitions Module (`migrate-basicdefinitions.ps1`)
```powershell
.\migrate-basicdefinitions.ps1
```

### 3. Batch File (`migrate-modules.bat`)

Simple Windows batch file for easy execution:
```cmd
migrate-modules.bat -Module all -ApplyMigration
```

## Manual Migration Commands

If you prefer to run migrations manually:

### Membership Module
```bash
# Navigate to Membership Infrastructure project
cd src\Modules\Membership\Nezam.Refahi.Membership.Infrastructure

# Create migration
dotnet ef migrations add YourMigrationName --context MembershipDbContext --startup-project ..\..\..\..\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj

# Apply migration
dotnet ef database update --context MembershipDbContext --startup-project ..\..\..\..\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj
```

### BasicDefinitions Module
```bash
# Navigate to BasicDefinitions Infrastructure project
cd src\Modules\BasicDefinitions\Nezam.Refahi.BasicDefinitions.Infrastructure

# Create migration
dotnet ef migrations add YourMigrationName --context BasicDefinitionsDbContext --startup-project ..\..\..\..\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj

# Apply migration
dotnet ef database update --context BasicDefinitionsDbContext --startup-project ..\..\..\..\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj
```

## Prerequisites

1. **Entity Framework Core Tools**: The scripts will automatically install if not present:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. **Database Connection**: Ensure your connection string is properly configured in `appsettings.json`

3. **Project Structure**: The scripts assume the standard project structure with WebApi as the startup project

## Database Schema Information

### Membership Module
- **Schema**: `membership`
- **Migration History Table**: `__EFMigrationsHistory__Membership`
- **Entities**: Member, MemberCapability, Role, MemberRole

### BasicDefinitions Module
- **Schema**: `definitions`
- **Migration History Table**: `__EFMigrationsHistory__BasicDefinitions`
- **Entities**: RepresentativeOffice, Features, Capability

## Troubleshooting

### Common Issues

1. **Connection String Issues**
   - Verify the connection string in `appsettings.json`
   - Ensure the database server is accessible

2. **Migration Conflicts**
   - Use `-Force` parameter to overwrite existing migrations
   - Check for pending migrations before creating new ones

3. **Permission Issues**
   - Ensure the database user has sufficient permissions
   - Run as administrator if needed

### Verification Commands

```bash
# List all migrations
dotnet ef migrations list --context MembershipDbContext
dotnet ef migrations list --context BasicDefinitionsDbContext

# Check database status
dotnet ef database update --context MembershipDbContext --dry-run
dotnet ef database update --context BasicDefinitionsDbContext --dry-run
```

## Best Practices

1. **Always backup** your database before applying migrations
2. **Test migrations** in a development environment first
3. **Use descriptive names** for migrations
4. **Review migration files** before applying to production
5. **Coordinate with team** when making schema changes

## Support

For issues or questions regarding migrations, please refer to:
- Entity Framework Core documentation
- Project architecture documentation
- Team lead or senior developers
