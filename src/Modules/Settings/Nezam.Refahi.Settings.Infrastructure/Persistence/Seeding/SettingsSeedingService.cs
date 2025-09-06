using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Settings.Domain.Entities;
using Nezam.Refahi.Settings.Domain.ValueObjects;
using Nezam.Refahi.Settings.Infrastructure.Persistence;
using System.Reflection;
using Nezam.Refahi.Settings.Contracts.Constants;

namespace Nezam.Refahi.Settings.Infrastructure.Persistence.Seeding;

/// <summary>
/// Service for seeding default settings data
/// </summary>
public class SettingsSeedingService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SettingsSeedingService> _logger;
    
    // System user ID for seeding operations


    public SettingsSeedingService(IServiceProvider serviceProvider, ILogger<SettingsSeedingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SettingsDbContext>();

        try
        {
            _logger.LogInformation("Starting settings seeding process...");

            // Ensure database is created
            await context.Database.EnsureCreatedAsync(cancellationToken);

            // Seed settings sections
            await SeedSettingsSectionsAsync(context, cancellationToken);

            // Seed settings categories
            await SeedSettingsCategoriesAsync(context, cancellationToken);

            // Seed default settings
            await SeedDefaultSettingsAsync(context, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Settings seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during settings seeding");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }


    

    private async Task SeedSettingsSectionsAsync(SettingsDbContext context, CancellationToken cancellationToken)
    {
        var sections = new[]
        {
            new SettingsSection("WebApp", "تنظیمات عمومی وب اپلیکیشن", 1),
            new SettingsSection("Webhooks", "تنظیمات اتصال به سرویس‌های خارجی", 2),
        };

        foreach (var section in sections)
        {
            var existingSection = await context.SettingsSections
                .FirstOrDefaultAsync(s => s.Name == section.Name, cancellationToken);

            if (existingSection == null)
            {
                context.SettingsSections.Add(section);
                _logger.LogInformation("Added settings section: {SectionName}", section.Name);
            }
            else
            {
                _logger.LogDebug("Settings section already exists: {SectionName}", section.Name);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedSettingsCategoriesAsync(SettingsDbContext context, CancellationToken cancellationToken)
    {
        // Get sections first
        var webAppSection = await context.SettingsSections
            .FirstOrDefaultAsync(s => s.Name == "WebApp", cancellationToken);
        var webhooksSection = await context.SettingsSections
            .FirstOrDefaultAsync(s => s.Name == "Webhooks", cancellationToken);

        var categories = new List<SettingsCategory>();

        if (webAppSection != null)
        {
            categories.Add(new SettingsCategory("General", "تنظیمات عمومی", webAppSection.Id, 1));
        }


        if (webhooksSection != null)
        {
            categories.Add(new SettingsCategory("Integration", "تنظیمات یکپارچه‌سازی با سرویس‌های خارجی", webhooksSection.Id, 1));
        }


        foreach (var category in categories)
        {
            var existingCategory = await context.SettingsCategories
                .FirstOrDefaultAsync(c => c.Name == category.Name, cancellationToken);

            if (existingCategory == null)
            {
                context.SettingsCategories.Add(category);
                _logger.LogInformation("Added settings category: {CategoryName}", category.Name);
            }
            else
            {
                _logger.LogDebug("Settings category already exists: {CategoryName}", category.Name);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedDefaultSettingsAsync(SettingsDbContext context, CancellationToken cancellationToken)
    {
        // Get sections and categories for reference
        var webAppSection = await context.SettingsSections
            .FirstOrDefaultAsync(s => s.Name == "WebApp", cancellationToken);
        var webhooksSection = await context.SettingsSections
            .FirstOrDefaultAsync(s => s.Name == "Webhooks", cancellationToken);

        var generalCategory = await context.SettingsCategories
            .FirstOrDefaultAsync(c => c.Name == "General", cancellationToken);
        var integrationCategory = await context.SettingsCategories
            .FirstOrDefaultAsync(c => c.Name == "Integration", cancellationToken);

        // Define settings as array
        var settingsToSeed = new[]
        {
            // WebApp Settings
            new
            {
                Key = SettingsConstants.WebApp.Name,
                Value = SettingsDefaultValues.WebApp.Name,
                Description = "نام نمایشی وب اپلیکیشن",
                Category = "General",
                IsReadOnly = false,
                DisplayOrder = 1
            },
            new
            {
                Key = SettingsConstants.WebApp.Description,
                Value = SettingsDefaultValues.WebApp.Description,
                Description = "توضیحات کوتاه درباره اپلیکیشن",
                Category = "General",
                IsReadOnly = false,
                DisplayOrder = 2
            },
            new
            {
                Key = SettingsConstants.WebApp.Logo,
                Value = SettingsDefaultValues.WebApp.Logo,
                Description = "مسیر فایل لوگو اپلیکیشن",
                Category = "General",
                IsReadOnly = false,
                DisplayOrder = 3
            },
            new
            {
                Key = SettingsConstants.WebApp.Version,
                Value = SettingsDefaultValues.WebApp.Version,
                Description = "نسخه فعلی اپلیکیشن",
                Category = "General",
                IsReadOnly = true,
                DisplayOrder = 4
            },
            // Webhook Settings
            new
            {
                Key = SettingsConstants.Webhooks.EngineerMemberServiceUrl,
                Value = SettingsDefaultValues.Webhooks.EngineerMemberServiceUrl,
                Description = "آدرس وب‌سرویس برای جستجوی اعضا بر اساس کد ملی",
                Category = "Integration",
                IsReadOnly = false,
                DisplayOrder = 1
            }
        };

        // Process each setting
        foreach (var settingData in settingsToSeed)
        {
            var existingSetting = await context.Settings
                .FirstOrDefaultAsync(s => s.Key.Value == settingData.Key, cancellationToken);

            if (existingSetting == null)
            {
                // Get category ID
                Guid categoryId = Guid.Empty;
                if (settingData.Category == "General" && generalCategory != null)
                    categoryId = generalCategory.Id;
                else if (settingData.Category == "Integration" && integrationCategory != null)
                    categoryId = integrationCategory.Id;

                if (categoryId != Guid.Empty)
                {
                    // Create new setting
                    var setting = new Setting(
                        new SettingKey(settingData.Key),
                        new SettingValue(settingData.Value, SettingType.String),
                        settingData.Description,
                        categoryId,
                        settingData.IsReadOnly,
                        settingData.DisplayOrder
                    );

                    // Set audit properties for the setting
                    setting.MarkModified("System:Seed");
                    
                    context.Settings.Add(setting);
                    _logger.LogInformation("Added setting: {SettingKey} = {SettingValue}", settingData.Key, settingData.Value);
                }
                else
                {
                    _logger.LogWarning("Category not found for setting: {SettingKey}", settingData.Key);
                }
            }
            else
            {
                _logger.LogDebug("Setting already exists: {SettingKey}", settingData.Key);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
