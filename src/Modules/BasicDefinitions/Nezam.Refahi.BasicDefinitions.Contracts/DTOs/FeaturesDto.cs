using System.ComponentModel.DataAnnotations;

namespace Nezam.Refahi.BasicDefinitions.Contracts.DTOs;

/// <summary>
/// Data Transfer Object for Features entity
/// </summary>
public sealed class FeaturesDto
{
    /// <summary>
    /// Unique identifier of the feature
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Title of the feature
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Type of the feature
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Whether the feature is currently active
    /// </summary>
    public bool IsActive { get; set; }

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
