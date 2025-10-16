using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;

namespace Nezam.Refahi.BasicDefinitions.Contracts.Services;

/// <summary>
/// Service interface for inter-context communication with BasicDefinitions
/// </summary>
public interface IAgencyService
{
    /// <summary>
    /// Get office by ID
    /// </summary>
    Task<AgencyDto?> GetOfficeByIdAsync(Guid officeId);

    /// <summary>
    /// Get office by code
    /// </summary>
    Task<AgencyDto?> GetOfficeByCodeAsync(string officeCode);

    /// <summary>
    /// Get office by external code
    /// </summary>
    Task<AgencyDto?> GetOfficeByExternalCodeAsync(string externalCode);

    /// <summary>
    /// Get all active offices
    /// </summary>
    Task<IEnumerable<AgencyDto>> GetActiveOfficesAsync();

    /// <summary>
    /// Get all offices (including inactive)
    /// </summary>
    Task<IEnumerable<AgencyDto>> GetAllOfficesAsync();

    /// <summary>
    /// Create a new representative office
    /// </summary>
    Task<AgencyDto?> CreateOfficeAsync(AgencyDto officeDto);

    /// <summary>
    /// Update an existing representative office
    /// </summary>
    Task<AgencyDto?> UpdateOfficeAsync(AgencyDto officeDto);

    /// <summary>
    /// Delete a representative office
    /// </summary>
    Task<bool> DeleteOfficeAsync(Guid officeId);
}
