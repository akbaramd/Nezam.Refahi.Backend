namespace Nezam.Refahi.Settings.Application.Commands.CreateSection;

/// <summary>
/// Response data for the CreateSectionCommand
/// </summary>
public class CreateSectionResponse
{
    /// <summary>
    /// The ID of the created section
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The name of the section
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The description of the section
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Whether the section is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// When the section was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
