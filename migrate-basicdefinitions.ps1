# Migration script for BasicDefinitions module
# This script creates and applies migrations for the BasicDefinitions bounded context

Write-Host "Starting BasicDefinitions module migration process..." -ForegroundColor Green

# Set the working directory to the BasicDefinitions Infrastructure project
$basicDefinitionsInfraPath = "src\Modules\BasicDefinitions\Nezam.Refahi.BasicDefinitions.Infrastructure"

if (-not (Test-Path $basicDefinitionsInfraPath)) {
    Write-Error "BasicDefinitions Infrastructure project not found at: $basicDefinitionsInfraPath"
    exit 1
}

Write-Host "Changing to BasicDefinitions Infrastructure directory: $basicDefinitionsInfraPath" -ForegroundColor Yellow
Set-Location $basicDefinitionsInfraPath

# Check if dotnet ef tools are installed
Write-Host "Checking for Entity Framework Core tools..." -ForegroundColor Yellow
$efToolsInstalled = dotnet tool list --global | Select-String "dotnet-ef"
if (-not $efToolsInstalled) {
    Write-Host "Installing Entity Framework Core tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
}

# Create a new migration
Write-Host "Creating new migration for BasicDefinitions module..." -ForegroundColor Yellow
$migrationName = Read-Host "Enter migration name (or press Enter for 'UpdateBasicDefinitionsSchema')"
if ([string]::IsNullOrWhiteSpace($migrationName)) {
    $migrationName = "UpdateBasicDefinitionsSchema"
}

Write-Host "Creating migration: $migrationName" -ForegroundColor Cyan
dotnet ef migrations add $migrationName --context BasicDefinitionsDbContext --startup-project ..\..\..\..\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration '$migrationName' created successfully!" -ForegroundColor Green
    
    # Ask if user wants to apply the migration
    $applyMigration = Read-Host "Do you want to apply the migration to the database? (y/n)"
    if ($applyMigration -eq "y" -or $applyMigration -eq "Y") {
        Write-Host "Applying migration to database..." -ForegroundColor Yellow
        dotnet ef database update --context BasicDefinitionsDbContext --startup-project ..\..\..\..\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Migration applied successfully!" -ForegroundColor Green
        } else {
            Write-Error "Failed to apply migration to database"
        }
    }
} else {
    Write-Error "Failed to create migration"
}

# Return to original directory
Set-Location ..\..\..\..\..\

Write-Host "BasicDefinitions migration process completed!" -ForegroundColor Green
