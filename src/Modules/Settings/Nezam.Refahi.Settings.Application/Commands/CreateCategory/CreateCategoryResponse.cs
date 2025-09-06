namespace Nezam.Refahi.Settings.Application.Commands.CreateCategory;

/// <summary>
/// Response data for the CreateCategoryCommand
/// </summary>
public class CreateCategoryResponse
{
    /// <summary>
    /// The ID of the created category
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The description of the category
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The ID of the section this category belongs to
    /// </summary>
    public Guid SectionId { get; set; }
    
    /// <summary>
    /// The display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Whether the category is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// When the category was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
