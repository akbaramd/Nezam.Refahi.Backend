using Nezam.Refahi.Settings.Domain.Entities;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Domain.Services;

/// <summary>
/// Domain service for business logic that doesn't naturally belong to a single entity
/// This service is stateless and focuses on cross-entity business rules
/// </summary>
public class SettingsDomainService
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ISettingChangeEventRepository _changeEventRepository;

    public SettingsDomainService(
        ISettingsRepository settingsRepository,
        ISettingChangeEventRepository changeEventRepository)
    {
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _changeEventRepository = changeEventRepository ?? throw new ArgumentNullException(nameof(changeEventRepository));
    }

    /// <summary>
    /// Validates if a setting key is unique within its category
    /// This business rule involves coordination between multiple entities
    /// </summary>
    public async Task<bool> IsSettingKeyUniqueAsync(string key, Guid categoryId, Guid? excludeSettingId = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Setting key cannot be empty", nameof(key));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID cannot be empty", nameof(categoryId));

        return await _settingsRepository.SettingKeyExistsAsync(key, categoryId, excludeSettingId);
    }

    /// <summary>
    /// Validates if a section name is unique across the system
    /// This business rule involves coordination between multiple entities
    /// </summary>
    public async Task<bool> IsSectionNameUniqueAsync(string name, Guid? excludeSectionId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Section name cannot be empty", nameof(name));

        return await _settingsRepository.SectionNameExistsAsync(name, excludeSectionId);
    }

    /// <summary>
    /// Validates if a category name is unique within its section
    /// This business rule involves coordination between multiple entities
    /// </summary>
    public async Task<bool> IsCategoryNameUniqueAsync(string name, Guid sectionId, Guid? excludeCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        if (sectionId == Guid.Empty)
            throw new ArgumentException("Section ID cannot be empty", nameof(sectionId));

        return await _settingsRepository.CategoryNameExistsAsync(name, sectionId, excludeCategoryId);
    }

    /// <summary>
    /// Validates business rules for setting value changes
    /// This involves complex validation logic that spans multiple entities
    /// </summary>
    public async Task<ValidationResult> ValidateSettingValueChangeAsync(Guid settingId, SettingValue newValue, Guid userId)
    {
        var result = new ValidationResult();

        if (settingId == Guid.Empty)
            return result.AddError("Setting ID cannot be empty");

        if (newValue == null)
            return result.AddError("New value cannot be null");

        if (userId == Guid.Empty)
            return result.AddError("User ID cannot be empty");

        // Get the current setting to validate business rules
        var currentSetting = await _settingsRepository.GetSettingByIdAsync(settingId);
        if (currentSetting == null)
            return result.AddError("Setting not found");

        // Check if setting is read-only
        if (currentSetting.IsReadOnly)
            return result.AddError("Cannot update read-only setting");

        // Check if setting is active
        if (!currentSetting.IsActive)
            return result.AddError("Cannot update inactive setting");

        // Validate that the new value type matches the current setting type
        if (currentSetting.Value.Type != newValue.Type)
            return result.AddError($"Setting type mismatch. Expected: {currentSetting.Value.Type}, Got: {newValue.Type}");

        // Additional business rule: Check if the value is actually changing
        if (currentSetting.Value.RawValue == newValue.RawValue)
            result.AddWarning("Setting value is not changing");

        return result;
    }

    /// <summary>
    /// Applies business rules for setting value changes
    /// This involves coordination between setting and change event entities
    /// </summary>
    public async Task<SettingChangeEvent> ApplySettingValueChangeAsync(Setting setting, SettingValue newValue, Guid userId, string? changeReason = null)
    {
        if (setting == null)
            throw new ArgumentNullException(nameof(setting));

        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        // Validate the change
        var validationResult = await ValidateSettingValueChangeAsync(setting.Id, newValue, userId);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors);
            throw new InvalidOperationException($"Setting value change validation failed: {errors}");
        }

        // Create change event
        var changeEvent = new SettingChangeEvent(
            setting.Id,
            setting.Key,
            setting.Value,
            newValue,
            userId,
            changeReason
        );

        // Update the setting value
        setting.UpdateValue(newValue, userId, changeReason);

        // Add change event to the setting's collection
        setting.ChangeEvents.Add(changeEvent);

        return changeEvent;
    }

    /// <summary>
    /// Validates business rules for section/category hierarchy changes
    /// This involves complex validation logic that spans multiple entities
    /// </summary>
    public async Task<ValidationResult> ValidateHierarchyChangeAsync(Guid sectionId, Guid? categoryId = null)
    {
        var result = new ValidationResult();

        if (sectionId == Guid.Empty)
            return result.AddError("Section ID cannot be empty");

        // Check if section exists and is active
        var section = await _settingsRepository.GetSectionByIdAsync(sectionId);
        if (section == null)
            return result.AddError("Section not found");

        if (!section.IsActive)
            return result.AddError("Cannot modify inactive section");

        // If category ID is provided, validate it
        if (categoryId.HasValue && categoryId.Value != Guid.Empty)
        {
            var category = await _settingsRepository.GetCategoryByIdAsync(categoryId.Value);
            if (category == null)
                return result.AddError("Category not found");

            if (category.SectionId != sectionId)
                return result.AddError("Category does not belong to the specified section");

            if (!category.IsActive)
                return result.AddError("Cannot modify inactive category");
        }

        return result;
    }

    /// <summary>
    /// Applies business rules for section/category hierarchy changes
    /// This involves coordination between multiple entities
    /// </summary>
    public async Task ApplyHierarchyChangeAsync(SettingsSection section, SettingsCategory? category = null)
    {
        if (section == null)
            throw new ArgumentNullException(nameof(section));

        // Validate the hierarchy change
        var validationResult = await ValidateHierarchyChangeAsync(section.Id, category?.Id);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors);
            throw new InvalidOperationException($"Hierarchy change validation failed: {errors}");
        }

        // Apply business rules for hierarchy changes
        // This could involve updating display orders, validating dependencies, etc.
        
        if (category != null)
        {
            // Ensure category belongs to this section
            if (category.SectionId != section.Id)
                throw new InvalidOperationException("Category does not belong to the specified section");

            // Update category's section reference if needed
            // This is an example of cross-entity business logic
        }
    }

    /// <summary>
    /// Validates business rules for bulk operations
    /// This involves complex validation logic that spans multiple entities
    /// </summary>
    public async Task<ValidationResult> ValidateBulkSettingsUpdateAsync(Dictionary<Guid, string> settingUpdates, Guid userId)
    {
        var result = new ValidationResult();

        if (settingUpdates == null || !settingUpdates.Any())
            return result.AddError("No settings to update");

        if (userId == Guid.Empty)
            return result.AddError("User ID cannot be empty");

        // Validate each setting update
        foreach (var update in settingUpdates)
        {
            if (update.Key == Guid.Empty)
                result.AddError("Invalid setting ID in bulk update");

            if (string.IsNullOrWhiteSpace(update.Value))
                result.AddError($"Empty value for setting {update.Key}");

            // Check if setting exists and can be updated
            var setting = await _settingsRepository.GetSettingByIdAsync(update.Key);
            if (setting == null)
                result.AddError($"Setting {update.Key} not found");
            else if (setting.IsReadOnly)
                result.AddError($"Setting {setting.Key.Value} is read-only and cannot be updated");
        }

        return result;
    }

    /// <summary>
    /// Applies business rules for bulk settings updates
    /// This involves coordination between multiple entities
    /// </summary>
    public async Task<List<SettingChangeEvent>> ApplyBulkSettingsUpdateAsync(Dictionary<Guid, string> settingUpdates, Guid userId, string? changeReason = null)
    {
        // Validate the bulk update
        var validationResult = await ValidateBulkSettingsUpdateAsync(settingUpdates, userId);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors);
            throw new InvalidOperationException($"Bulk settings update validation failed: {errors}");
        }

        var changeEvents = new List<SettingChangeEvent>();

        // Apply updates to each setting
        foreach (var update in settingUpdates)
        {
            var setting = await _settingsRepository.GetSettingByIdAsync(update.Key);
            if (setting != null)
            {
                // Create new setting value with the same type
                var newValue = new SettingValue(update.Value, setting.Value.Type);
                
                // Apply the change
                var changeEvent = await ApplySettingValueChangeAsync(setting, newValue, userId, changeReason);
                changeEvents.Add(changeEvent);
            }
        }

        return changeEvents;
    }
}
