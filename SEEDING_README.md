# Database Seeding Guide

This document explains how to seed the database with initial data for the Nezam.Refahi.Backend project.

## Available Seeding Scripts

### 1. `seed-settings.ps1` - Seed All Data
This script seeds the complete database including:
- Admin users
- Settings sections
- Settings categories
- System settings

**Usage:**
```powershell
.\seed-settings.ps1
```

### 2. `seed-settings-only.ps1` - Seed Only Settings
This script seeds only the settings data without affecting users:
- Settings sections
- Settings categories
- System settings

**Usage:**
```powershell
.\seed-settings-only.ps1
```

### 3. `clear-and-seed-all.ps1` - Clear All and Seed Fresh
This script completely clears the database and seeds fresh data:
- **WARNING**: This will DELETE ALL existing data!
- Clears all users, settings, and other data
- Seeds fresh admin users and settings

**Usage:**
```powershell
.\clear-and-seed-all.ps1
```

## Seeding Process

### What Gets Seeded

#### Settings Sections
- **System**: System Configuration
- **Application**: Application Settings  
- **Security**: Security Configuration
- **Business**: Business Configuration

#### Settings Categories
- **General**: General System Settings (under System section)
- **AppInfo**: Application Information (under Application section)
- **Security**: Security Settings (under Security section)
- **Business**: Business Settings (under Business section)

#### System Settings
- `app_name`: Application Name (نظام رفاهی)
- `app_description`: Application Description (سیستم مدیریت رفاهی و اقامتی)
- `app_version`: Application Version (1.0.0)
- `app_build`: Application Build (2024.1)
- `app_environment`: Application Environment (Production)
- `company_name`: Company Name (شرکت نظام رفاهی)
- `company_address`: Company Address (تهران، ایران)
- `company_phone`: Company Phone (+98-21-12345678)
- `company_email`: Company Email (info@nezam-refahi.ir)
- `company_website`: Company Website (https://nezam-refahi.ir)
- `session_timeout`: Session Timeout (30 minutes)
- `max_login_attempts`: Maximum Login Attempts (5)
- `password_min_length`: Minimum Password Length (8)
- `require_2fa`: Require Two-Factor Authentication (false)
- `lockout_duration`: Account Lockout Duration (15 minutes)
- `default_currency`: Default Currency (IRR)
- `tax_rate`: Default Tax Rate (9%)
- `business_hours_start`: Business Hours Start (08:00)
- `business_hours_end`: Business Hours End (18:00)
- `support_email`: Support Email (support@nezam-refahi.ir)

#### Admin Users
- **Primary Admin**: مدیر سیستم
  - National ID: 2741153671
  - Phone: 09371770774
  - Role: Administrator
  - Phone verified: Yes

## Troubleshooting

### Common Issues

#### 1. UNIQUE constraint failed: Users.PhoneNumber
**Cause**: A user with the same phone number already exists.
**Solution**: Use `seed-settings-only.ps1` to seed only settings, or use `clear-and-seed-all.ps1` to start fresh.

#### 2. Build Errors
**Cause**: Project doesn't compile.
**Solution**: Fix compilation errors before running seeding scripts.

#### 3. Database Connection Issues
**Cause**: Database is not accessible.
**Solution**: Ensure the database is running and connection string is correct.

### Error Handling

The seeding process includes comprehensive error handling:
- Checks for existing data before seeding
- Logs all operations for debugging
- Handles database constraint violations gracefully
- Provides detailed error messages

## Manual Seeding

If you need to seed data programmatically, you can use the `DataSeeder` class:

```csharp
// Seed all data
await dataSeeder.SeedAllDataAsync();

// Seed only settings
await dataSeeder.SeedOnlySettingsAsync();

// Clear all and seed fresh
await dataSeeder.ClearAndSeedAllDataAsync();

// Validate seeding results
var validation = await dataSeeder.ValidateSeedingAsync();
```

## Database Initialization

The seeding process is automatically triggered during database initialization:
- When the application starts for the first time
- When the database is empty
- When specific data is missing (admin users, settings)

## Notes

- All setting keys use snake_case format (e.g., `app_name`, `company_address`)
- Settings are organized hierarchically: Section → Category → Setting
- The seeding process is idempotent - running it multiple times won't create duplicates
- Admin users are automatically verified and have full system access
- All seeded data includes proper audit information (CreatedAt, CreatedBy, etc.)
