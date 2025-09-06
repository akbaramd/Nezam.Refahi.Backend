# Settings Seeding

This module provides automatic seeding functionality for settings data in the Nezam Refahi application.

## Features

- **Automatic Seeding**: Settings are automatically seeded when the application starts
- **Idempotent**: Safe to run multiple times - won't create duplicates
- **Manual Seeding**: Can be triggered manually via console commands
- **Status Checking**: Check if settings are already seeded

## Usage

### Automatic Seeding

Settings are automatically seeded when the application starts via the `SettingsSeedingService` hosted service.

### Manual Seeding

#### Using Service Provider

```csharp
// In your application startup or console command
await serviceProvider.SeedSettingsAsync();
```

#### Using DbContext

```csharp
// Direct seeding using DbContext
await context.SeedSettingsAsync();
```

#### Console Command

```bash
# Seed settings (if not already seeded)
dotnet run -- seed-settings

# Force re-seed (even if already seeded)
dotnet run -- seed-settings --force
```

### Check Seeding Status

```csharp
// Check if settings are seeded
bool isSeeded = await context.IsSettingsSeededAsync();

// Get detailed seeding status
var status = await context.GetSeedingStatusAsync();
Console.WriteLine($"Seeded: {status.IsSeeded}");
Console.WriteLine($"Sections: {status.SectionsCount}");
Console.WriteLine($"Categories: {status.CategoriesCount}");
Console.WriteLine($"Settings: {status.SettingsCount}");
```

## Seeded Data

### Settings Sections

- **WebApp**: تنظیمات وب اپلیکیشن
- **Webhooks**: تنظیمات اتصال به سرویس‌های خارجی
- **Authentication**: تنظیمات سیستم احراز هویت و امنیت
- **Email**: تنظیمات ارسال ایمیل
- **SMS**: تنظیمات ارسال پیامک

### Settings Categories

- **General**: تنظیمات عمومی
- **Security**: تنظیمات امنیتی
- **Integration**: تنظیمات یکپارچه‌سازی با سرویس‌های خارجی
- **Communication**: تنظیمات ارتباطات (ایمیل، پیامک)

### Default Settings

#### WebApp Settings

- `WEBAPP_NAME`: "نظام رفاهی"
- `WEBAPP_DESCRIPTION`: "سیستم مدیریت رفاهی کارکنان"
- `WEBAPP_LOGO`: "/images/logo.png"
- `WEBAPP_VERSION`: "1.0.0"

#### Webhook Settings

- `WEBHOOK_ENGINEER_MEMBER_SERVICE_URL`: "" (empty by default)

## Configuration

The seeding service is automatically registered in the `NezamRefahiSettingsInfrastructureModule`:

```csharp
// Register settings seeding service
context.Services.AddHostedService<SettingsSeedingService>();
```

## Logging

The seeding process logs all operations:

- Information about added sections, categories, and settings
- Debug messages for existing data (skipped)
- Error messages for any failures

## Error Handling

- Database connection errors are logged and re-thrown
- Individual seeding failures are logged but don't stop the process
- The service is idempotent - safe to run multiple times

## Customization

To add more settings or modify existing ones:

1. Update the constants in `SettingsConstants.cs`
2. Update the default values in `SettingsDefaultValues.cs`
3. Modify the seeding logic in `SettingsSeedingService.cs`
4. Add new sections/categories as needed

## Dependencies

- Entity Framework Core
- Microsoft.Extensions.Hosting
- Microsoft.Extensions.Logging
- Microsoft.Extensions.DependencyInjection
