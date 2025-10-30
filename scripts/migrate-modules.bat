@echo off
REM Simple migration command for Membership and BasicDefinitions modules
REM This batch file provides easy access to migration commands

echo ========================================
echo Nezam.Refahi Migration Tool
echo ========================================
echo.

REM Check if PowerShell is available
powershell -Command "Get-Host" >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: PowerShell is not available
    pause
    exit /b 1
)

REM Run the PowerShell migration script
echo Running migration script...
powershell -ExecutionPolicy Bypass -File "migrate-modules.ps1" %*

echo.
echo Migration process completed.
pause
