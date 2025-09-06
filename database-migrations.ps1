param (
    [Parameter(Mandatory=$false)]
    [string]$MigrationName = "Update",
    
    [Parameter(Mandatory=$false)]
    [switch]$UpdateOnly,
    
    [Parameter(Mandatory=$false)]
    [switch]$ListMigrations,
    
    [Parameter(Mandatory=$false)]
    [string]$RemoveMigration,
    
    [Parameter(Mandatory=$false)]
    [switch]$Help
)

$infrastructureProject = "src\Modules\Identity\Nezam.Refahi.Identity.Infrastructure"
$startupProject = "src\Nezam.Refahi.WebApi"
$migrationsDir = "Migrations"

# Function to display help information
function Show-Help {
    Write-Host "Database Migrations Management Script" -ForegroundColor Cyan
    Write-Host "====================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Parameters:" -ForegroundColor Yellow
    Write-Host "  -MigrationName <name>  : Base name for the migration (default: 'Update')" -ForegroundColor White
    Write-Host "  -UpdateOnly            : Only update the database without creating a new migration" -ForegroundColor White
    Write-Host "  -ListMigrations        : List all existing migrations" -ForegroundColor White
    Write-Host "  -RemoveMigration <name>: Remove the specified migration" -ForegroundColor White
    Write-Host "  -Help                  : Display this help information" -ForegroundColor White
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Yellow
    Write-Host "  .\database-migrations.ps1" -ForegroundColor White
    Write-Host "  .\database-migrations.ps1 -MigrationName 'AddUserEntity'" -ForegroundColor White
    Write-Host "  .\database-migrations.ps1 -UpdateOnly" -ForegroundColor White
    Write-Host "  .\database-migrations.ps1 -ListMigrations" -ForegroundColor White
    Write-Host "  .\database-migrations.ps1 -RemoveMigration 'AddUserEntity_20250521'" -ForegroundColor White
    Write-Host ""
}

# Show help if requested
if ($Help) {
    Show-Help
    exit 0
}

# Function to check if dotnet ef is installed
function Test-EFInstalled {
    try {
        $efVersion = dotnet ef --version
        return $true
    }
    catch {
        return $false
    }
}

# Function to list all migrations
function List-Migrations {
    Write-Host "Listing all migrations..." -ForegroundColor Cyan
    dotnet ef migrations list --project $infrastructureProject --startup-project $startupProject
}

# Function to remove a migration
function Remove-EFMigration {
    param (
        [string]$MigrationName
    )
    
    Write-Host "Removing migration '$MigrationName'..." -ForegroundColor Yellow
    dotnet ef migrations remove --project $infrastructureProject --startup-project $startupProject --force
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration '$MigrationName' successfully removed." -ForegroundColor Green
    }
    else {
        Write-Host "Failed to remove migration '$MigrationName'." -ForegroundColor Red
    }
}

# Function to add a new migration
function Add-Migration {
    param (
        [string]$Name
    )
    
    # Add timestamp to make the migration name unique
    $timestamp = Get-Date -Format "yyyyMMddHHmmss"
    $uniqueName = "${Name}_${timestamp}"
    
    Write-Host "Creating migration '$uniqueName'..." -ForegroundColor Cyan
    dotnet ef migrations add $uniqueName --project $infrastructureProject --startup-project $startupProject --output-dir $migrationsDir
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration '$uniqueName' successfully created." -ForegroundColor Green
        return $true
    }
    else {
        Write-Host "Failed to create migration '$uniqueName'." -ForegroundColor Red
        return $false
    }
}

# Function to update the database
function Update-Database {
    Write-Host "Updating database..." -ForegroundColor Cyan
    dotnet ef database update --project $infrastructureProject --startup-project $startupProject
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Database successfully updated." -ForegroundColor Green
        return $true
    }
    else {
        Write-Host "Failed to update database." -ForegroundColor Red
        return $false
    }
}

# Main script execution
try {
    # Check if EF Core tools are installed
    if (-not (Test-EFInstalled)) {
        Write-Host "Entity Framework Core tools are not installed. Installing..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-ef
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to install Entity Framework Core tools. Please install manually using 'dotnet tool install --global dotnet-ef'." -ForegroundColor Red
            exit 1
        }
    }
    
    # List migrations if requested
    if ($ListMigrations) {
        List-Migrations
        exit 0
    }
    
    # Remove migration if requested
    if ($RemoveMigration) {
        Remove-EFMigration -MigrationName $RemoveMigration
        exit 0
    }
    
    # If not just updating, create a new migration
    $migrationCreated = $true
    if (-not $UpdateOnly) {
        $migrationCreated = Add-Migration -Name $MigrationName
    }
    
    # Update the database if migration was created successfully or if only updating
    if ($migrationCreated -or $UpdateOnly) {
        Update-Database
    }
}
catch {
    Write-Host "An error occurred: $_" -ForegroundColor Red
    exit 1
}
