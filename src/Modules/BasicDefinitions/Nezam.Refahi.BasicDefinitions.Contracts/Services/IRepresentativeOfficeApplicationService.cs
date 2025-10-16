using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.BasicDefinitions.Contracts.Services;

/// <summary>
/// Application service interface for Agency operations
/// </summary>
public interface IAgencyApplicationService
{
    /// <summary>
    /// Get office by ID
    /// </summary>
    Task<ApplicationResult<AgencyDto>> GetOfficeByIdAsync(Guid officeId);

    /// <summary>
    /// Get office by code
    /// </summary>
    Task<ApplicationResult<AgencyDto>> GetOfficeByCodeAsync(string officeCode);

    /// <summary>
    /// Get office by external code
    /// </summary>
    Task<ApplicationResult<AgencyDto>> GetOfficeByExternalCodeAsync(string externalCode);

    /// <summary>
    /// Get all active offices
    /// </summary>
    Task<ApplicationResult<IEnumerable<AgencyDto>>> GetActiveOfficesAsync();

    /// <summary>
    /// Get all offices (including inactive)
    /// </summary>
    Task<ApplicationResult<IEnumerable<AgencyDto>>> GetAllOfficesAsync();

    /// <summary>
    /// Create a new representative office
    /// </summary>
    Task<ApplicationResult<AgencyDto>> CreateOfficeAsync(AgencyDto officeDto);

    /// <summary>
    /// Update an existing representative office
    /// </summary>
    Task<ApplicationResult<AgencyDto>> UpdateOfficeAsync(AgencyDto officeDto);

    /// <summary>
    /// Delete a representative office
    /// </summary>
    Task<ApplicationResult<bool>> DeleteOfficeAsync(Guid officeId);
}
