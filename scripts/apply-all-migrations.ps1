<#
.SYNOPSIS
    Applies all database migrations for all modules
    
.DESCRIPTION
    This script applies database migrations for all infrastructure modules
    using their respective DbContext types.
    
.PARAMETER DeleteExistingData
    If specified, drops and recreates the database (WARNING: Destructive!)
#>

param(
    [switch]$DeleteExistingData
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir
$WebApiProject = Join-Path $ProjectRoot "src\Nezam.Refahi.WebApi\Nezam.Refahi.WebApi.csproj"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Apply All Database Migrations" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($DeleteExistingData) {
    Write-Host "WARNING: This will DELETE ALL existing database data!" -ForegroundColor Red
    $confirmation = Read-Host "Are you sure you want to continue? (yes/no)"
    if ($confirmation -ne "yes") {
        Write-Host "Operation cancelled." -ForegroundColor Yellow
        exit 0
    }
}

# Define DbContext configurations
$DbContextConfigs = @(
    @{
        Name = "Identity"
        Context = "Nezam.Refahi.Identity.Infrastructure.Persistence.IdentityDbContext"
        Project = "src\Modules\Identity\Nezam.Refahi.Identity.Infrastructure\Nezam.Refahi.Identity.Infrastructure.csproj"
    },
    @{
        Name = "Membership"
        Context = "Nezam.Refahi.Membership.Infrastructure.Persistence.MembershipDbContext"
        Project = "src\Modules\Membership\Nezam.Refahi.Membership.Infrastructure\Nezam.Refahi.Membership.Infrastructure.csproj"
    },
    @{
        Name = "Facilities"
        Context = "FacilitiesDbContext"
        Project = "src\Modules\Facilities\Nezam.Refahi.Facilities.Infrastructure\Nezam.Refahi.Facilities.Infrastructure.csproj"
    },
    @{
        Name = "Settings"
        Context = "Nezam.Refahi.Settings.Infrastructure.Persistence.SettingsDbContext"
        Project = "src\Modules\Settings\Nezam.Refahi.Settings.Infrastructure\Nezam.Refahi.Settings.Infrastructure.csproj"
    },
    @{
        Name = "BasicDefinitions"
        Context = "Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence.BasicDefinitionsDbContext"
        Project = "src\Modules\BasicDefinitions\Nezam.Refahi.BasicDefinitions.Infrastructure\Nezam.Refahi.BasicDefinitions.Infrastructure.csproj"
    },
    @{
        Name = "Surveying"
        Context = "Nezam.Refahi.Surveying.Infrastructure.Persistence.SurveyingDbContext"
        Project = "src\Modules\Survey\Nezam.Refahi.Surveying.Infrastructure\Nezam.Refahi.Surveying.Infrastructure.csproj"
    },
    @{
        Name = "Finance"
        Context = "Nezam.Refahi.Finance.Infrastructure.Persistence.FinanceDbContext"
        Project = "src\Modules\Finance\Nezam.Refahi.Finance.Infrastructure\Nezam.Refahi.Finance.Infrastructure.csproj"
    },
    @{
        Name = "Recreation"
        Context = "Nezam.Refahi.Recreation.Infrastructure.Persistence.RecreationDbContext"
        Project = "src\Modules\Recreation\Nezam.Refahi.Recreation.Infrastructure\Nezam.Refahi.Recreation.Infrastructure.csproj"
    },
    @{
        Name = "Notification"
        Context = "Nezam.Refahi.Notification.Infrastructure.Persistence.NotificationDbContext"
        Project = "src\Modules\Notification\Nezam.Refahi.Notification.Infrastructure\Nezam.Refahi.Notification.Infrastructure.csproj"
    }
)

Push-Location $ProjectRoot
try {
    foreach ($config in $DbContextConfigs) {
        $projectPath = Join-Path $ProjectRoot $config.Project
        
        if (-not (Test-Path $projectPath)) {
            Write-Host "Skipping $($config.Name): Project not found" -ForegroundColor Yellow
            continue
        }
        
        Write-Host ""
        Write-Host "Processing $($config.Name)..." -ForegroundColor Cyan
        
        $projectDir = Split-Path -Parent $projectPath
        Push-Location $projectDir
        try {
            if ($DeleteExistingData) {
                Write-Host "  Dropping database for $($config.Name)..." -ForegroundColor Yellow
                dotnet ef database drop --context $config.Context --startup-project $WebApiProject --force --no-build 2>&1 | Out-Null
            }
            
            Write-Host "  Applying migrations for $($config.Name)..." -ForegroundColor Yellow
            $result = dotnet ef database update --context $config.Context --startup-project $WebApiProject --no-build 2>&1
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  ✓ $($config.Name) migrations applied successfully" -ForegroundColor Green
            }
            else {
                Write-Host "  ✗ Failed to apply migrations for $($config.Name)" -ForegroundColor Red
                Write-Host $result -ForegroundColor Red
            }
        }
        catch {
            Write-Host "  ✗ Error processing $($config.Name): $_" -ForegroundColor Red
        }
        finally {
            Pop-Location
        }
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "All migrations processed" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}
finally {
    Pop-Location
}

