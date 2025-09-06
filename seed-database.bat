@echo off
setlocal enabledelayedexpansion

REM Database seeding script for Nezam Refahi Backend
REM This script provides various options for seeding the database with initial data

echo ===============================================
echo     Nezam Refahi Database Seeder
echo ===============================================
echo.

REM Check if operation parameter is provided
if "%1"=="" (
    echo Usage: seed-database.bat [operation]
    echo.
    echo Operations:
    echo   admin     - Seed admin users only
    echo   all       - Seed all data
    echo   validate  - Validate existing seeding
    echo   reset     - Reset and reseed all data
    echo.
    echo Examples:
    echo   seed-database.bat admin
    echo   seed-database.bat all
    echo   seed-database.bat validate
    echo   seed-database.bat reset
    echo.
    pause
    exit /b 1
)

set OPERATION=%1
set PROJECT_ROOT=%~dp0
set INFRASTRUCTURE_PROJECT=%PROJECT_ROOT%src\Nezam.Refahi.Infrastructure
set WEBAPI_PROJECT=%PROJECT_ROOT%src\Nezam.Refahi.WebApi

REM Check if projects exist
if not exist "%INFRASTRUCTURE_PROJECT%" (
    echo Error: Infrastructure project not found at: %INFRASTRUCTURE_PROJECT%
    pause
    exit /b 1
)

if not exist "%WEBAPI_PROJECT%" (
    echo Error: WebApi project not found at: %WEBAPI_PROJECT%
    pause
    exit /b 1
)

echo Starting seeding operation: %OPERATION%
echo.

REM Change to WebApi project directory
cd /d "%WEBAPI_PROJECT%"

REM Build the project
echo Building project...
dotnet build --configuration Release

if %ERRORLEVEL% neq 0 (
    echo Error: Build failed
    pause
    exit /b 1
)

echo Build completed successfully
echo.

REM Run the appropriate seeding operation
if "%OPERATION%"=="admin" (
    echo Seeding admin users...
    echo Admin seeding requires the application to be running
    echo Please start the application to trigger automatic seeding
) else if "%OPERATION%"=="all" (
    echo Seeding all data...
    echo Full seeding requires the application to be running
    echo Please start the application to trigger automatic seeding
) else if "%OPERATION%"=="validate" (
    echo Validating existing data...
    echo Validation requires the application to be running
    echo Please start the application to validate existing data
) else if "%OPERATION%"=="reset" (
    echo WARNING: This operation will reset the database!
    echo All existing data will be lost!
    echo.
    set /p CONFIRMATION="Are you sure you want to continue? (yes/no): "
    
    if /i "!CONFIRMATION!"=="yes" (
        echo Resetting database...
        
        REM Remove existing database
        if exist "nezam_refahi.db" (
            del "nezam_refahi.db"
            echo Existing database removed
        )
        
        REM Remove existing migrations
        cd /d "%INFRASTRUCTURE_PROJECT%"
        echo Removing existing migrations...
        dotnet ef database drop --force
        dotnet ef migrations remove
        
        echo Database reset completed
        echo Please run the application to recreate the database and seed data
    ) else (
        echo Database reset cancelled
    )
) else (
    echo Invalid operation: %OPERATION%
    echo.
    echo Valid operations: admin, all, validate, reset
    pause
    exit /b 1
)

echo.
echo ===============================================
echo     Seeding operation completed
echo ===============================================
echo.

pause
