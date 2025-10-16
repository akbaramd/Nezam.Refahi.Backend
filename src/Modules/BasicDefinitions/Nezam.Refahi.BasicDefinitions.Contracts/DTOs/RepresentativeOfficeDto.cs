using System.ComponentModel.DataAnnotations;

namespace Nezam.Refahi.BasicDefinitions.Contracts.DTOs;

/// <summary>
/// Data Transfer Object for Agency entity
/// </summary>
public sealed class AgencyDto
{
    /// <summary>
    /// Unique identifier of the office
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique code for the office (e.g., "URM001", "TBR002")
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// External code used in organizations (e.g., "ORG001", "BRANCH_001")
    /// </summary>
    public string ExternalCode { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the office
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Physical address of the office
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Name of the office manager
    /// </summary>
    public string? ManagerName { get; set; }

    /// <summary>
    /// Phone number of the office manager
    /// </summary>
    public string? ManagerPhone { get; set; }

    /// <summary>
    /// Whether the office is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date when the office was established
    /// </summary>
    public DateTime? EstablishedDate { get; set; }

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

