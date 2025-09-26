using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;

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

    /// <summary>
    /// Get all offices (including inactive)
    /// </summary>
    Task<IEnumerable<RepresentativeOfficeDto>> GetAllOfficesAsync();

    /// <summary>
    /// Create a new representative office
    /// </summary>
    Task<RepresentativeOfficeDto?> CreateOfficeAsync(RepresentativeOfficeDto officeDto);

    /// <summary>
    /// Update an existing representative office
    /// </summary>
    Task<RepresentativeOfficeDto?> UpdateOfficeAsync(RepresentativeOfficeDto officeDto);

    /// <summary>
    /// Delete a representative office
    /// </summary>
    Task<bool> DeleteOfficeAsync(Guid officeId);
}
