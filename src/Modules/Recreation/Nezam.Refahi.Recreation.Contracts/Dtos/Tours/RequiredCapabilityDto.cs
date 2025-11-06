namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Required capability information for tour participation
/// </summary>
public sealed class RequiredCapabilityDto
{
    /// <summary>
    /// Capability identifier
    /// </summary>
    public string CapabilityId { get; set; } = string.Empty;

    /// <summary>
    /// Capability display name/title (fetched from BasicDefinitions)
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

