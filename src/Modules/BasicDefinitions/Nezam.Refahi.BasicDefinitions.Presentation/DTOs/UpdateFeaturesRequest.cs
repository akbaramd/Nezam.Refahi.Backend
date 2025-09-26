using System.ComponentModel.DataAnnotations;

namespace Nezam.Refahi.BasicDefinitions.Presentation.DTOs;

/// <summary>
/// Request DTO for updating an existing Features
/// </summary>
public sealed class UpdateFeaturesRequest
{
    /// <summary>
    /// Unique identifier of the feature
    /// </summary>
    [Required(ErrorMessage = "ID is required")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Title of the feature
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Type of the feature
    /// </summary>
    [Required(ErrorMessage = "Type is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Type must be between 2 and 100 characters")]
    public string Type { get; set; } = string.Empty;
}
