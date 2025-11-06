@echo off
REM Publish script for Nezam.Refahi.WebApi
REM This is a batch file wrapper for PowerShell script

setlocal

set SCRIPT_DIR=%~dp0
set PROJECT_ROOT=%SCRIPT_DIR%..
set OUTPUT_PATH=%1

if "%OUTPUT_PATH%"=="" set OUTPUT_PATH=%PROJECT_ROOT%\publish

echo ========================================
echo Nezam.Refahi.WebApi Publish Script
echo ========================================
echo.
echo Output Path: %OUTPUT_PATH%
echo.

REM Check if PowerShell is available
powershell -Command "Get-Command powershell" >nul 2>&1
if errorlevel 1 (
    echo Error: PowerShell is not available
    exit /b 1
)

REM Run the PowerShell script
powershell -ExecutionPolicy Bypass -File "%SCRIPT_DIR%publish-webapi.ps1" -OutputPath "%OUTPUT_PATH%" -Configuration Release -CleanOutput -ApplyMigrations

if errorlevel 1 (
    echo.
    echo Publish failed!
    exit /b 1
)

echo.
echo ========================================
echo Publish completed successfully!
echo ========================================
pause

