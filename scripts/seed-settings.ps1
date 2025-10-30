# Settings Seeding Script for Nezam.Refahi.Backend
# This script seeds the database with default settings

Write-Host "Starting Settings Seeding Process..." -ForegroundColor Green

# Navigate to the WebApi project
Set-Location "src\Nezam.Refahi.WebApi"

# Build the project first
Write-Host "Building the project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed! Please fix the build errors first." -ForegroundColor Red
    exit 1
}

# Run the application to trigger seeding
Write-Host "Running the application to trigger seeding..." -ForegroundColor Yellow
Write-Host "The application will seed the database with default settings." -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop after seeding is complete." -ForegroundColor Cyan

dotnet run

Write-Host "Settings seeding completed!" -ForegroundColor Green
Write-Host "You can now check the database for the seeded settings." -ForegroundColor Cyan
