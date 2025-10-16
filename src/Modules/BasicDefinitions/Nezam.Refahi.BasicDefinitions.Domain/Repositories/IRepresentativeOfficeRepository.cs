using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;

namespace Nezam.Refahi.BasicDefinitions.Domain.Repositories;

/// <summary>
/// Repository interface for Agency aggregate
/// </summary>
public interface IAgencyRepository : IRepository<Agency, Guid>
{
    /// <summary>
    /// Get office by its unique code
    /// </summary>
    Task<Agency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get office by its external code
    /// </summary>
    Task<Agency?> GetByExternalCodeAsync(string externalCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active offices
    /// </summary>
    Task<IEnumerable<Agency>> GetActiveOfficesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get offices by manager name
    /// </summary>
    Task<IEnumerable<Agency>> GetByManagerAsync(string managerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if office code exists
    /// </summary>
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if office code exists excluding specific office
    /// </summary>
    Task<bool> CodeExistsAsync(string code, Guid excludeOfficeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if external code exists
    /// </summary>
    Task<bool> ExternalCodeExistsAsync(string externalCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if external code exists excluding specific office
    /// </summary>
    Task<bool> ExternalCodeExistsAsync(string externalCode, Guid excludeOfficeId, CancellationToken cancellationToken = default);
}
