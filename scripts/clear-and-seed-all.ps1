# Clear and Seed All Data Script for Nezam.Refahi.Backend
# This script clears all data and seeds fresh

Write-Host "Starting Clear and Seed All Data Process..." -ForegroundColor Green
Write-Host "WARNING: This will DELETE ALL existing data!" -ForegroundColor Red
Write-Host "Are you sure you want to continue? (y/N)" -ForegroundColor Yellow

$response = Read-Host
if ($response -ne "y" -and $response -ne "Y") {
    Write-Host "Operation cancelled by user." -ForegroundColor Yellow
    exit 0
}

# Navigate to the WebApi project
Set-Location "src\Nezam.Refahi.WebApi"

# Build the project first
Write-Host "Building the project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed! Please fix the build errors first." -ForegroundColor Red
    exit 1
}

# Run the application to trigger clear and seed
Write-Host "Running the application to clear all data and seed fresh..." -ForegroundColor Yellow
Write-Host "The application will:" -ForegroundColor Cyan
Write-Host "1. Clear ALL existing data (users, settings, etc.)" -ForegroundColor Cyan
Write-Host "2. Seed fresh admin users and settings" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop after seeding is complete." -ForegroundColor Cyan

dotnet run

Write-Host "Clear and seed all data completed!" -ForegroundColor Green
Write-Host "All data has been cleared and fresh data has been seeded." -ForegroundColor Cyan
Write-Host "You can now check the database for the fresh data." -ForegroundColor Cyan
