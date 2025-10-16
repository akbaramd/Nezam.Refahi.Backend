#!/usr/bin/env pwsh
# Survey Module Test Runner
# Comprehensive test execution with coverage and reporting

param(
    [string]$TestCategory = "All",
    [string]$OutputFormat = "trx",
    [string]$CoverageFormat = "cobertura",
    [switch]$Verbose,
    [switch]$Coverage,
    [switch]$Performance
)

Write-Host "🧪 Survey Module Test Runner" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

# Set test project path
$TestProjectPath = "src/Modules/Survey/Nezam.Refahi.Surveying.Tests/Nezam.Refahi.Surveying.Tests.csproj"

# Check if test project exists
if (-not (Test-Path $TestProjectPath)) {
    Write-Error "Test project not found at: $TestProjectPath"
    exit 1
}

# Build the solution first
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet build --configuration Release --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}

# Prepare test command
$TestCommand = "dotnet test `"$TestProjectPath`" --configuration Release --no-build"

# Add verbosity
if ($Verbose) {
    $TestCommand += " --verbosity normal"
} else {
    $TestCommand += " --verbosity minimal"
}

# Add test category filter
switch ($TestCategory) {
    "Unit" { $TestCommand += " --filter Category=Unit" }
    "Integration" { $TestCommand += " --filter Category=Integration" }
    "Performance" { $TestCommand += " --filter Category=Performance" }
    "Security" { $TestCommand += " --filter Category=Security" }
    "Domain" { $TestCommand += " --filter Namespace~Domain" }
    "Application" { $TestCommand += " --filter Namespace~Application" }
    "All" { }
    default { 
        Write-Warning "Unknown test category: $TestCategory. Running all tests."
    }
}

# Add coverage collection
if ($Coverage) {
    Write-Host "📊 Collecting code coverage..." -ForegroundColor Green
    $TestCommand += " --collect:`"XPlat Code Coverage`""
}

# Add output format
$TestCommand += " --logger trx --results-directory TestResults"

# Run tests
Write-Host "🚀 Running tests..." -ForegroundColor Green
Write-Host "Command: $TestCommand" -ForegroundColor Gray

Invoke-Expression $TestCommand
$TestExitCode = $LASTEXITCODE

# Generate test report
if (Test-Path "TestResults") {
    Write-Host "📋 Test Results Summary:" -ForegroundColor Cyan
    
    # Count test results
    $TrxFiles = Get-ChildItem "TestResults" -Filter "*.trx" -Recurse
    if ($TrxFiles.Count -gt 0) {
        Write-Host "✅ Test result files generated: $($TrxFiles.Count)" -ForegroundColor Green
    }
    
    # Coverage report
    if ($Coverage) {
        $CoverageFiles = Get-ChildItem "TestResults" -Filter "coverage.cobertura.xml" -Recurse
        if ($CoverageFiles.Count -gt 0) {
            Write-Host "📊 Coverage report generated" -ForegroundColor Green
            
            # Generate HTML coverage report if reportgenerator is available
            if (Get-Command "reportgenerator" -ErrorAction SilentlyContinue) {
                Write-Host "🌐 Generating HTML coverage report..." -ForegroundColor Yellow
                reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html"
                Write-Host "📁 Coverage report available at: TestResults/CoverageReport/index.html" -ForegroundColor Green
            }
        }
    }
}

# Performance tests
if ($Performance) {
    Write-Host "⚡ Running performance tests..." -ForegroundColor Magenta
    dotnet test "$TestProjectPath" --filter Category=Performance --configuration Release --no-build --logger trx --results-directory TestResults/Performance
}

# Summary
Write-Host "`n📊 Test Execution Summary:" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan

if ($TestExitCode -eq 0) {
    Write-Host "✅ All tests passed!" -ForegroundColor Green
} else {
    Write-Host "❌ Some tests failed!" -ForegroundColor Red
}

Write-Host "📁 Test results saved to: TestResults/" -ForegroundColor Blue
Write-Host "🔍 View detailed results in TestResults/*.trx files" -ForegroundColor Blue

if ($Coverage) {
    Write-Host "📊 Coverage data collected" -ForegroundColor Blue
}

Write-Host "`n🎯 Test Categories Available:" -ForegroundColor Yellow
Write-Host "- Unit: Domain and application unit tests" -ForegroundColor White
Write-Host "- Integration: Integration and repository tests" -ForegroundColor White
Write-Host "- Performance: Performance and load tests" -ForegroundColor White
Write-Host "- Security: Security and authorization tests" -ForegroundColor White
Write-Host "- Domain: Domain layer tests only" -ForegroundColor White
Write-Host "- Application: Application layer tests only" -ForegroundColor White
Write-Host "- All: Run all tests (default)" -ForegroundColor White

Write-Host "`n💡 Usage Examples:" -ForegroundColor Yellow
Write-Host ".\run-tests.ps1 -TestCategory Unit -Coverage" -ForegroundColor White
Write-Host ".\run-tests.ps1 -TestCategory Domain -Verbose" -ForegroundColor White
Write-Host ".\run-tests.ps1 -TestCategory All -Coverage -Performance" -ForegroundColor White

exit $TestExitCode
