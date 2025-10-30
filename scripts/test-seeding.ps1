# Test script to verify Identity seeding works correctly
Write-Host "Testing Identity module seeding..." -ForegroundColor Green

# Build the project first
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Build successful!" -ForegroundColor Green

# Run the application briefly to test seeding
Write-Host "Starting application to test seeding..." -ForegroundColor Yellow
Write-Host "The application will start and run the seeding process automatically." -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop after seeding is complete." -ForegroundColor Cyan

dotnet run --project src/Nezam.Refahi.WebApi/Nezam.Refahi.WebApi.csproj
