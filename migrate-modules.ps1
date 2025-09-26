# Comprehensive Migration Script for Membership and BasicDefinitions Modules
# This script handles migrations for both Membership and BasicDefinitions bounded contexts

param(
    [string]$Module = "all",  # "membership", "basicdefinitions", or "all"
    [string]$MigrationName = "",
    [switch]$ApplyMigration = $false,
    [switch]$Force = $false
)

Write-Host "=== Nezam.Refahi Database Migration Tool ===" -ForegroundColor Cyan
Write-Host "Module: $Module" -ForegroundColor Yellow
Write-Host "Apply Migration: $ApplyMigration" -ForegroundColor Yellow
Write-Host "Force: $Force" -ForegroundColor Yellow
Write-Host ""

# Check if dotnet ef tools are installed
Write-Host "Checking for Entity Framework Core tools..." -ForegroundColor Yellow
$efToolsInstalled = dotnet tool list --global | Select-String "dotnet-ef"
if (-not $efToolsInstalled) {
    Write-Host "Installing Entity Framework Core tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to install Entity Framework Core tools"
        exit 1
    }
}

function Migrate-Module {
    param(
        [string]$ModuleName,
        [string]$InfraPath,
        [string]$DbContextName,
        [string]$DefaultMigrationName
    )
    
    Write-Host "=== Processing $ModuleName Module ===" -ForegroundColor Green
    
    if (-not (Test-Path $InfraPath)) {
        Write-Error "$ModuleName Infrastructure project not found at: $InfraPath"
        return $false
    }
    
    Write-Host "Changing to $ModuleName Infrastructure directory: $InfraPath" -ForegroundColor Yellow
    $originalLocation = Get-Location
    Set-Location $InfraPath
    
    try {
        # Determine migration name
        $migrationName = $MigrationName
        if ([string]::IsNullOrWhiteSpace($migrationName)) {
            $migrationName = $DefaultMigrationName
        }
        
        Write-Host "Creating migration: $migrationName" -ForegroundColor Cyan
        dotnet ef migrations add $migrationName --context $DbContextName --startup-project ..\..\..\..\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Migration '$migrationName' created successfully!" -ForegroundColor Green
            
            # Apply migration if requested
            if ($ApplyMigration) {
                Write-Host "Applying migration to database..." -ForegroundColor Yellow
                dotnet ef database update --context $DbContextName --startup-project ..\..\..\..\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "Migration applied successfully!" -ForegroundColor Green
                    return $true
                } else {
                    Write-Error "Failed to apply migration to database"
                    return $false
                }
            } else {
                Write-Host "Migration created but not applied. Use -ApplyMigration to apply it." -ForegroundColor Yellow
                return $true
            }
        } else {
            Write-Error "Failed to create migration"
            return $false
        }
    }
    finally {
        Set-Location $originalLocation
    }
}

# Process migrations based on module parameter
$success = $true

switch ($Module.ToLower()) {
    "membership" {
        $success = Migrate-Module -ModuleName "Membership" -InfraPath "src\Modules\Membership\Nezam.Refahi.Membership.Infrastructure" -DbContextName "MembershipDbContext" -DefaultMigrationName "UpdateMembershipSchema"
    }
    "basicdefinitions" {
        $success = Migrate-Module -ModuleName "BasicDefinitions" -InfraPath "src\Modules\BasicDefinitions\Nezam.Refahi.BasicDefinitions.Infrastructure" -DbContextName "BasicDefinitionsDbContext" -DefaultMigrationName "UpdateBasicDefinitionsSchema"
    }
    "all" {
        Write-Host "Processing all modules..." -ForegroundColor Cyan
        
        # Process Membership first
        $membershipSuccess = Migrate-Module -ModuleName "Membership" -InfraPath "src\Modules\Membership\Nezam.Refahi.Membership.Infrastructure" -DbContextName "MembershipDbContext" -DefaultMigrationName "UpdateMembershipSchema"
        
        Write-Host ""
        
        # Process BasicDefinitions second
        $basicDefinitionsSuccess = Migrate-Module -ModuleName "BasicDefinitions" -InfraPath "src\Modules\BasicDefinitions\Nezam.Refahi.BasicDefinitions.Infrastructure" -DbContextName "BasicDefinitionsDbContext" -DefaultMigrationName "UpdateBasicDefinitionsSchema"
        
        $success = $membershipSuccess -and $basicDefinitionsSuccess
    }
    default {
        Write-Error "Invalid module parameter. Use 'membership', 'basicdefinitions', or 'all'"
        exit 1
    }
}

Write-Host ""
if ($success) {
    Write-Host "=== Migration Process Completed Successfully ===" -ForegroundColor Green
} else {
    Write-Host "=== Migration Process Completed with Errors ===" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Usage Examples:" -ForegroundColor Cyan
Write-Host "  .\migrate-modules.ps1 -Module membership -MigrationName 'AddNewField' -ApplyMigration" -ForegroundColor White
Write-Host "  .\migrate-modules.ps1 -Module basicdefinitions -ApplyMigration" -ForegroundColor White
Write-Host "  .\migrate-modules.ps1 -Module all -ApplyMigration" -ForegroundColor White
