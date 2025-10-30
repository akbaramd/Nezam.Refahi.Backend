# Identity Module Seeding Script
# This script seeds the Identity module with default roles and admin users

Write-Host "Starting Identity module seeding..." -ForegroundColor Green

try {
    # Build the project first
    Write-Host "Building the project..." -ForegroundColor Yellow
    dotnet build --configuration Release --no-restore
    
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed. Please fix compilation errors before running seeding."
    }
    
    # Run the seeding
    Write-Host "Running Identity seeding..." -ForegroundColor Yellow
    dotnet run --project src/Nezam.Refahi.WebApi --configuration Release --no-build
    
    Write-Host "Identity seeding completed successfully!" -ForegroundColor Green
}
catch {
    Write-Host "Identity seeding failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
