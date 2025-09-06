using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Settings.Domain.Entities;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Settings bounded context
/// </summary>
public class SettingsRepository : EfRepository<SettingsDbContext, Setting, Guid>, ISettingsRepository
{   
    public SettingsRepository(SettingsDbContext dbContext) : base(dbContext)
    {
    }

    #region Section Management

    public async Task<SettingsSection?> GetSectionByIdAsync(Guid id)
    {
        return await _dbContext.SettingsSections
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<SettingsSection?> GetSectionByNameAsync(string name)
    {
        return await _dbContext.SettingsSections
            .FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<IEnumerable<SettingsSection>> GetAllSectionsAsync()
    {
        return await _dbContext.SettingsSections
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<SettingsSection> AddSectionAsync(SettingsSection section)
    {
        var entity = await _dbContext.SettingsSections.AddAsync(section);
        return entity.Entity;
    }

    public Task<SettingsSection> UpdateSectionAsync(SettingsSection section)
    {
        var entity = _dbContext.SettingsSections.Update(section);
        return Task.FromResult(entity.Entity);
    }

    public async Task DeleteSectionAsync(Guid id)
    {
        var section = await GetSectionByIdAsync(id);
        if (section != null)
        {
            _dbContext.SettingsSections.Remove(section);
        }
    }

    public async Task<bool> IsSectionNameUniqueAsync(string name, Guid? excludeSectionId = null)
    {
        return !await _dbContext.SettingsSections
            .AnyAsync(s => s.Name == name && (!excludeSectionId.HasValue || s.Id != excludeSectionId.Value));
    }

    public async Task<IEnumerable<SettingsSection>> GetActiveSectionsAsync()
    {
        return await _dbContext.SettingsSections
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<bool> SectionExistsAsync(Guid id)
    {
        return await _dbContext.SettingsSections.AnyAsync(s => s.Id == id);
    }

    public async Task<bool> SectionNameExistsAsync(string name, Guid? excludeId = null)
    {
        return await _dbContext.SettingsSections
            .AnyAsync(s => s.Name == name && (!excludeId.HasValue || s.Id != excludeId.Value));
    }

    #endregion

    #region Category Management

    public async Task<SettingsCategory?> GetCategoryByIdAsync(Guid id)
    {
        return await _dbContext.SettingsCategories
            .Include(c => c.Section)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<SettingsCategory?> GetCategoryByNameAsync(string name, Guid sectionId)
    {
        return await _dbContext.SettingsCategories
            .Include(c => c.Section)
            .FirstOrDefaultAsync(c => c.Name == name && c.SectionId == sectionId);
    }

    public async Task<IEnumerable<SettingsCategory>> GetCategoriesBySectionAsync(Guid sectionId)
    {
                return await _dbContext.SettingsCategories
            .Include(c => c.Section)
            .Where(c => c.SectionId == sectionId)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<SettingsCategory>> GetAllCategoriesAsync()
    {
        return await _dbContext.SettingsCategories
            .Include(c => c.Section)
            .OrderBy(c => c.Section.DisplayOrder)
            .ThenBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<SettingsCategory> AddCategoryAsync(SettingsCategory category)
    {
        var entity = await _dbContext.SettingsCategories.AddAsync(category);
        return entity.Entity;
    }

    public Task<SettingsCategory> UpdateCategoryAsync(SettingsCategory category)
    {
        var entity = _dbContext.SettingsCategories.Update(category);
        return Task.FromResult(entity.Entity);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await GetCategoryByIdAsync(id);
        if (category != null)
        {
            _dbContext.SettingsCategories.Remove(category);
        }
    }

    public async Task<bool> IsCategoryNameUniqueAsync(string name, Guid sectionId, Guid? excludeCategoryId = null)
    {
        return !await _dbContext.SettingsCategories
            .AnyAsync(c => c.Name == name && c.SectionId == sectionId && 
                          (!excludeCategoryId.HasValue || c.Id != excludeCategoryId.Value));
    }

    public async Task<IEnumerable<SettingsCategory>> GetActiveCategoriesAsync(Guid sectionId)
    {
        return await _dbContext.SettingsCategories
            .Include(c => c.Section)
            .Where(c => c.SectionId == sectionId && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> CategoryExistsAsync(Guid id)
    {
        return await _dbContext.SettingsCategories.AnyAsync(c => c.Id == id);
    }

    public async Task<bool> CategoryNameExistsAsync(string name, Guid sectionId, Guid? excludeId = null)
    {
        return await _dbContext.SettingsCategories
            .AnyAsync(c => c.Name == name && c.SectionId == sectionId && 
                          (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    #endregion

    #region Setting Management

    public async Task<Setting?> GetSettingByIdAsync(Guid id)
    {
        return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Setting?> GetSettingByKeyAsync(string key)
    {
        return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .FirstOrDefaultAsync(s => s.Key.Value == key && s.IsActive);
    }

    public async Task<IEnumerable<Setting>> GetSettingsByCategoryAsync(Guid categoryId)
    {
        return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(s => s.CategoryId == categoryId && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Key.Value)
            .ToListAsync();
    }

    public async Task<IEnumerable<Setting>> GetSettingsBySectionAndCategoryAsync(string sectionName, string categoryName)
    {
        return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(s => s.Category.Section.Name == sectionName && 
                       s.Category.Name == categoryName && 
                       s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Key.Value)
            .ToListAsync();
    }

    public async Task<IEnumerable<Setting>> GetAllSettingsAsync()
    {
        return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Category.Section.DisplayOrder)
            .ThenBy(s => s.Category.DisplayOrder)
            .ThenBy(s => s.DisplayOrder)
            .ThenBy(s => s.Key.Value)
            .ToListAsync();
    }

    public async Task<Setting> AddSettingAsync(Setting setting)
    {
        var entity = await _dbContext.Settings.AddAsync(setting);
        return entity.Entity;
    }

    public Task<Setting> UpdateSettingAsync(Setting setting)
    {
        var entity = _dbContext.Settings.Update(setting);
        return Task.FromResult(entity.Entity);
    }

    public async Task DeleteSettingAsync(Guid id)
    {
        var setting = await GetSettingByIdAsync(id);
        if (setting != null)
        {
            _dbContext.Settings.Remove(setting);
        }
    }

    public async Task<bool> IsSettingKeyUniqueAsync(string key, Guid categoryId, Guid? excludeSettingId = null)
    {
        return !await _dbContext.Settings
            .AnyAsync(s => s.Key.Value == key && s.CategoryId == categoryId && 
                          (!excludeSettingId.HasValue || s.Id != excludeSettingId.Value));
    }

    public async Task<IEnumerable<Setting>> GetSettingsBySectionAsync(string sectionName)
    {
        return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(s => s.Category.Section.Name == sectionName && s.IsActive)
            .OrderBy(s => s.Category.DisplayOrder)
            .ThenBy(s => s.DisplayOrder)
            .ThenBy(s => s.Key.Value)
            .ToListAsync();
    }

    public async Task<IEnumerable<Setting>> GetActiveSettingsAsync(Guid categoryId)
    {
        return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(s => s.CategoryId == categoryId && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Key.Value)
            .ToListAsync();
    }

    public async Task<bool> SettingExistsAsync(Guid id)
    {
        return await _dbContext.Settings.AnyAsync(s => s.Id == id);
    }

    public async Task<bool> SettingKeyExistsAsync(string key, Guid categoryId, Guid? excludeId = null)
    {
        return await _dbContext.Settings
            .AnyAsync(s => s.Key.Value == key && s.CategoryId == categoryId && 
                          (!excludeId.HasValue || s.Id != excludeId.Value));
    }

    #endregion

    #region Bulk Operations

    public async Task<IEnumerable<Setting>> GetSettingsByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(s => ids.Contains(s.Id))
            .ToListAsync();
    }

    public void UpdateSettingsAsync(IEnumerable<Setting> settings)
    {
        _dbContext.Settings.UpdateRange(settings);
    }

    public async Task<IEnumerable<Setting>> GetSettingsByKeysAsync(IEnumerable<string> keys)
    {
            return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(s => keys.Contains(s.Key.Value) && s.IsActive)
            .OrderBy(s => s.Category.Section.DisplayOrder)
            .ThenBy(s => s.Category.DisplayOrder)
            .ThenBy(s => s.DisplayOrder)
            .ThenBy(s => s.Key.Value)
            .ToListAsync();
    }

    #endregion

    #region Search and Filtering

    public async Task<IEnumerable<Setting>> SearchSettingsAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(s => s.IsActive &&
                       (s.Key.Value.ToLower().Contains(term) ||
                        s.Description.ToLower().Contains(term) ||
                        s.Category.Name.ToLower().Contains(term) ||
                        s.Category.Section.Name.ToLower().Contains(term))
                        )
            .OrderBy(s => s.Category.Section.DisplayOrder)
            .ThenBy(s => s.Category.DisplayOrder)
            .ThenBy(s => s.DisplayOrder)
            .ThenBy(s => s.Key.Value)
            .ToListAsync();
    }

    public async Task<IEnumerable<Setting>> GetSettingsByTypeAsync(SettingType type)
    {
        return await _dbContext.Settings
            .Include(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(s => s.Value.Type == type && s.IsActive)
            .OrderBy(s => s.Category.Section.DisplayOrder)
            .ThenBy(s => s.Category.DisplayOrder)
            .ThenBy(s => s.DisplayOrder)
            .ThenBy(s => s.Key.Value)
            .ToListAsync();
    }

    public async Task<IEnumerable<SettingsSection>> SearchSectionsAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _dbContext.SettingsSections
            .Where(s => s.IsActive &&
                       (s.Name.ToLower().Contains(term) ||
                        s.Description.ToLower().Contains(term)))
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<SettingsCategory>> SearchCategoriesAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _dbContext.SettingsCategories
            .Include(c => c.Section)
            .Where(c => c.IsActive &&
                       (c.Name.ToLower().Contains(term) ||
                        c.Description.ToLower().Contains(term) ||
                        c.Section.Name.ToLower().Contains(term)))
            .OrderBy(c => c.Section.DisplayOrder)
            .ThenBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    #endregion
}