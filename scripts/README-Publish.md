# Publish Scripts Documentation

This directory contains scripts for publishing the Nezam.Refahi.WebApi project.

## Scripts

### publish-webapi.ps1
Main PowerShell script for publishing the WebApi project.

**Usage:**
```powershell
# Basic publish
.\scripts\publish-webapi.ps1 -OutputPath "C:\Publish\WebApi"

# Publish with cleanup
.\scripts\publish-webapi.ps1 -OutputPath "C:\Publish\WebApi" -CleanOutput

# Publish with migrations
.\scripts\publish-webapi.ps1 -OutputPath "C:\Publish\WebApi" -ApplyMigrations

# Publish with cleanup and delete existing data (WARNING: Destructive!)
.\scripts\publish-webapi.ps1 -OutputPath "C:\Publish\WebApi" -CleanOutput -DeleteExistingData -ApplyMigrations
```

**Parameters:**
- `OutputPath`: Output directory for publishing (default: ./publish)
- `Configuration`: Build configuration - Debug or Release (default: Release)
- `CleanOutput`: Delete existing output directory before publishing
- `ApplyMigrations`: Apply database migrations after publishing
- `DeleteExistingData`: Delete existing database data before applying migrations (WARNING: Destructive!)

### publish-webapi.bat
Batch file wrapper for the PowerShell script. Can be run directly from Windows Explorer.

**Usage:**
```cmd
.\scripts\publish-webapi.bat [OutputPath]
```

### apply-all-migrations.ps1
Applies database migrations for all modules.

**Usage:**
```powershell
# Apply all migrations
.\scripts\apply-all-migrations.ps1

# Drop and recreate all databases (WARNING: Destructive!)
.\scripts\apply-all-migrations.ps1 -DeleteExistingData
```

### delete-database-data.ps1
Generates SQL script to delete all data from database tables.

**Usage:**
```powershell
# Generate SQL script for all contexts
.\scripts\delete-database-data.ps1

# Generate SQL script for specific context
.\scripts\delete-database-data.ps1 -Context "Facilities"

# Skip confirmation (dangerous!)
.\scripts\delete-database-data.ps1 -Confirm
```

## Workflow Examples

### Standard Publish
```powershell
.\scripts\publish-webapi.ps1 -OutputPath "C:\Publish\WebApi" -Configuration Release -CleanOutput
```

### Publish with Database Migration
```powershell
.\scripts\publish-webapi.ps1 -OutputPath "C:\Publish\WebApi" -CleanOutput -ApplyMigrations
```

### Fresh Start (Delete all data and republish)
```powershell
.\scripts\publish-webapi.ps1 -OutputPath "C:\Publish\WebApi" -CleanOutput -DeleteExistingData -ApplyMigrations
```

## Important Notes

⚠️ **WARNING**: The `-DeleteExistingData` parameter will delete ALL data from the database. This operation cannot be undone!

⚠️ Always backup your database before running destructive operations.

⚠️ The scripts require:
- .NET SDK 8.0 or later
- Entity Framework Core tools (`dotnet tool install --global dotnet-ef`)
- PowerShell 5.1 or later

## Output Structure

After publishing, the output directory will contain:
- Compiled application binaries
- Configuration files (appsettings.json)
- Plugins directory
- wwwroot directory (static files)
- All required dependencies

## Troubleshooting

### Build fails
- Ensure all projects compile: `dotnet build`
- Check for missing NuGet packages: `dotnet restore`

### Migration fails
- Verify connection string in appsettings.json
- Check database server is accessible
- Ensure EF Core tools are installed: `dotnet tool install --global dotnet-ef`

### Permission errors
- Run PowerShell as Administrator if needed
- Check file system permissions for output directory

