using Nezam.Refahi.Settings.Contracts;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Application.Services;

public class SettingService(ISettingsRepository settingsRepository) : ISettingsService
{
    

   
    
    public async Task<SettingValue?> GetSettingValueAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        var setting = await settingsRepository.GetSettingByKeyAsync(key);
        return setting?.Value;
    }

    public async Task<string> GetSettingValueAsync(string key, string defaultValue)
    {
        var settingValue = await GetSettingValueAsync(key);
        return settingValue?.RawValue ?? defaultValue;
    }

    public async Task<T> GetSettingValueAsync<T>(string key)
    {
        var settingValue = await GetSettingValueAsync(key);
        if (settingValue == null)
            return default(T)!;

        try
        {
            return settingValue.GetTypedValue<T>();
        }
        catch
        {
            return default(T)!;
        }
    }

    public async Task<T> GetSettingValueAsync<T>(string key, T defaultValue)
    {
        var settingValue = await GetSettingValueAsync(key);
        if (settingValue == null)
            return defaultValue;

        try
        {
            return settingValue.GetTypedValue<T>();
        }
        catch
        {
            return defaultValue;
        }
    }

    public async Task<Dictionary<string, string>> GetSettingsBySectionAsync(string sectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
            return new Dictionary<string, string>();

        var settings = await settingsRepository.GetSettingsBySectionAsync(sectionName);
        return settings
            .Where(s => s.IsActive)
            .ToDictionary(s => s.Key.Value, s => s.Value.RawValue);
    }

    public async Task<bool> SettingExistsAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        var setting = await settingsRepository.GetSettingByKeyAsync(key);
        return setting != null && setting.IsActive;
    }
}
