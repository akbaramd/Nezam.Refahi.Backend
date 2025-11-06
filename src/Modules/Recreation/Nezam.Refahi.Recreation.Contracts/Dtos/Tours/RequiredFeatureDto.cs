namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Required feature information for tour participation
/// </summary>
public sealed class RequiredFeatureDto
{
    /// <summary>
    /// Feature identifier
    /// </summary>
    public string FeatureId { get; set; } = string.Empty;

    /// <summary>
    /// Feature display name/title (fetched from BasicDefinitions)
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

