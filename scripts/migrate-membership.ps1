# Migration script for Membership module
# This script creates and applies migrations for the Membership bounded context

Write-Host "Starting Membership module migration process..." -ForegroundColor Green

# Set the working directory to the Membership Infrastructure project
$membershipInfraPath = "src\Modules\Membership\Nezam.Refahi.Membership.Infrastructure"

if (-not (Test-Path $membershipInfraPath)) {
    Write-Error "Membership Infrastructure project not found at: $membershipInfraPath"
    exit 1
}

Write-Host "Changing to Membership Infrastructure directory: $membershipInfraPath" -ForegroundColor Yellow
Set-Location $membershipInfraPath

# Check if dotnet ef tools are installed
Write-Host "Checking for Entity Framework Core tools..." -ForegroundColor Yellow
$efToolsInstalled = dotnet tool list --global | Select-String "dotnet-ef"
if (-not $efToolsInstalled) {
    Write-Host "Installing Entity Framework Core tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
}

# Create a new migration
Write-Host "Creating new migration for Membership module..." -ForegroundColor Yellow
$migrationName = Read-Host "Enter migration name (or press Enter for 'UpdateMembershipSchema')"
if ([string]::IsNullOrWhiteSpace($migrationName)) {
    $migrationName = "UpdateMembershipSchema"
}

Write-Host "Creating migration: $migrationName" -ForegroundColor Cyan
dotnet ef migrations add $migrationName --context MembershipDbContext --startup-project ..\..\..\..\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration '$migrationName' created successfully!" -ForegroundColor Green
    
    # Ask if user wants to apply the migration
    $applyMigration = Read-Host "Do you want to apply the migration to the database? (y/n)"
    if ($applyMigration -eq "y" -or $applyMigration -eq "Y") {
        Write-Host "Applying migration to database..." -ForegroundColor Yellow
        dotnet ef database update --context MembershipDbContext --startup-project ..\..\..\..\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj
        
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

Write-Host "Membership migration process completed!" -ForegroundColor Green
