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