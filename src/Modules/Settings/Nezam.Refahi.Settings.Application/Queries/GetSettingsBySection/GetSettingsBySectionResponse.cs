using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Application.Queries.GetSettingsBySection;

/// <summary>
/// Response data for the GetSettingsBySectionQuery
/// </summary>
public class GetSettingsBySectionResponse
{
    /// <summary>
    /// The list of sections with their categories and settings
    /// </summary>
    public List<SectionWithSettingsDto> Sections { get; set; } = new();
    
    /// <summary>
    /// Total number of sections
    /// </summary>
    public int TotalSections { get; set; }
    
    /// <summary>
    /// Total number of categories
    /// </summary>
    public int TotalCategories { get; set; }
    
    /// <summary>
    /// Total number of settings
    /// </summary>
    public int TotalSettings { get; set; }
}

/// <summary>
/// DTO for a section with its categories and settings
/// </summary>
public class SectionWithSettingsDto
{
    /// <summary>
    /// The section ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The section name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The section description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The display order
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Whether the section is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// The categories in this section
    /// </summary>
    public List<CategoryWithSettingsDto> Categories { get; set; } = new();
    
    /// <summary>
    /// The number of categories in this section
    /// </summary>
    public int CategoryCount { get; set; }
    
    /// <summary>
    /// The total number of settings in this section
    /// </summary>
    public int TotalSettings { get; set; }
}

/// <summary>
/// DTO for a category with its settings
/// </summary>
public class CategoryWithSettingsDto
{
    /// <summary>
    /// The category ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The category name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The category description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The display order
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Whether the category is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// The settings in this category
    /// </summary>
    public List<SimpleSettingDto> Settings { get; set; } = new();
    
    /// <summary>
    /// The number of settings in this category
    /// </summary>
    public int SettingCount { get; set; }
}

/// <summary>
/// Simple DTO for a setting
/// </summary>
public class SimpleSettingDto
{
    /// <summary>
    /// The setting ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The setting key
    /// </summary>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// The setting value
    /// </summary>
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// The setting type
    /// </summary>
    public SettingType Type { get; set; }
    
    /// <summary>
    /// The setting description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the setting is read-only
    /// </summary>
    public bool IsReadOnly { get; set; }
    
    /// <summary>
    /// Whether the setting is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// The display order
    /// </summary>
    public int DisplayOrder { get; set; }
}
