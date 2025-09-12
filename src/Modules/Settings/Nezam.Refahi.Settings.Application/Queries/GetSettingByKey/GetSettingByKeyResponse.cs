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