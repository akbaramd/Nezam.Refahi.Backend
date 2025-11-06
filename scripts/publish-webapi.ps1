<#
.SYNOPSIS
    Publishes the Nezam.Refahi.WebApi project with options to clean old data
    
.DESCRIPTION
    This script publishes the WebApi project to a specified output directory.
    It can optionally clean old publish output and delete existing database data.
    
.PARAMETER OutputPath
    The output directory for publishing (default: ./publish)
    
.PARAMETER Configuration
    Build configuration: Debug or Release (default: Release)
    
.PARAMETER CleanOutput
    If specified, deletes existing output directory before publishing
    
.PARAMETER ApplyMigrations
    If specified, applies database migrations after publishing
    
.PARAMETER DeleteExistingData
    If specified, deletes existing database data before applying migrations (WARNING: Destructive!)
    
.EXAMPLE
    .\publish-webapi.ps1 -OutputPath "C:\Publish\WebApi" -Configuration Release
    
.EXAMPLE
    .\publish-webapi.ps1 -OutputPath "C:\Publish\WebApi" -CleanOutput -ApplyMigrations
    
.EXAMPLE
    .\publish-webapi.ps1 -OutputPath "C:\Publish\WebApi" -CleanOutput -DeleteExistingData -ApplyMigrations
#>

param(
    [string]$OutputPath = "./publish",
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    [switch]$CleanOutput,
    [switch]$ApplyMigrations,
    [switch]$DeleteExistingData
)

# Set error handling
$ErrorActionPreference = "Stop"

# Get script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir
$WebApiPath = Join-Path $ProjectRoot "src\Nezam.Refahi.WebApi"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Nezam.Refahi.WebApi Publish Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Validate WebApi project exists
if (-not (Test-Path $WebApiPath)) {
    Write-Host "Error: WebApi project not found at: $WebApiPath" -ForegroundColor Red
    exit 1
}

$WebApiProjectFile = Join-Path $WebApiPath "Nezam.Refahi.WebApi.csproj"
if (-not (Test-Path $WebApiProjectFile)) {
    Write-Host "Error: Project file not found: $WebApiProjectFile" -ForegroundColor Red
    exit 1
}

Write-Host "Project Path: $WebApiPath" -ForegroundColor Green
Write-Host "Output Path: $OutputPath" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Green
Write-Host ""

# Clean output directory if requested
if ($CleanOutput) {
    if (Test-Path $OutputPath) {
        Write-Host "Cleaning output directory: $OutputPath" -ForegroundColor Yellow
        Remove-Item -Path $OutputPath -Recurse -Force
        Write-Host "Output directory cleaned." -ForegroundColor Green
    }
}

# Change to WebApi directory
Push-Location $WebApiPath
try {
    Write-Host "Restoring packages..." -ForegroundColor Yellow
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        throw "Package restore failed"
    }
    Write-Host "Packages restored successfully." -ForegroundColor Green
    
    Write-Host ""
    Write-Host "Building project..." -ForegroundColor Yellow
    dotnet build --configuration $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed"
    }
    Write-Host "Build completed successfully." -ForegroundColor Green
    
    Write-Host ""
    Write-Host "Publishing project..." -ForegroundColor Yellow
    $PublishArgs = @(
        "publish",
        "--configuration", $Configuration,
        "--output", $OutputPath,
        "--no-build"
    )
    
    dotnet $PublishArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Publish failed"
    }
    Write-Host "Publish completed successfully." -ForegroundColor Green
    
    # Handle database operations
    if ($DeleteExistingData -or $ApplyMigrations) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "Database Operations" -ForegroundColor Cyan
        Write-Host "========================================" -ForegroundColor Cyan
        
        if ($DeleteExistingData) {
            Write-Host ""
            Write-Host "WARNING: This will delete existing database data!" -ForegroundColor Red
            $confirmation = Read-Host "Are you sure you want to continue? (yes/no)"
            if ($confirmation -ne "yes") {
                Write-Host "Database deletion cancelled." -ForegroundColor Yellow
                exit 0
            }
            
            Write-Host "Deleting existing database data..." -ForegroundColor Yellow
            $deleteScript = Join-Path $ScriptDir "delete-database-data.ps1"
            if (Test-Path $deleteScript) {
                & $deleteScript -Confirm
            }
            else {
                Write-Host "Note: Using migration drop/recreate instead." -ForegroundColor Yellow
            }
        }
        
        if ($ApplyMigrations) {
            Write-Host ""
            Write-Host "Applying database migrations..." -ForegroundColor Yellow
            
            # Use the apply-all-migrations script
            $migrateScript = Join-Path $ScriptDir "apply-all-migrations.ps1"
            if (Test-Path $migrateScript) {
                Push-Location $ProjectRoot
                try {
                    if ($DeleteExistingData) {
                        & $migrateScript -DeleteExistingData
                    }
                    else {
                        & $migrateScript
                    }
                }
                finally {
                    Pop-Location
                }
            }
            else {
                Write-Host "Note: Run migrate-modules.ps1 separately to apply migrations." -ForegroundColor Yellow
            }
        }
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Publish completed successfully!" -ForegroundColor Green
    Write-Host "Output: $OutputPath" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
}
catch {
    Write-Host ""
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host "Stack trace: $($_.ScriptStackTrace)" -ForegroundColor Red
    exit 1
}
finally {
    Pop-Location
}

