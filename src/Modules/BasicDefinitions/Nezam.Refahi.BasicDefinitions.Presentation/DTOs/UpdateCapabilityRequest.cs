using System.ComponentModel.DataAnnotations;

namespace Nezam.Refahi.BasicDefinitions.Presentation.DTOs;

/// <summary>
/// Request DTO for updating an existing Capability
/// </summary>
public sealed class UpdateCapabilityRequest
{
    /// <summary>
    /// Unique identifier of the capability
    /// </summary>
    [Required(ErrorMessage = "ID is required")]
    public string Id { get; set; } = string.Empty;

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
    /// Whether the capability is currently active
    /// </summary>
    public bool IsActive { get; set; }

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
