using System.ComponentModel.DataAnnotations;

namespace Nezam.Refahi.BasicDefinitions.Presentation.DTOs;

/// <summary>
/// Request DTO for creating a new Capability
/// </summary>
public sealed class CreateCapabilityRequest
{
    /// <summary>
    /// Unique key for the capability
    /// </summary>
    [Required(ErrorMessage = "Key is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Key must be between 2 and 100 characters")]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Name of the capability
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the capability
    /// </summary>
    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 500 characters")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Date from which the capability is valid
    /// </summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// Date until which the capability is valid
    /// </summary>
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// List of feature IDs to associate with this capability
    /// </summary>
    public List<string> FeatureIds { get; set; } = new();
}
