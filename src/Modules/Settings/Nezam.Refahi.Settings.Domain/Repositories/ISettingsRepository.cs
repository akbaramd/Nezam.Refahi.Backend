using Nezam.Refahi.Settings.Domain.Entities;

namespace Nezam.Refahi.Settings.Domain.Repositories;

/// <summary>
/// Repository interface for managing settings data
/// </summary>
public interface ISettingsRepository
{
    // SettingsSection operations
    Task<SettingsSection?> GetSectionByIdAsync(Guid id);
    Task<SettingsSection?> GetSectionByNameAsync(string name);
    Task<IEnumerable<SettingsSection>> GetAllSectionsAsync();
    Task<IEnumerable<SettingsSection>> GetActiveSectionsAsync();
    Task<SettingsSection> AddSectionAsync(SettingsSection section);
    Task<SettingsSection> UpdateSectionAsync(SettingsSection section);
    Task DeleteSectionAsync(Guid id);
    Task<bool> SectionExistsAsync(Guid id);
    Task<bool> SectionNameExistsAsync(string name, Guid? excludeId = null);
    
    // SettingsCategory operations
    Task<SettingsCategory?> GetCategoryByIdAsync(Guid id);
    Task<SettingsCategory?> GetCategoryByNameAsync(string name, Guid sectionId);
    Task<IEnumerable<SettingsCategory>> GetCategoriesBySectionAsync(Guid sectionId);
    Task<IEnumerable<SettingsCategory>> GetActiveCategoriesAsync(Guid sectionId);
    Task<SettingsCategory> AddCategoryAsync(SettingsCategory category);
    Task<SettingsCategory> UpdateCategoryAsync(SettingsCategory category);
    Task DeleteCategoryAsync(Guid id);
    Task<bool> CategoryExistsAsync(Guid id);
    Task<bool> CategoryNameExistsAsync(string name, Guid sectionId, Guid? excludeId = null);
    
    // Setting operations
    Task<Setting?> GetSettingByIdAsync(Guid id);
    Task<Setting?> GetSettingByKeyAsync(string key);
    Task<IEnumerable<Setting>> GetSettingsByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Setting>> GetSettingsBySectionAsync(string sectionName);
    Task<IEnumerable<Setting>> GetActiveSettingsAsync(Guid categoryId);
    Task<IEnumerable<Setting>> GetAllSettingsAsync();
    Task<Setting> AddSettingAsync(Setting setting);
    Task<Setting> UpdateSettingAsync(Setting setting);
    Task DeleteSettingAsync(Guid id);
    Task<bool> SettingExistsAsync(Guid id);
    Task<bool> SettingKeyExistsAsync(string key, Guid categoryId, Guid? excludeId = null);
    
    // Bulk operations
    Task<IEnumerable<Setting>> GetSettingsByKeysAsync(IEnumerable<string> keys);
    Task<IEnumerable<Setting>> GetSettingsBySectionAndCategoryAsync(string sectionName, string categoryName);
    
    // Search operations
    Task<IEnumerable<Setting>> SearchSettingsAsync(string searchTerm);
    Task<IEnumerable<SettingsSection>> SearchSectionsAsync(string searchTerm);
    Task<IEnumerable<SettingsCategory>> SearchCategoriesAsync(string searchTerm);
}
