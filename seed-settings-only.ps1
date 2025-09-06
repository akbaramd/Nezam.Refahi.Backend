# Settings-Only Seeding Script for Nezam.Refahi.Backend
# This script seeds only the database settings without touching users

Write-Host "Starting Settings-Only Seeding Process..." -ForegroundColor Green

# Navigate to the WebApi project
Set-Location "src\Nezam.Refahi.WebApi"

# Build the project first
Write-Host "Building the project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed! Please fix the build errors first." -ForegroundColor Red
    exit 1
}

# Run the application to trigger settings-only seeding
Write-Host "Running the application to trigger settings-only seeding..." -ForegroundColor Yellow
Write-Host "The application will seed only the database settings (sections, categories, system settings)." -ForegroundColor Cyan
Write-Host "Users will not be affected." -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop after seeding is complete." -ForegroundColor Cyan

dotnet run

Write-Host "Settings-only seeding completed!" -ForegroundColor Green
Write-Host "You can now check the database for the seeded settings." -ForegroundColor Cyan
Write-Host "Note: Users were not affected by this seeding process." -ForegroundColor Yellow
