using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.BasicDefinitions.Application.Services;

/// <summary>
/// Application service interface for RepresentativeOffice operations
/// </summary>
public interface IRepresentativeOfficeApplicationService
{
    /// <summary>
    /// Get office by ID
    /// </summary>
    Task<ApplicationResult<RepresentativeOfficeDto>> GetOfficeByIdAsync(Guid officeId);

    /// <summary>
    /// Get office by code
    /// </summary>
    Task<ApplicationResult<RepresentativeOfficeDto>> GetOfficeByCodeAsync(string officeCode);

    /// <summary>
    /// Get office by external code
    /// </summary>
    Task<ApplicationResult<RepresentativeOfficeDto>> GetOfficeByExternalCodeAsync(string externalCode);

    /// <summary>
    /// Get all active offices
    /// </summary>
    Task<ApplicationResult<IEnumerable<RepresentativeOfficeDto>>> GetActiveOfficesAsync();

    /// <summary>
    /// Get all offices (including inactive)
    /// </summary>
    Task<ApplicationResult<IEnumerable<RepresentativeOfficeDto>>> GetAllOfficesAsync();

    /// <summary>
    /// Create a new representative office
    /// </summary>
    Task<ApplicationResult<RepresentativeOfficeDto>> CreateOfficeAsync(RepresentativeOfficeDto officeDto);

    /// <summary>
    /// Update an existing representative office
    /// </summary>
    Task<ApplicationResult<RepresentativeOfficeDto>> UpdateOfficeAsync(RepresentativeOfficeDto officeDto);

    /// <summary>
    /// Delete a representative office
    /// </summary>
    Task<ApplicationResult<bool>> DeleteOfficeAsync(Guid officeId);
}
