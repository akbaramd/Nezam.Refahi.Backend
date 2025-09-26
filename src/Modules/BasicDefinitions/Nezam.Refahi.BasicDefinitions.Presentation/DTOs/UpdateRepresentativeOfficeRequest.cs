using System.ComponentModel.DataAnnotations;

namespace Nezam.Refahi.BasicDefinitions.Presentation.DTOs;

/// <summary>
/// Request DTO for updating an existing RepresentativeOffice
/// </summary>
public sealed class UpdateRepresentativeOfficeRequest
{
    /// <summary>
    /// Unique identifier of the office
    /// </summary>
    [Required(ErrorMessage = "ID is required")]
    public Guid Id { get; set; }

    /// <summary>
    /// Unique code for the office (e.g., "URM001", "TBR002")
    /// </summary>
    [Required(ErrorMessage = "Code is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Code must be between 2 and 50 characters")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// External code used in organizations (e.g., "ORG001", "BRANCH_001")
    /// </summary>
    [Required(ErrorMessage = "External code is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "External code must be between 2 and 50 characters")]
    public string ExternalCode { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the office
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Physical address of the office
    /// </summary>
    [Required(ErrorMessage = "Address is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 500 characters")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Name of the office manager
    /// </summary>
    [StringLength(100, ErrorMessage = "Manager name cannot exceed 100 characters")]
    public string? ManagerName { get; set; }

    /// <summary>
    /// Phone number of the office manager
    /// </summary>
    [StringLength(20, ErrorMessage = "Manager phone cannot exceed 20 characters")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? ManagerPhone { get; set; }

    /// <summary>
    /// Whether the office is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date when the office was established
    /// </summary>
    public DateTime? EstablishedDate { get; set; }
}
