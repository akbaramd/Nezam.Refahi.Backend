using Nezam.Refahi.BasicDefinitions.Domain.Entities;

namespace Nezam.Refahi.BasicDefinitions.Contracts.Services;

/// <summary>
/// Service interface for inter-context communication with BasicDefinitions
/// </summary>
public interface IRepresentativeOfficeService
{
    /// <summary>
    /// Get office by ID
    /// </summary>
    Task<RepresentativeOfficeDto?> GetOfficeByIdAsync(Guid officeId);

    /// <summary>
    /// Get office by code
    /// </summary>
    Task<RepresentativeOfficeDto?> GetOfficeByCodeAsync(string officeCode);

    /// <summary>
    /// Get office by external code
    /// </summary>
    Task<RepresentativeOfficeDto?> GetOfficeByExternalCodeAsync(string externalCode);

    /// <summary>
    /// Get all active offices
    /// </summary>
    Task<IEnumerable<RepresentativeOfficeDto>> GetActiveOfficesAsync();
}

/// <summary>
/// DTO for RepresentativeOffice
/// </summary>
public class RepresentativeOfficeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string ExternalCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? ManagerName { get; set; }
    public string? ManagerPhone { get; set; }
    public bool IsActive { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
