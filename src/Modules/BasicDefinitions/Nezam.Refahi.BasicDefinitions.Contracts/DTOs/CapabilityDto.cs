using System.ComponentModel.DataAnnotations;

namespace Nezam.Refahi.BasicDefinitions.Contracts.DTOs;

/// <summary>
/// Data Transfer Object for Capability entity
/// </summary>
public sealed class CapabilityDto
{
    /// <summary>
    /// Unique identifier of the capability
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the capability
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the capability
    /// </summary>
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
    /// List of features associated with this capability
    /// </summary>
    public List<FeaturesDto> Features { get; set; } = new();

    /// <summary>
    /// Date when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who created the record
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Date when the record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    public string? UpdatedBy { get; set; }
}
