namespace Nezam.Refahi.Membership.Contracts.Services;

/// <summary>
/// Service interface for validating representative office existence
/// This service will call the BasicDefinitions module to validate office IDs
/// </summary>
public interface IAgencyValidationService
{
    /// <summary>
    /// Validates that a representative office exists and is active
    /// </summary>
    /// <param name="AgencyId">The office ID to validate</param>
    /// <returns>True if the office exists and is active, false otherwise</returns>
    Task<bool> ValidateOfficeExistsAsync(Guid AgencyId);

    /// <summary>
    /// Validates multiple representative office IDs
    /// </summary>
    /// <param name="AgencyIds">The office IDs to validate</param>
    /// <returns>Dictionary with office ID as key and validation result as value</returns>
    Task<Dictionary<Guid, bool>> ValidateOfficesExistAsync(IEnumerable<Guid> AgencyIds);

    /// <summary>
    /// Gets basic information about a representative office
    /// </summary>
    /// <param name="AgencyId">The office ID</param>
    /// <returns>Basic office information or null if not found</returns>
    Task<AgencyBasicInfo?> GetOfficeBasicInfoAsync(Guid AgencyId);
}

/// <summary>
/// Basic information about a representative office for validation purposes
/// </summary>
public sealed class AgencyBasicInfo
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
