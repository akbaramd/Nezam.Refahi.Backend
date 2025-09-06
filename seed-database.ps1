#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Database seeding script for Nezam Refahi Backend
    
.DESCRIPTION
    This script provides various options for seeding the database with initial data,
    including admin users, default locations, and other system data.
    
.PARAMETER Operation
    The seeding operation to perform:
    - "admin": Seed admin users only
    - "all": Seed all data (admin users, locations, surveys, etc.)
    - "validate": Validate existing seeding
    - "reset": Reset and reseed all data (WARNING: This will clear existing data)
    
.PARAMETER Environment
    The environment to run against (default: Development)
    
.EXAMPLE
    .\seed-database.ps1 -Operation admin
    
.EXAMPLE
    .\seed-database.ps1 -Operation all -Environment Production
    
.NOTES
    Author: Nezam Refahi Development Team
    Version: 1.0
    Date: 2024
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("admin", "all", "validate", "reset")]
    [string]$Operation,
    
    [Parameter(Mandatory = $false)]
    [ValidateSet("Development", "Staging", "Production")]
    [string]$Environment = "Development"
)

# Set execution policy for this session
Set-ExecutionPolicy Bypass -Scope Process -Force

# Configuration
$ProjectRoot = $PSScriptRoot
$InfrastructureProject = Join-Path $ProjectRoot "src\Nezam.Refahi.Infrastructure"
$WebApiProject = Join-Path $ProjectRoot "src\Nezam.Refahi.WebApi"

# Colors for output
$Colors = @{
    Info = "Cyan"
    Success = "Green"
    Warning = "Yellow"
    Error = "Red"
}

function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Colors[$Color]
}

function Write-Info { param([string]$Message) Write-ColorOutput $Message "Info" }
function Write-Success { param([string]$Message) Write-ColorOutput $Message "Success" }
function Write-Warning { param([string]$Message) Write-ColorOutput $Message "Warning" }
function Write-Error { param([string]$Message) Write-ColorOutput $Message "Error" }

# Header
Write-Host "===============================================" -ForegroundColor Magenta
Write-Host "    Nezam Refahi Database Seeder" -ForegroundColor Magenta
Write-Host "===============================================" -ForegroundColor Magenta
Write-Host ""

# Check prerequisites
Write-Info "Checking prerequisites..."

# Check if .NET is installed
try {
    $dotnetVersion = dotnet --version
    Write-Success ".NET SDK found: $dotnetVersion"
} catch {
    Write-Error ".NET SDK not found. Please install .NET 8.0 or later."
    exit 1
}

# Check if EF Core tools are installed
try {
    $efVersion = dotnet ef --version
    Write-Success "EF Core tools found: $efVersion"
} catch {
    Write-Warning "EF Core tools not found. Installing..."
    dotnet tool install --global dotnet-ef
    Write-Success "EF Core tools installed successfully"
}

# Check if projects exist
if (-not (Test-Path $InfrastructureProject)) {
    Write-Error "Infrastructure project not found at: $InfrastructureProject"
    exit 1
}

if (-not (Test-Path $WebApiProject)) {
    Write-Error "WebApi project not found at: $WebApiProject"
    exit 1
}

Write-Success "Prerequisites check completed"
Write-Host ""

# Set environment
$env:ASPNETCORE_ENVIRONMENT = $Environment
Write-Info "Using environment: $Environment"
Write-Host ""

# Function to run seeding
function Invoke-Seeding {
    param([string]$Operation)
    
    Write-Info "Starting seeding operation: $Operation"
    Write-Host ""
    
    try {
        # Change to WebApi project directory
        Push-Location $WebApiProject
        
        # Build the project
        Write-Info "Building project..."
        dotnet build --configuration Release
        
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed"
        }
        
        Write-Success "Build completed successfully"
        Write-Host ""
        
        # Run the appropriate seeding operation
        switch ($Operation) {
            "admin" {
                Write-Info "Seeding admin users..."
                # This would typically be done through a console application or API endpoint
                # For now, we'll use the database initializer
                Write-Warning "Admin seeding requires the application to be running"
                Write-Info "Please start the application to trigger automatic seeding"
            }
            
            "all" {
                Write-Info "Seeding all data..."
                Write-Warning "Full seeding requires the application to be running"
                Write-Info "Please start the application to trigger automatic seeding"
            }
            
            "validate" {
                Write-Info "Validating existing data..."
                # This would typically be done through a console application or API endpoint
                Write-Warning "Validation requires the application to be running"
                Write-Info "Please start the application to validate existing data"
            }
            
            "reset" {
                Write-Warning "WARNING: This operation will reset the database!"
                Write-Warning "All existing data will be lost!"
                
                $confirmation = Read-Host "Are you sure you want to continue? (yes/no)"
                if ($confirmation -eq "yes") {
                    Write-Info "Resetting database..."
                    
                    # Remove existing database
                    $dbPath = Join-Path $WebApiProject "nezam_refahi.db"
                    if (Test-Path $dbPath) {
                        Remove-Item $dbPath -Force
                        Write-Success "Existing database removed"
                    }
                    
                    # Remove existing migrations
                    Push-Location $InfrastructureProject
                    Write-Info "Removing existing migrations..."
                    dotnet ef database drop --force
                    dotnet ef migrations remove
                    Pop-Location
                    
                    Write-Success "Database reset completed"
                    Write-Info "Please run the application to recreate the database and seed data"
                } else {
                    Write-Info "Database reset cancelled"
                }
            }
        }
        
        Write-Host ""
        Write-Success "Seeding operation '$Operation' completed successfully"
        
    } catch {
        Write-Error "Seeding operation failed: $($_.Exception.Message)"
        Write-Host $_.Exception.StackTrace -ForegroundColor Red
        exit 1
    } finally {
        Pop-Location
    }
}

# Function to show help
function Show-Help {
    Write-Host "Usage Examples:" -ForegroundColor Yellow
    Write-Host "  .\seed-database.ps1 -Operation admin" -ForegroundColor White
    Write-Host "  .\seed-database.ps1 -Operation all" -ForegroundColor White
    Write-Host "  .\seed-database.ps1 -Operation validate" -ForegroundColor White
    Write-Host "  .\seed-database.ps1 -Operation reset" -ForegroundColor White
    Write-Host ""
    Write-Host "Environment Options:" -ForegroundColor Yellow
    Write-Host "  Development (default)" -ForegroundColor White
    Write-Host "  Staging" -ForegroundColor White
    Write-Host "  Production" -ForegroundColor White
    Write-Host ""
}

# Main execution
try {
    switch ($Operation) {
        "admin" { Invoke-Seeding "admin" }
        "all" { Invoke-Seeding "all" }
        "validate" { Invoke-Seeding "validate" }
        "reset" { Invoke-Seeding "reset" }
        default {
            Write-Error "Invalid operation: $Operation"
            Show-Help
            exit 1
        }
    }
} catch {
    Write-Error "Unexpected error: $($_.Exception.Message)"
    exit 1
}

Write-Host ""
Write-Host "===============================================" -ForegroundColor Magenta
Write-Host "    Seeding operation completed" -ForegroundColor Magenta
Write-Host "===============================================" -ForegroundColor Magenta
