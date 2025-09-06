using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Application.Queries.GetSettingByKey;

/// <summary>
/// Response data for the GetSettingByKeyQuery
/// </summary>
public class GetSettingByKeyResponse
{
    /// <summary>
    /// The setting information
    /// </summary>
    public SettingDetailDto? Setting { get; set; }
    
    /// <summary>
    /// Whether the setting was found
    /// </summary>
    public bool Found { get; set; }
}

/// <summary>
/// Detailed DTO for a setting
/// </summary>
public class SettingDetailDto
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
    
    /// <summary>
    /// The category information
    /// </summary>
    public CategoryDetailDto Category { get; set; } = new();
    
    /// <summary>
    /// The section information
    /// </summary>
    public SectionDetailDto Section { get; set; } = new();
    
    /// <summary>
    /// When the setting was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When the setting was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }
    
    /// <summary>
    /// The typed value based on the setting type
    /// </summary>
    public object? TypedValue { get; set; }
}

/// <summary>
/// Detailed category information
/// </summary>
public class CategoryDetailDto
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
}

/// <summary>
/// Detailed section information
/// </summary>
public class SectionDetailDto
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
}
